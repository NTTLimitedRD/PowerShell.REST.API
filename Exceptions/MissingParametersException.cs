namespace DynamicPowerShellApi.Exceptions
{
	using System;

	/// <summary>
	/// The missing parameters exception.
	/// </summary>
	[Serializable]
	class MissingParametersException : Exception
	{
		/// <summary>
		/// Initialises a new instance of the <see cref="MissingParametersException"/> class.
		/// </summary>
		/// <param name="message">
		/// The error message.
		/// </param>
		public MissingParametersException(string message)
			: base(message)
		{
		}
	}
}
