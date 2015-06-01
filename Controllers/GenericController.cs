using System.IO;
using System.Management.Automation;
using System.Web.Http.Results;
using DynamicPowerShellApi.Jobs;
using DynamicPowerShellApi.Logging;
using DynamicPowerShellApi.Model;
using Newtonsoft.Json;

namespace DynamicPowerShellApi.Controllers
{
	using Configuration;
	using Exceptions;
	using Microsoft.Owin;
	using Newtonsoft.Json.Linq;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Net;
	using System.Net.Http;
	using System.Threading.Tasks;
	using System.Web.Http;

	/// <summary>
	/// Generic controller for running PowerShell commands.
	/// </summary>
	public class GenericController : ApiController
	{
		/// <summary>
		/// The PowerShell runner.
		/// </summary>
		private readonly IRunner _powershellRunner;

		/// <summary>
		/// Crash logger.
		/// </summary>
		private readonly ICrashLogger _crashLogger;

		/// <summary>	The job list provider. </summary>
		private readonly IJobListProvider _jobListProvider;

		/// <summary>
		/// Initialises a new instance of the <see cref="GenericController"/> class.
		/// </summary>
		public GenericController()
		{
		}

		/// <summary>	Initialises a new instance of the <see cref="GenericController"/> class. </summary>
		/// <remarks>	Anthony, 6/1/2015. </remarks>
		/// <exception cref="ArgumentNullException">	Thrown when one or more required arguments are
		/// 											null. </exception>
		/// <param name="powershellRunner">	The PowerShell runner. </param>
		/// <param name="crashLogger">	   	An implementation of a crash logger. </param>
		/// <param name="jobListProvider"> 	The job list provider. </param>
		public GenericController(IRunner powershellRunner, ICrashLogger crashLogger, IJobListProvider jobListProvider)
		{
			if (jobListProvider == null)
				throw new ArgumentNullException("jobListProvider");
			if (crashLogger == null)
				throw new ArgumentNullException("crashLogger");
			if (powershellRunner == null)
				throw new ArgumentNullException("powershellRunner");

			_powershellRunner = powershellRunner;
			_crashLogger = crashLogger;
			_jobListProvider = jobListProvider;
		}

		/// <summary>
		/// Get a status message
		/// </summary>
		/// <returns>OK always</returns>
		[Route("status")]
		[AllowAnonymous]
		public HttpResponseMessage Status()
		{
			return new HttpResponseMessage {Content = new StringContent("OK")};
		}

		[Route("jobs")]
		public dynamic AllJobStatus()
		{
			dynamic jobs = new
			{
				running = _jobListProvider.GetRunningJobs(),
				completed = _jobListProvider.GetCompletedJobs()
			};

			return jobs;
		}

		/// <summary>	Gets a job. </summary>
		/// <remarks>	Anthony, 6/1/2015. </remarks>
		/// <exception cref="ArgumentOutOfRangeException">	Thrown when one or more arguments are outside
		/// 												the required range. </exception>
		/// <param name="jobId">	Identifier for the job. </param>
		/// <returns>	The job. </returns>
		[Route("job")]
		public dynamic GetJob(string jobId)
		{
			Guid jobGuid;
			if (!Guid.TryParse(jobId, out jobGuid))
				throw new ArgumentOutOfRangeException("jobId is not a valid GUID");

			using (TextReader reader = new StreamReader(
				Path.Combine(WebApiConfiguration.Instance.Jobs.JobStorePath, jobId + ".json")))
			{
				dynamic d = JObject.Parse(reader.ReadToEnd());
				return d;
			}
		}

