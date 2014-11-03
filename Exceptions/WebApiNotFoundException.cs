using System;

namespace DynamicPowerShellApi.Exceptions
{
	/// <summary>
	/// The web api not found exception.
	/// </summary>
	[Serializable]
	public class WebApiNotFoundException : Exception
	{
		/// <summary>
		/// Initialises a new instance of the <see cref="WebApiNotFoundException"/> class.
		/// </summary>
		/// <param name="message">
		/// The error message.
		/// </param>
		public WebApiNotFoundException(string message)
			: base(message)
		{
		}
	}
}
