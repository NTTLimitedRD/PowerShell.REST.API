namespace DynamicPowerShellApi.Controllers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Net;
	using System.Net.Http;
	using System.Threading.Tasks;
	using System.Web.Http;

	using Configuration;
	using Exceptions;

	using Newtonsoft.Json.Linq;

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
		/// Initialises a new instance of the <see cref="GenericController"/> class.
		/// </summary>
		public GenericController()
		{
		}

		/// <summary>
		/// Initialises a new instance of the <see cref="GenericController"/> class.
		/// </summary>
		/// <param name="powershellRunner">
		/// The PowerShell runner.
		/// </param>
		public GenericController(IRunner powershellRunner)
		{
			this._powershellRunner = powershellRunner;
		}

		/// <summary>
		/// Get a status message
		/// </summary>
		/// <returns>OK always</returns>
		[Route("status")]
		[AllowAnonymous]
		public HttpResponseMessage Status()
		{
			return new HttpResponseMessage { Content = new StringContent("OK") };
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
		[Authorize]
		public async Task<HttpResponseMessage> ProcessRequestAsync()
		{
			DynamicPowershellApiEvents
				.Raise
				.ReceivedRequest(Request.RequestUri.ToString());
			
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

			// Get our parameters.
			IEnumerable<KeyValuePair<string, string>> query = Request.GetQueryNameValuePairs();
			if (method.Parameters.Any(param => query.All(q => q.Key != param.Name)))
			{
				DynamicPowershellApiEvents.Raise.VerboseMessaging(String.Format("Cannot find all parameters required."));
				throw new MissingParametersException("Cannot find all parameters required.");
			}

			// We now catch an exception from the runner
			try
			{
				DynamicPowershellApiEvents.Raise.VerboseMessaging(String.Format("Started Executing the runner"));

				var output = await _powershellRunner.ExecuteAsync(method.PowerShellPath, method.Snapin, query.ToList());
				
				if (output.PowerShellReturnedValidData == true)
				{                 
					JToken token = output.ActualPowerShellData.StartsWith("[") ? (JToken)JArray.Parse(output.ActualPowerShellData) : JObject.Parse(output.ActualPowerShellData);
					return new HttpResponseMessage { Content = new JsonContent(token) };
				}
				else
				{
					DynamicPowershellApiEvents.Raise.UnhandledException(output.ActualPowerShellData);

					return new HttpResponseMessage
					{
						StatusCode = HttpStatusCode.InternalServerError,
						Content = new StringContent(output.ActualPowerShellData)
					};    
				}
			}
			catch (Exception ex)
			{
				DynamicPowershellApiEvents.Raise.UnhandledException(ex.Message);

				return new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.InternalServerError,
					Content = new StringContent(ex.Message)
				};
			}			
		} 
	}
}