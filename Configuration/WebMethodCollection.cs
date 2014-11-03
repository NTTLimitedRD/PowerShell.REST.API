namespace DynamicPowerShellApi.Configuration
{
	using System.Collections.Generic;
	using System.Configuration;

	/// <summary>
	/// The web method collection.
	/// </summary>
	public class WebMethodCollection : ConfigurationElementCollection, IEnumerable<WebMethod>
	{
		/// <summary>
		/// Creates a new element.
		/// </summary>
		/// <returns>
		/// The <see cref="ConfigurationElement"/>.
		/// </returns>
		protected override ConfigurationElement CreateNewElement()
		{
			return new WebMethod();
		}

		/// <summary>
		/// Gets an element key.
		/// </summary>
		/// <param name="element">
		/// The element.
		/// </param>
		/// <returns>
		/// The <see cref="object"/>.
		/// </returns>
		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((WebMethod)element).Name;
		}

		/// <summary>
		/// Gets the element name.
		/// </summary>
		protected override string ElementName
		{
			get
			{
				return "WebMethod";
			}
		}

		/// <summary>
		/// Is element name?
		/// </summary>
		/// <param name="elementName">
		/// The element name.
		/// </param>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		protected override bool IsElementName(string elementName)
		{
			return !string.IsNullOrEmpty(elementName) && elementName == "WebMethod";
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
		/// Gets <see cref="WebMethod"/> object using the index.
		/// </summary>
		/// <param name="index">
		/// The index.
		/// </param>
		/// <returns>
		/// The <see cref="WebMethod"/>.
		/// </returns>
		public WebMethod this[int index]
		{
			get
			{
				return this.BaseGet(index) as WebMethod;
			}
		}

		/// <summary>
		/// Gets <see cref="WebMethod"/> object using the key.
		/// </summary>
		/// <param name="key">
		/// The key.
		/// </param>
		/// <returns>
		/// The <see cref="WebMethod"/>.
		/// </returns>
		public new WebMethod this[string key]
		{
			get
			{
				return this.BaseGet(key) as WebMethod;
			}
		}

		/// <summary>
		/// Gets the enumerator.
		/// </summary>
		/// <returns>
		/// The <see cref="IEnumerator"/>.
		/// </returns>
		public new IEnumerator<WebMethod> GetEnumerator()
		{
			int count = Count;
			for (int i = 0; i < count; i++)
			{
				yield return BaseGet(i) as WebMethod;
			}
		}
	}
}