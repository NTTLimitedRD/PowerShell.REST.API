namespace DynamicPowerShellApi.Security
{
	using System.Security.Cryptography.X509Certificates;

	using DynamicPowerShellApi.Configuration;
	using DynamicPowerShellApi.Exceptions;

	/// <summary>
	/// The certificate.
	/// </summary>
	public static class Certificate
	{
		/// <summary>
		/// The read certificate.
		/// </summary>
		/// <returns>
		/// The <see cref="X509Certificate2"/>.
		/// </returns>
		/// <exception cref="CertificateNotFoundException">
		/// </exception>
		public static X509Certificate2 ReadCertificate()
		{
			var auth = WebApiConfiguration.Instance.Authentication;
			var store = new X509Store(auth.StoreName, auth.StoreLocation);
			store.Open(OpenFlags.OpenExistingOnly | OpenFlags.ReadOnly);

			var certificates = store.Certificates.Find(X509FindType.FindByThumbprint, auth.Thumbprint, true);
			store.Close();

			if (certificates.Count != 1)
			{
				throw new CertificateNotFoundException(
					string.Format(
						"Certificate not found in Store Name {0}, Location {1} and Thumbprint {2}",
						auth.StoreName,
						auth.StoreLocation,
						auth.Thumbprint));
			}
			return certificates[0];
		}
	}
}