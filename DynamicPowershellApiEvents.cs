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
		/// Loading SnapIn
		/// </summary>
		/// <param name="snapin">
		/// SnapIn name.
		/// </param>
		[
			Event(
				Events.LoadingSnapIn,
				Message = "Loading {0} SnapIn.",
				Level = EventLevel.Informational
				)
		]
		public void LoadingSnapIn(string snapin)
		{
			WriteEvent(Events.LoadingSnapIn, snapin);
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
		/// The snap in exception log.
		/// </summary>
		/// <param name="errorMessage">
		/// The error message from the <see cref="PSSnapInException"/> object.
		/// </param>
		[
			Event(
				Events.SnapinException,
				Message = "Snapin exception: {0}",
				Level = EventLevel.Error
				)
		]
		public void SnapinException(string errorMessage)
		{
			WriteEvent(Events.SnapinException, errorMessage);
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
		/// PowerShell script execution exception.
		/// </summary>
		/// <param name="errorMessage">
		/// The error message.
		/// </param>
		[
			Event(
				Events.ScriptExecutionException,
				Message = " PowerShell script execution exception: {0}",
				Level = EventLevel.Error
				)
		]
		public void ScriptExecutionException(string errorMessage)
		{
			WriteEvent(Events.ScriptExecutionException, errorMessage);
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
			///	Script Execution Exception
			/// </summary>
			public const int ScriptExecutionException = 1003;

			/// <summary>
			///	Loading SnapIn
			/// </summary>
			public const int LoadingSnapIn = 1004;

			/// <summary>
			///	Loading Module
			/// </summary>
			public const int LoadingModule = 1005;

			/// <summary>
			///	Module exception occurred
			/// </summary>
			public const int ModuleException = 1006;
		}
	}
}
