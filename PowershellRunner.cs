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
		public Task<string> ExecuteAsync(string filename, string snapin, IList<KeyValuePair<string, string>> parametersList)
		{
			if (string.IsNullOrWhiteSpace(filename))
				throw new ArgumentException("Argument cannot be null, empty or composed of whitespaces only", "filename");
			if (parametersList == null)
				throw new ArgumentNullException("parametersList", "Argument cannot be null");
			
			DynamicPowershellApiEvents
				.Raise
				.ExecutingPowerShellScript(filename);

			try
			{
				string scriptContent = File.ReadAllText(Path.Combine("ScriptRepository", filename));

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
					}

					// invoke execution on the pipeline (collecting output)
					Collection<PSObject> psOutput = powerShellInstance.Invoke();

					// check the other output streams (for example, the error stream)
					if (powerShellInstance.Streams.Error.Count > 0)
					{
						// error records were written to the error stream.
						// do something with the items found.
						Console.WriteLine("PowerShell script crashed with errors:");

						var errors = powerShellInstance.Streams.Error.ReadAll();
						if (errors != null)
						{
							foreach (var error in errors)
							{
								if (error.Exception != null)
									Console.WriteLine("PowerShell Exception {0} : {1}", error.Exception.Message, error.Exception.StackTrace);

								Console.WriteLine(" Error '{0}'", error.ScriptStackTrace);
							}
						}

						return null;
					}
					
					PSObject lastMessage = psOutput.LastOrDefault();
					
					return Task.FromResult(lastMessage == null ? string.Empty : lastMessage.ToString());
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Failed to load powershell script : {0}", ex.Message);
				Console.Write(ex.StackTrace);
				return null;
			}
		}
	}
}
