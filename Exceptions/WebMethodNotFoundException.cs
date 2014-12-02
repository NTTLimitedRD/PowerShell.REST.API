using System;

namespace DynamicPowerShellApi.Exceptions
{
	/// <summary>
	/// The web method not found exception.
	/// </summary>
	[Serializable]
	class WebMethodNotFoundException : Exception
	{
		/// <summary>
		/// Initialises a new instance of the <see cref="WebMethodNotFoundException"/> class.
		/// </summary>
		/// <param name="message">
		/// The error message.
		/// </param>
		public WebMethodNotFoundException(string message)
			: base(message)
		{
		}
	}
}
