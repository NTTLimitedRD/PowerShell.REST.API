namespace DynamicPowerShellApi.Configuration
{
	using System.Collections.Generic;
	using System.Configuration;

	/// <summary>
	/// The collection of API elements.
	/// </summary>
	public class WebApiCollection : ConfigurationElementCollection, IEnumerable<WebApi>
	{
		/// <summary>
		/// Creates a new element.
		/// </summary>
		/// <returns>
		/// The <see cref="ConfigurationElement"/>.
		/// </returns>
		protected override ConfigurationElement CreateNewElement()
		{
			return new WebApi();
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
			return ((WebApi)element).Name;
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
		/// Gets the element name.
		/// </summary>
		protected override string ElementName
		{
			get
			{
				return "WebApi";
			}
		}

		/// <summary>
		/// Gets the <see cref="WebApi"/> object using the index location.
		/// </summary>
		/// <param name="index">
		/// The index.
		/// </param>
		/// <returns>
		/// The <see cref="WebApi"/>.
		/// </returns>
		public WebApi this[int index]
		{
			get
			{
				return (WebApi)this.BaseGet(index);
			}
		}

		/// <summary>
		/// Gets the <see cref="WebApi"/> object using the key location.
		/// </summary>
		/// <param name="key">
		/// The key.
		/// </param>
		/// <returns>
		/// The <see cref="WebApi"/>.
		/// </returns>
		public new WebApi this[string key]
		{
			get
			{
				return (WebApi)this.BaseGet(key);
			}
		}

		/// <summary>
		/// Gets the enumerator.
		/// </summary>
		/// <returns>
		/// The <see cref="IEnumerator"/>.
		/// </returns>
		public new IEnumerator<WebApi> GetEnumerator()
		{
			int count = Count;
			for (int i = 0; i < count; i++)
			{
				yield return BaseGet(i) as WebApi;
			}
		}
	}
}