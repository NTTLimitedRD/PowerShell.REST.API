using System;

namespace DynamicPowerShellApi.Model
{
	/// <summary>
	/// Error Response.
	/// </summary>
	public class ErrorResponse
	{
		/// <summary>
		/// Gets or sets the message.
		/// </summary>
		/// <value>
		/// The message.
		/// </value>
		public string Message { get; set; }

		/// <summary>
		/// Gets a value indicating whether this <see cref="ErrorResponse"/> is success.
		/// </summary>
		/// <value>
		///   <c>true</c> if success; otherwise, <c>false</c>.
		/// </value>
		public bool Success
		{
			get { return false; }
		}

		/// <summary>
		/// Gets or sets the log file.
		/// </summary>
		/// <value>
		/// The log file.
		/// </value>
		public string LogFile { get; set; }

		/// <summary>
		/// Gets or sets the activity identifier.
		/// </summary>
		/// <value>
		/// The activity identifier.
		/// </value>
		public Guid ActivityId { get; set; }
	}
}
