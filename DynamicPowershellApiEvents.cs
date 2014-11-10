namespace DynamicPowerShellApi
{
	using System;
	using System.Management.Automation.Runspaces;

	using DD.Cloud.Aperture.Platform.Diagnostics;

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
				ApertureCorrelationManager.SynchronizeEventSourceActivityIds();

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
		/// Execute the PowerShell script.
		/// </summary>
		/// <param name="powershellPath">
		/// The PowerShell path.
		/// </param>
		[
			Event(
				Events.PowershellStart, 
				Message = "Started execution of powershell script file {0}", 
				Level = EventLevel.Informational
				)
		]
		public void ExecutingPowerShellScript(string powershellPath)
		{
			WriteEvent(Events.PowershellStart, powershellPath);
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
				Message = "Started execution of powershell script file {0}",
				Level = EventLevel.Error
				)
		]
		public void SnapinException(string errorMessage)
		{
			WriteEvent(Events.SnapinException, errorMessage);
		}


		/// <summary>
		/// Unhandled Exception message
		/// </summary>
		/// <param name="errorMessage">The error message.</param>
		[
			Event(
				Events.UnhandledException,
				Message = "Unhandled exception in service {0}",
				Level = EventLevel.Error
				)
		]
		public void UnhandledException(string errorMessage)
		{
			WriteEvent(Events.UnhandledException, errorMessage);
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
		///		Event Id constants.
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
		}
	}
}
