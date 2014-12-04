namespace DynamicPowerShellApi
{
	using System;
	using System.Collections.Generic;
	using System.Net.Http;
	using System.Web.Http;
	using System.Web.Http.Controllers;
	using System.Web.Http.Dispatcher;
	using Controllers;

	using DynamicPowerShellApi.Configuration;

	/// <summary>
	/// The generic controller selector.
	/// </summary>
	public class GenericControllerSelector : IHttpControllerSelector
	{
		/// <summary>
		/// The current configuration from the http server.
		/// </summary>
		private readonly HttpConfiguration _currentConfiguration;

		/// <summary>
		/// Gets the generic descriptor.
		/// </summary>
		private HttpControllerDescriptor GenericDescriptor
		{
			get
			{
				return 
				new HttpControllerDescriptor(
					this._currentConfiguration,
					"generic",
					typeof(GenericController));
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="GenericControllerSelector" /> class.
		/// </summary>
		/// <param name="configuration">The configuration of the http channel.</param>
		public GenericControllerSelector(HttpConfiguration configuration)
		{
			if (configuration == null)
				throw new ArgumentNullException("configuration", "Argument cannot be null.");

			this._currentConfiguration = configuration;
		}

		/// <summary>
		/// Selects a <see cref="T:System.Web.Http.Controllers.HttpControllerDescriptor" /> for the given <see cref="T:System.Net.Http.HttpRequestMessage" />.
		/// </summary>
		/// <param name="request">The request message.</param>
		/// <returns>
		/// An <see cref="T:System.Web.Http.Controllers.HttpControllerDescriptor" /> instance.
		/// </returns>
		public HttpControllerDescriptor SelectController(HttpRequestMessage request)
		{
			if (request == null)
				throw new ArgumentNullException("request", "Argument cannot be null.");

			Console.WriteLine("Selecting controller for request {0}", request.RequestUri);
			DynamicPowershellApiEvents.Raise.VerboseMessaging(String.Format("Received Request {0}", request.RequestUri));
			
			return this.GenericDescriptor;
		}

		/// <summary>
		/// Returns a map, keyed by controller string, of all <see cref="T:System.Web.Http.Controllers.HttpControllerDescriptor" /> that the selector can select.  This is primarily called by <see cref="T:System.Web.Http.Description.IApiExplorer" /> to discover all the possible controllers in the system.
		/// </summary>
		/// <returns>
		/// A map of all <see cref="T:System.Web.Http.Controllers.HttpControllerDescriptor" /> that the selector can select, or null if the selector does not have a well-defined mapping of <see cref="T:System.Web.Http.Controllers.HttpControllerDescriptor" />.
		/// </returns>
		public IDictionary<string, HttpControllerDescriptor> GetControllerMapping()
		{
			// Exercised only by ASP.NET Web API’s API explorer feature
			var dic = new Dictionary<string, HttpControllerDescriptor>();

			Console.WriteLine("Registering API endpoints.");

			foreach (WebApi api in WebApiConfiguration.Instance.Apis)
			{
				Console.WriteLine("Registering API {0}.", api.Name);
				dic.Add(api.Name, new HttpControllerDescriptor(this._currentConfiguration, api.Name, typeof(GenericController)));
			}

			dic.Add("generic", this.GenericDescriptor);

			return dic;
		}
	}
}