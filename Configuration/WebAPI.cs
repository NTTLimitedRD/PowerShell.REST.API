namespace DynamicPowerShellApi.Configuration
{
	using System.Configuration;

	/// <summary>
	/// The web api element.
	/// </summary>
	public class WebApi : ConfigurationElement
	{
		/// <summary>
		/// Gets the name.
		/// </summary>
		[ConfigurationProperty("Name", IsKey = true)]
		public string Name
		{
			get
			{
				return (string)this["Name"];
			}
		}

		/// <summary>
		/// Gets the web methods.
		/// </summary>
		[ConfigurationProperty("WebMethods")]
		public WebMethodCollection WebMethods
		{
			get
			{
				return (WebMethodCollection)this["WebMethods"];
			}
		}
	}
}