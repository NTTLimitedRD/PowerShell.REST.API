using System.Text;
using System.Text.RegularExpressions;
using DynamicPowerShellApi.Model;

namespace DynamicPowerShellApi
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.IO;
	using System.Linq;
	using System.Management.Automation;
	using System.Management.Automation.Runspaces;
	using System.Threading.Tasks;

	/// <summary>
	/// The PowerShell runner.
	/// </summary>
	public class PowershellRunner : IRunner
	{
		/// <summary>
		/// The asynchronous execution method.
		/// </summary>
		/// <param name="filename">
		/// The filename.
		/// </param>
		/// <param name="snapin">
		/// The snap in.
		/// </param>
		/// <param name="parametersList">
		/// The parameters List.
		/// </param>
		/// <returns>
		/// The <see cref="Task"/>.
		/// </returns>
		/// <exception cref="PSSnapInException">
		/// </exception>
		public Task<PowershellReturn> ExecuteAsync(string filename, string snapin, IList<KeyValuePair<string, string>> parametersList)
		{
			if (string.IsNullOrWhiteSpace(filename))
				throw new ArgumentException("Argument cannot be null, empty or composed of whitespaces only", "filename");
			if (parametersList == null)
				throw new ArgumentNullException("parametersList", "Argument cannot be null");

			// Raise an event so we know what is going on
			try
			{
				var sb = new StringBuilder();

				foreach (KeyValuePair<string, string> kvp in parametersList)
				{
					if (sb.Length > 0)
						sb.Append(";");

					sb.Append(string.Format("{0}:{1}", kvp.Key, kvp.Value));
				}

				DynamicPowershellApiEvents
					.Raise
					.ExecutingPowerShellScript(filename, sb.ToString());
			}
			catch (Exception)
			{
				DynamicPowershellApiEvents
					.Raise
					.ExecutingPowerShellScript(filename, "Unknown");		        
			}
			
			try
			{
				string strBaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
				string scriptContent = File.ReadAllText(Path.Combine(strBaseDirectory, Path.Combine("ScriptRepository", filename)));

				RunspaceConfiguration rsConfig = RunspaceConfiguration.Create();

				if (!String.IsNullOrWhiteSpace(snapin))
				{
					PSSnapInException snapInException;
					PSSnapInInfo info = rsConfig.AddPSSnapIn(snapin, out snapInException);
					Console.WriteLine("DEBUG: Loading snapin {0}", snapin);
					if (snapInException != null)
					{
						DynamicPowershellApiEvents
							.Raise
							.SnapinException(snapInException.Message);
						throw snapInException;
					}
				}

				using (PowerShell powerShellInstance = PowerShell.Create(InitialSessionState.Create()))
				{
					powerShellInstance.RunspacePool = RunspacePoolWrapper.Pool;
					if (powerShellInstance.Runspace == null)
					{
						powerShellInstance.Runspace = RunspaceFactory.CreateRunspace(rsConfig);
						powerShellInstance.Runspace.Open();
					}

					powerShellInstance.AddScript(scriptContent);

					foreach (var item in parametersList)
					{
						powerShellInstance.AddParameter(item.Key, item.Value);
						Console.WriteLine("The parameters are {0}-{1}", item.Key, item.Value);
					}

					// invoke execution on the pipeline (collecting output)
					Collection<PSObject> psOutput = powerShellInstance.Invoke();
					string sMessage = psOutput.LastOrDefault() != null ? Regex.Replace(psOutput.LastOrDefault().ToString(), @"[^\u0000-\u007F]", string.Empty) : "";

					DynamicPowershellApiEvents.Raise.PowerShellScriptFinalised("The powershell has completed - anlaysing results now");

					// check the other output streams (for example, the error stream)
					if (powerShellInstance.HadErrors)
					{						
						// Create a string builder for the errors
						StringBuilder sb = new StringBuilder();

						// error records were written to the error stream.
						// do something with the items found.
						Console.WriteLine("PowerShell script crashed with errors:");
						sb.Append("PowerShell script crashed with errors:" + Environment.NewLine);
						sb.Append(String.Format("{0}", sMessage));

						var errors = powerShellInstance.Streams.Error.ReadAll();
						if (errors != null)
						{
							foreach (var error in errors)
							{
								if (error.ErrorDetails == null )
									DynamicPowershellApiEvents.Raise.UnhandledException("error.ErrorDetails is null");

								string errorDetails = error.ErrorDetails != null ? error.ErrorDetails.Message : "";
								string scriptStack = error.ScriptStackTrace != null ? error.ScriptStackTrace : "";
								string commandPath = error.InvocationInfo.PSCommandPath != null ? error.InvocationInfo.PSCommandPath : "";

								if (error.ScriptStackTrace == null)
									DynamicPowershellApiEvents.Raise.UnhandledException("error.ScriptStackTrace is null");

								if (error.InvocationInfo == null)
									DynamicPowershellApiEvents.Raise.UnhandledException("error.InvocationInfo is null");
								else
								{
									if (error.InvocationInfo.PSCommandPath == null)
										DynamicPowershellApiEvents.Raise.UnhandledException("error.InvocationInfo.PSCommandPath is null");	
								}
								
								if (error.Exception == null)
									DynamicPowershellApiEvents.Raise.UnhandledException("error.Exception is null");

								DynamicPowershellApiEvents.Raise.PowerShellError(errorDetails, scriptStack, commandPath, error.InvocationInfo.ScriptLineNumber);

								if (error.Exception != null)
								{
									Console.WriteLine("PowerShell Exception {0} : {1}", error.Exception.Message, error.Exception.StackTrace);
									sb.Append(String.Format("PowerShell Exception {0} : {1}", error.Exception.Message, error.Exception.StackTrace));
								}

								Console.WriteLine("Error '{0}'", error.ScriptStackTrace);
								sb.Append(String.Format("Error {0}", error.ScriptStackTrace));
							}
						}
						else
						{
							
							sb.Append(sMessage);
						}

						Console.WriteLine("Creating a new PowershellReturn object");
						DynamicPowershellApiEvents.Raise.PowerShellScriptFinalised(String.Format("An error was rasied {0}", sb.ToString()));

						// Create a new return value
						var ps = new PowershellReturn
						{
							PowerShellReturnedValidData = false,
							ActualPowerShellData = sb.ToString()
						};

						// Now return it
						return Task.FromResult(ps);
					}
					
					PSObject lastMessage = psOutput.LastOrDefault();

					var psGood = new PowershellReturn
					{
						PowerShellReturnedValidData = true,
						ActualPowerShellData = sMessage
					};

					DynamicPowershellApiEvents.Raise.PowerShellScriptFinalised(String.Format("The powershell returned the following {0}", psGood.ActualPowerShellData));

					return Task.FromResult(psGood);
				}
			}
			catch(Exception runnerException)
			{
				DynamicPowershellApiEvents.Raise.UnhandledException(runnerException.Message, runnerException.StackTrace);
				throw;
			}
		}
	}
}