namespace DynamicPowerShellApi.Configuration
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Configuration;
	using System.Linq;

	/// <summary>
	/// The parameter element collection.
	/// </summary>
	public class ParameterCollection : ConfigurationElementCollection, IEnumerable<Parameter>
	{
		/// <summary>
		/// Creates a new element.
		/// </summary>
		/// <returns>
		/// The <see cref="ConfigurationElement"/>.
		/// </returns>
		protected override ConfigurationElement CreateNewElement()
		{
			return new Parameter();
		}

		/// <summary>
		/// Get an element key.
		/// </summary>
		/// <param name="element">
		/// The element.
		/// </param>
		/// <returns>
		/// The <see cref="object"/>.
		/// </returns>
		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((Parameter)element).Name;
		}

		/// <summary>
		/// Gets the element name.
		/// </summary>
		protected override string ElementName
		{
			get
			{
				return "Parameter";
			}
		}

		/// <summary>
		/// Gets the collection type.
		/// </summary>
		public override ConfigurationElementCollectionType CollectionType
		{
			get
			{
				return ConfigurationElementCollectionType.BasicMap;
			}
		}

		/// <summary>
		/// Gets the <see cref="Parameter"/> object using the index.
		/// </summary>
		/// <param name="index">
		/// The index.
		/// </param>
		/// <returns>
		/// The <see cref="Parameter"/>.
		/// </returns>
		public Parameter this[int index]
		{
			get
			{
				return this.BaseGet(index) as Parameter;
			}
		}

		/// <summary>
		/// Gets the <see cref="Parameter"/> object using the key.
		/// </summary>
		/// <param name="key">
		/// The key.
		/// </param>
		/// <returns>
		/// The <see cref="Parameter"/>.
		/// </returns>
		public new Parameter this[string key]
		{
			get
			{
				return this.BaseGet(key) as Parameter;
			}
		}

		/// <summary>
		/// The get enumerator.
		/// </summary>
		/// <returns>
		/// The <see cref="IEnumerator"/>.
		/// </returns>
		public new IEnumerator<Parameter> GetEnumerator()
		{
			int count = Count;
			for (int i = 0; i < count; i++)
			{
				yield return BaseGet(i) as Parameter;
			}
		}
	}
}