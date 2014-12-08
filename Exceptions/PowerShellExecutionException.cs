using System;
using System.Collections.Generic;

namespace DynamicPowerShellApi.Exceptions
{
	/// <summary>
	/// PowerShell execution exception.
	/// </summary>
	public class PowerShellExecutionException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PowerShellExecutionException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		public PowerShellExecutionException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Gets or sets the log time.
		/// </summary>
		/// <value>
		/// The log time.
		/// </value>
		public DateTime LogTime { get; set; }

		/// <summary>
		/// Gets or sets the exceptions.
		/// </summary>
		/// <value>
		/// The exceptions.
		/// </value>
		public List<PowerShellException> Exceptions { get; set; }
	}
}
