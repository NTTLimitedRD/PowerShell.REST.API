using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Threading.Tasks;
using DD.Cloud.Aperture.Platform.Core.Linq;

namespace DynamicPowerShellApi
{
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
		/// <param name="module">
		/// The module.
		/// </param>
		/// <param name="parametersList">
		/// The parameters List.
		/// </param>
		/// <returns>
		/// The <see cref="Task"/>.
		/// null if error
		/// </returns>
		public Task<string> ExecuteAsync(string filename, string snapin, string module, IList<KeyValuePair<string, string>> parametersList)
		{
			if (string.IsNullOrWhiteSpace(filename))
				throw new ArgumentException("Argument cannot be null, empty or composed of whitespaces only", "filename");

			if (parametersList == null)
				throw new ArgumentNullException("parametersList", "Argument cannot be null");

			DynamicPowershellApiEvents
				.Raise
				.ExecutingPowerShellScript(filename);

			RunspaceConfiguration rsConfig = RunspaceConfiguration.Create();

			if (!String.IsNullOrWhiteSpace(snapin))
			{
				DynamicPowershellApiEvents
					.Raise
					.LoadingSnapIn(snapin);

				PSSnapInException snapInException;
				rsConfig.AddPSSnapIn(snapin, out snapInException);

				if (snapInException != null)
				{
					DynamicPowershellApiEvents
						.Raise
						.SnapinException(snapInException.Message);

					throw snapInException;
				}
			}

			InitialSessionState initialSession = InitialSessionState.Create();

			if (!String.IsNullOrWhiteSpace(module))
			{
				DynamicPowershellApiEvents
					.Raise
					.LoadingModule(module);

				initialSession.ImportPSModule(new string[] { module });
			}

			using (PowerShell powerShellInstance = PowerShell.Create(initialSession))
			{
				powerShellInstance.RunspacePool = RunspacePoolWrapper.Pool;
				if (powerShellInstance.Runspace == null)
				{
					powerShellInstance.Runspace = RunspaceFactory.CreateRunspace(rsConfig);
					powerShellInstance.Runspace.Open();
				}

				powerShellInstance
					.AddScript(
						File.ReadAllText(Path.Combine("ScriptRepository", filename))
					);

				parametersList.ForEach(item =>
					// ReSharper disable once AccessToDisposedClosure
					powerShellInstance.AddParameter(
							item.Key,
							item.Value
					)
				);

				// invoke execution on the pipeline (collecting output)
				Collection<PSObject> psOutput = powerShellInstance.Invoke();

				// check the other output streams (for example, the error stream)
				if (powerShellInstance.Streams.Error.Count > 0)
				{
					// error records were written to the error stream.
					var errors = powerShellInstance.Streams.Error.ReadAll();
					if ((errors == null) || (errors.Count == 0))
					{
						DynamicPowershellApiEvents
							.Raise
							.ScriptExecutionException("PowerShell script failed with unknown error(s).");

						throw new Exception("PowerShell script failed with unknown error(s).");
					}

					errors.ForEach(error =>
						DynamicPowershellApiEvents
							.Raise
							.ScriptExecutionException(error.ErrorDetails.Message));

					Exception exception = errors.First().Exception;
					if (exception != null)
						throw exception;
				}

				var lastMessage = psOutput.LastOrDefault();
				return Task
					.FromResult(
						lastMessage == null ? string.Empty : lastMessage.ToString()
					);
			}
		}
	}
}
