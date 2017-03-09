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

		/// <summary>
		/// Determines whether the parameter is optional.
		/// </summary>
		[ConfigurationProperty("IsOptional", DefaultValue = false)]
		public bool IsOptional
		{
			get
			{
				return (bool)this["IsOptional"];
			}
		}

		/// <summary>
		/// Determines whether the parameter is required (configured via <see cref="IsOptional"/>).
		/// </summary>
		public bool IsRequired => !IsOptional;
	}
}