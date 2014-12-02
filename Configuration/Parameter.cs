namespace DynamicPowerShellApi.Configuration
{
	using System.Configuration;

	/// <summary>
	/// The parameter element.
	/// </summary>
	public class Parameter : ConfigurationElement
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
		/// Gets the parameter type.
		/// </summary>
		[ConfigurationProperty("Type")]
		public string ParamType
		{
			get
			{
				return (string)this["Type"];
			}
		}
	}
}