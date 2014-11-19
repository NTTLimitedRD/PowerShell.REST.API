namespace DynamicPowerShellApi.Exceptions
{
	using System;

	/// <summary>
	/// The certificate not found exception.
	/// </summary>
	[Serializable]
	public class CertificateNotFoundException : Exception
	{
		/// <summary>
		/// Initialises a new instance of the <see cref="CertificateNotFoundException"/> class.
		/// </summary>
		/// <param name="message">
		/// The message.
		/// </param>
		public CertificateNotFoundException(string message)
			: base(message)
		{
		}
	}
}