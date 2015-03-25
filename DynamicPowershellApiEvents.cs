using System.Collections;

namespace DynamicPowerShellApi
{
	using System;
	using System.Management.Automation.Runspaces;

	using Microsoft.Diagnostics.Tracing;

	/// <summary>
	/// The dynamic PowerShell API event source.
	/// </summary>
	[EventSource(Name = EventSourceName)]
	public sealed class DynamicPowershellApiEvents : EventSource
	{
		/// <summary>
		/// The singleton instance.
		/// </summary>
		private static readonly Lazy<DynamicPowershellApiEvents> Instance =
			new Lazy<DynamicPowershellApiEvents>(() => new DynamicPowershellApiEvents());

		/// <summary>
		/// The Dynamic PowerShell API event source name.
		/// </summary>
		public const string EventSourceName = "DDCloud-DynamicPowershellApi";

		/// <summary>
		///	Singleton instance of the event-source that can be used to raise events.
		/// </summary>
		public static DynamicPowershellApiEvents Raise
		{
			get
			{
				return Instance.Value;
			}
		}

		/// <summary>
		/// Prevents a default instance of the <see cref="DynamicPowershellApiEvents"/> class from being created.
		/// </summary>
		private DynamicPowershellApiEvents()
#if DEBUG
			: base(true /* throwOnEventWriteErrors */)
#endif
		{
		}

		/// <summary>
		/// The start-up event
		/// </summary>                
		[
			Event(
				Events.ApiStartup,
				Message = "Started API",
				Level = EventLevel.Informational
			)
		]
		public void StartUp()
		{
			WriteEvent(Events.ApiStartup);
		}

		/// <summary>
		/// The received request.
		/// </summary>                
		[
			Event(
				Events.ApiStop,
				Message = "Stopped API",
				Level = EventLevel.Informational
			)
		]
		public void Stop()
		{
			WriteEvent(Events.ApiStop);
		}

		/// <summary>
		/// The received request.
		/// </summary>
		/// <param name="requestUri">
		/// The request uri.
		/// </param>
		[
			Event(
				Events.ReceivedRequest,
				Message = "Received request: {0}",
				Level = EventLevel.Informational
			)
		]
		public void ReceivedRequest(string requestUri)
		{
			WriteEvent(Events.ReceivedRequest, requestUri);
		}

		/// <summary>
		/// The PowerShell script has finished.
		/// </summary>
		/// <param name="message">
		/// The message to raise.
		/// </param>        
		[
			Event(
				Events.PowershellStop, 
				Message = "Stopped execution of powershell {0}", 
				Level = EventLevel.Verbose
				)
		]
		public void PowerShellScriptFinalised(string message)
		{
			WriteEvent(Events.PowershellStop, message);
		}

		/// <summary>
		/// Execute the PowerShell script.
		/// </summary>
		/// <param name="powershellPath">
		/// The PowerShell path.
		/// </param>
		/// <param name="paramDetails">
		/// The PowerShell parameters.
		/// </param>
		[
			Event(
				Events.PowershellStart,
				Message = "Started execution of powershell script file {0} - {1}",
				Level = EventLevel.Informational
				)
		]
		public void ExecutingPowerShellScript(string powershellPath, string paramDetails)
		{
			WriteEvent(Events.PowershellStart, powershellPath, paramDetails);
		}

		/// <summary>
		/// The snap in exception log.
		/// </summary>
		/// <param name="errorMessage">
		/// The error message from the <see cref="PSSnapInException"/> object.
		/// </param>
		[
			Event(
				Events.SnapinException,
				Message = "Snapin Exception Raised {0}",
				Level = EventLevel.Error
				)
		]
		public void SnapinException(string errorMessage)
		{
			WriteEvent(Events.SnapinException, errorMessage);
		}

		/// <summary>
		/// Loading Module
		/// </summary>
		/// <param name="module">
		/// Module name.
		/// </param>
		[
			Event(
				Events.LoadingModule,
				Message = "Loading {0} Module.",
				Level = EventLevel.Informational
				)
		]
		public void LoadingModule(string module)
		{
			WriteEvent(Events.LoadingModule, module);
		}