		/// <summary>
		/// The process request async.
		/// </summary>
		/// <returns>
		/// The <see cref="Task"/>.
		/// </returns>
		/// <exception cref="MalformedUriException">
		/// </exception>
		/// <exception cref="WebApiNotFoundException">
		/// </exception>
		/// <exception cref="WebMethodNotFoundException">
		/// </exception>
		/// <exception cref="MissingParametersException">
		/// </exception>
		/// <exception cref="Exception">
		/// </exception>
		[AuthorizeIfEnabled]
		public async Task<HttpResponseMessage> ProcessRequestAsync(HttpRequestMessage request = null)
		{
			DynamicPowershellApiEvents
				.Raise
				.ReceivedRequest(Request.RequestUri.ToString());

			Guid activityId = Guid.NewGuid();

			if (Request.RequestUri.Segments.Length < 4)
				throw new MalformedUriException(string.Format("There is {0} segments but must be at least 4 segments in the URI.", Request.RequestUri.Segments.Length));

			string apiName = Request.RequestUri.Segments[2].Replace("/", string.Empty);
			string methodName = Request.RequestUri.Segments[3].Replace("/", string.Empty);

			// Check that the verbose messaging is working
			DynamicPowershellApiEvents.Raise.VerboseMessaging(String.Format("The api name is {0} and the method is {1}", apiName, methodName));

			// find the api.
			var api = WebApiConfiguration.Instance.Apis[apiName];
			if (api == null)
			{
				// Check that the verbose messaging is working
				DynamicPowershellApiEvents.Raise.VerboseMessaging(String.Format("Cannot find the requested web API: {0}", apiName));
				throw new WebApiNotFoundException(string.Format("Cannot find the requested web API: {0}", apiName));
			}

			// find the web method.
			WebMethod method = api.WebMethods[methodName];
			if (method == null)
			{
				DynamicPowershellApiEvents.Raise.VerboseMessaging(String.Format("Cannot find the requested web method: {0}", methodName));
				throw new WebMethodNotFoundException(string.Format("Cannot find web method: {0}", methodName));
			}
			
			// Is this scheduled as a job?
			bool asJob = method.AsJob;

			// Is this a POST method
			IEnumerable<KeyValuePair<string, string>> query2;			
			if (Request.Method == HttpMethod.Post)
			{
				string documentContents = await Request.Content.ReadAsStringAsync();

				try
				{
					// Work out the parameters from JSON
					var queryStrings = new Dictionary<string, string>();
					JToken token = documentContents.StartsWith("[") ? (JToken)JArray.Parse(documentContents) : JObject.Parse(documentContents);
											
					foreach (var details in token)
					{
						var name = details.First.First.ToString();
						var value = details.Last.First.ToString();
						
						queryStrings.Add(name, value);
					}

					if (method.Parameters.Any(param => queryStrings.All(q => q.Key != param.Name)))
					{
						DynamicPowershellApiEvents.Raise.VerboseMessaging(String.Format("Cannot find all parameters required."));
						throw new MissingParametersException("Cannot find all parameters required.");
					}
											
					query2 = queryStrings.ToList();
				}
				catch (Exception )
				{
					DynamicPowershellApiEvents.Raise.VerboseMessaging(String.Format("Cannot find all parameters required."));
					throw new MissingParametersException("Cannot find all parameters required.");
				}				
			}
			else
			{
				// Get our parameters.
				IEnumerable<KeyValuePair<string, string>> query = Request.GetQueryNameValuePairs();
				if (method.Parameters.Any(param => query.All(q => q.Key != param.Name)))
				{
					DynamicPowershellApiEvents.Raise.VerboseMessaging(String.Format("Cannot find all parameters required."));
					throw new MissingParametersException("Cannot find all parameters required.");
				}

				query2 = query;
			}			

			// We now catch an exception from the runner
			try
			{
				DynamicPowershellApiEvents.Raise.VerboseMessaging(String.Format("Started Executing the runner"));

				if (!asJob)
				{
					PowershellReturn output =
						await _powershellRunner.ExecuteAsync(method.PowerShellPath, method.Snapin, method.Module, query2.ToList(), asJob);

					JToken token = output.ActualPowerShellData.StartsWith("[")
						? (JToken) JArray.Parse(output.ActualPowerShellData)
						: JObject.Parse(output.ActualPowerShellData);

					return new HttpResponseMessage
					{
						Content = new JsonContent(token)
					};
				}
				else // run as job.
				{
					Guid jobId = Guid.NewGuid();
					string requestedHost = String.Empty;

					if (Request.Properties.ContainsKey("MS_OwinContext"))
						requestedHost = ((OwinContext)Request.Properties["MS_OwinContext"]).Request.RemoteIpAddress;

					_jobListProvider.AddRequestedJob(jobId, requestedHost );

					// Go off and run this job please sir.
					var task = Task<bool>.Factory.StartNew(
						() =>
						{
							try
							{
								Task<PowershellReturn> goTask =  
								_powershellRunner.ExecuteAsync(
										method.PowerShellPath,
										method.Snapin,
										method.Module,
										query2.ToList(),
										true);

								goTask.Wait();
								var output = goTask.Result;

								JToken token = output.ActualPowerShellData.StartsWith("[")
								? (JToken)JArray.Parse(output.ActualPowerShellData)
								: JObject.Parse(output.ActualPowerShellData);

								_jobListProvider.CompleteJob(jobId, output.PowerShellReturnedValidData, String.Empty);

								string outputPath = Path.Combine(WebApiConfiguration.Instance.Jobs.JobStorePath, jobId + ".json");

								using (TextWriter writer = File.CreateText(outputPath))
								{
									JsonSerializer serializer = new JsonSerializer
									{
										Formatting = Formatting.Indented // Make it readable for Ops sake!
									};
									serializer.Serialize(writer, token);
								}

								return true;
							}
							catch (PowerShellExecutionException poException)
							{
								CrashLogEntry entry = new CrashLogEntry
								{
									Exceptions = poException.Exceptions,
									LogTime = poException.LogTime,
									RequestAddress = requestedHost, 
									RequestMethod = methodName,
									RequestUrl = Request.RequestUri.ToString()
								};
								entry.SetActivityId(activityId);
								string logFile = _crashLogger.SaveLog(entry);

								DynamicPowershellApiEvents.Raise.InvalidPowerShellOutput(poException.Message + " logged to " + logFile);

								ErrorResponse response =
										new ErrorResponse
										{
											ActivityId = activityId,
											LogFile = logFile,
											Message = poException.Message
										};

								JToken token = new JObject(response);

								_jobListProvider.CompleteJob(jobId, false, String.Empty);

								string outputPath = Path.Combine(WebApiConfiguration.Instance.Jobs.JobStorePath, jobId + ".json");

								using (TextWriter writer = File.CreateText(outputPath))
								{
									JsonSerializer serializer = new JsonSerializer
									{
										Formatting = Formatting.Indented // Make it readable for Ops sake!
									};
									serializer.Serialize(writer, token);
								}
								return true;
							}
						}
					);
					
					// return the Job ID.
					return new HttpResponseMessage
					{
						Content = new JsonContent(new JValue(jobId))
					};
				}
			}
			catch (PowerShellExecutionException poException)
			{
				CrashLogEntry entry = new CrashLogEntry
				{
					Exceptions = poException.Exceptions,
					LogTime = poException.LogTime,
					RequestAddress = String.Empty, // TODO: Find a way of getting the request host.
					RequestMethod = methodName,
					RequestUrl = Request.RequestUri.ToString()
				};
				entry.SetActivityId(activityId);
				string logFile = _crashLogger.SaveLog(entry);

				DynamicPowershellApiEvents.Raise.InvalidPowerShellOutput(poException.Message + " logged to " + logFile);

				HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.InternalServerError, 
						new ErrorResponse
						{
							ActivityId = activityId,
							LogFile = logFile,
							Message = poException.Message
						}
						);				

				return response;
			}
			catch (Exception ex)
			{
				CrashLogEntry entry = new CrashLogEntry
				{
					Exceptions = new List<PowerShellException>
					{
						new PowerShellException
						{
							ErrorMessage = ex.Message,
							LineNumber = 0,
							ScriptName = "GenericController.cs",
							StackTrace = ex.StackTrace
						}
					},
					LogTime = DateTime.Now,
					RequestAddress = String.Empty, // TODO: Find a way of getting the request host.
					RequestMethod = methodName,
					RequestUrl = Request.RequestUri.ToString()
				};
				entry.SetActivityId(activityId);
				string logFile = _crashLogger.SaveLog(entry);

				DynamicPowershellApiEvents.Raise.UnhandledException(ex.Message + " logged to " + logFile, ex.StackTrace ?? String.Empty);

				HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.InternalServerError, 
						new ErrorResponse
						{
							ActivityId = activityId,
							LogFile = logFile,
							Message = ex.Message
						}
						);				

				return response;
			}			
		}
	}
}