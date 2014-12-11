namespace DynamicPowerShellApi.Exceptions
{
	/// <summary>
	/// Exception details from PowerShell.
	/// </summary>
	public class PowerShellException
	{
		/// <summary>
		/// Gets or sets the name of the script.
		/// </summary>
		/// <value>
		/// The name of the script.
		/// </value>
		public string ScriptName { get; set; }

		/// <summary>
		/// Gets or sets the error message.
		/// </summary>
		/// <value>
		/// The error message.
		/// </value>
		public string ErrorMessage { get; set; }

		/// <summary>
		/// Gets or sets the line number.
		/// </summary>
		/// <value>
		/// The line number.
		/// </value>
		public int LineNumber { get; set; }

		/// <summary>
		/// Gets or sets the stack trace.
		/// </summary>
		/// <value>
		/// The stack trace.
		/// </value>
		public string StackTrace { get; set; }
	}
}