		/// <summary>
		/// The module in exception log.
		/// </summary>
		/// <param name="errorMessage">
		/// The error message from the <see cref="PSException"/> object.
		/// </param>
		[
			Event(
				Events.ModuleException,
				Message = "Module exception: {0}",
				Level = EventLevel.Error
				)
		]
		public void ModuleException(string errorMessage)
		{
			WriteEvent(Events.ModuleException, errorMessage);
		}

		/// <summary>
		/// Invalid PowerShell Output
		/// </summary>
		/// <param name="errorMessage">The error message.</param>
		[
			Event(
				Events.InvalidPowerShellOutput,
				Message = "Invalid PowerShell Output {0}",
				Level = EventLevel.Error
				)
		]
		public void InvalidPowerShellOutput(string errorMessage)
		{
			WriteEvent(Events.UnhandledException, errorMessage);
		}

		/// <summary>
		/// Unhandled Exception message
		/// </summary>
		/// <param name="errorMessage">The error message.</param>
		/// <param name="stackTrace">The stack trace.</param>
		[
			Event(
				Events.UnhandledException,
				Message = "Unhandled exception in service {0}, stack {1}",
				Level = EventLevel.Error
				)
		]
		public void UnhandledException(string errorMessage, string stackTrace = "")
		{
			WriteEvent(Events.UnhandledException, errorMessage, stackTrace);
		}

		/// <summary>
		/// Configuration error.
		/// </summary>
		/// <param name="errorMessage">The error message.</param>
		[
			Event(
				Events.ConfigurationError,
				Message = "Cannot start service from configuration error-  {0}",
				Level = EventLevel.Error
			)
		]
		public void ConfigurationError(string errorMessage)
		{
			WriteEvent(Events.ConfigurationError, errorMessage);
		}

		/// <summary>
		/// Configuration error.
		/// </summary>
		/// <param name="errorMessage">The error message.</param>
		[
			Event(
				Events.VerboseMessaging,
				Message = "Verbose Message - {0}",
				Level = EventLevel.Verbose
			)
		]
		public void VerboseMessaging(string errorMessage)
		{
			WriteEvent(Events.VerboseMessaging, errorMessage);
		}

		/// <summary>
		/// Capture a PowerShell error.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="stackTrace">The stack trace.</param>
		/// <param name="file">The file.</param>
		/// <param name="lineNumber">The line number.</param>
		[
			Event(
				Events.PowerShellError,
				Message = "A Powershell error was raised. Error {0} with trace {1} on file {2}:{3}",
				Level = EventLevel.Warning
			)
		]
		public void PowerShellError(string message, string stackTrace, string file, int lineNumber)
		{
			WriteEvent(Events.PowerShellError, message, stackTrace, file, lineNumber);
		}

		/// <summary>
		/// Event Id constants.
		/// </summary>
		public static class Events
		{
			/// <summary>
			///	Received the request.
			/// </summary>
			public const int ReceivedRequest = 1000;

			/// <summary>
			///	Started the PowerShell script.
			/// </summary>
			public const int PowershellStart = 1001;

			/// <summary>
			///	Snap in exception occurred
			/// </summary>
			public const int SnapinException = 1002;

			/// <summary>
			/// The unhandled exception message code.
			/// </summary>
			public const int UnhandledException = 1003;

			/// <summary>
			/// The configuration error message code.
			/// </summary>
			public const int ConfigurationError = 1004;

			/// <summary>
			/// The start-up event
			/// </summary>
			public const int ApiStartup = 1005;

			/// <summary>
			/// The stop event
			/// </summary>
			public const int ApiStop = 1006;

			/// <summary>
			///	Started the PowerShell script.
			/// </summary>
			public const int PowershellStop = 1007;

			/// <summary>
			///	Started the PowerShell script.
			/// </summary>
			public const int VerboseMessaging = 1008;

			/// <summary>
			/// The PowerShell error event
			/// </summary>
			public const int PowerShellError = 1010;

			/// <summary>
			/// Invalid PowerShell output.
			/// </summary>
			public const int InvalidPowerShellOutput = 1011;

			/// <summary>
			///	Loading Module
			/// </summary>
			public const int LoadingModule = 1012;

			/// <summary>
			///	Module exception occurred
			/// </summary>
			public const int ModuleException = 1013;
		}
	}
}