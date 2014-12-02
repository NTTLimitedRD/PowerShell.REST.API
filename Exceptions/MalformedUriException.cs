namespace DynamicPowerShellApi.Exceptions
{
	using System;

	/// <summary>
	/// The malformed URI exception.
	/// </summary>
	[Serializable]
	public class MalformedUriException : Exception
	{
		/// <summary>
		/// Initialises a new instance of the <see cref="MalformedUriException"/> class.
		/// </summary>
		/// <param name="message">
		/// The error message.
		/// </param>
		public MalformedUriException(string message)
			: base(message)
		{
		}
	}
}