namespace DynamicPowerShellApi.Configuration
{
	using System.Configuration;
	using System.Security.Cryptography.X509Certificates;

	/// <summary>
	/// The authentication.
	/// </summary>
	public class Authentication : ConfigurationElement
	{
		/// <summary>
		/// Gets the store name.
		/// </summary>
		[ConfigurationProperty("StoreName", IsRequired = true)]
		public StoreName StoreName
		{
			get
			{
				return (StoreName)this["StoreName"];
			}
		}

		/// <summary>
		/// Gets the store location.
		/// </summary>
		[ConfigurationProperty("StoreLocation", IsRequired = true)]
		public StoreLocation StoreLocation
		{
			get
			{
				return (StoreLocation)this["StoreLocation"];
			}
		}

		/// <summary>
		/// Gets the thumbprint.
		/// </summary>
		[ConfigurationProperty("Thumbprint", IsRequired = true)]
		public string Thumbprint
		{
			get
			{
				return (string)this["Thumbprint"];
			}
		}

		/// <summary>
		/// Gets the Audience.
		/// </summary>
		[ConfigurationProperty("Audience", IsRequired = true)]
		public string Audience
		{
			get
			{
				return (string)this["Audience"];
			}
		}

        /// <summary>
        /// Enable authentication.
        /// </summary>
        [ConfigurationProperty("Enabled", IsRequired = false,DefaultValue = true)]
        public bool Enabled
        {
            get
            {
                return (bool)this["Enabled"];
            }
        }
	}
}