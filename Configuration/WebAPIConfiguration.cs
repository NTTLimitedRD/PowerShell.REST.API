namespace DynamicPowerShellApi.Configuration
{
	using System;
	using System.Configuration;

	/// <summary>
	/// The web api configuration.
	/// </summary>
	public class WebApiConfiguration : ConfigurationSection
	{
		/// <summary>
		/// The singleton object of the web API configuration.
		/// </summary>
		private static readonly Lazy<WebApiConfiguration> LazyConfiguration =
			new Lazy<WebApiConfiguration>(() => (WebApiConfiguration)ConfigurationManager.GetSection("WebApiConfiguration"));

		/// <summary>
		/// Prevents a default instance of the <see cref="WebApiConfiguration"/> class from being created.
		/// </summary>
		private WebApiConfiguration()
		{
		}

		/// <summary>
		/// Gets the singleton instance of <see cref="WebApiConfiguration"/>.
		/// </summary>
		public static WebApiConfiguration Instance
		{
			get
			{
				return LazyConfiguration.Value;
			}
		}

		/// <summary>
		/// Gets the APIs.
		/// </summary>
		[ConfigurationProperty("Apis")]
		public WebApiCollection Apis
		{
			get
			{
				return (WebApiCollection)this["Apis"];
			}
		}

		/// <summary>
		/// Gets the host address.
		/// </summary>
		[ConfigurationProperty("HostAddress", IsRequired = true)]
		public Uri HostAddress
		{
			get
			{
				return (Uri)this["HostAddress"];
			}
		}

		/// <summary>
		/// Gets the authentication.
		/// </summary>
		[ConfigurationProperty("Authentication", IsRequired = true)]
		public Authentication Authentication
		{
			get
			{
				return (Authentication)this["Authentication"];
			}
		}
	}
}