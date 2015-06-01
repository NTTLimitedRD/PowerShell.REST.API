using System.Configuration;

namespace DynamicPowerShellApi.Configuration
{
	/// <summary>	A job store configuration. </summary>
	/// <remarks>	Anthony, 6/1/2015. </remarks>
	/// <seealso cref="T:System.Configuration.ConfigurationElement"/>
	public class JobStoreConfiguration
		: ConfigurationElement
	{
		/// <summary>	Gets the full pathname of the job directory. </summary>
		/// <value>	The full pathname of the job file. </value>
		[ConfigurationProperty("JobStorePath", IsRequired = true)]
		public string JobStorePath
		{
			get
			{
				return (string)this["JobStorePath"];
			}
		}
	}
}
