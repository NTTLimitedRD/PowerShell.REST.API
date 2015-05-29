namespace DynamicPowerShellApi
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Web.Http;
	using System.Web.Http.Controllers;

	using Controllers;

	/// <summary>
	/// An action selector to force our generic PowerShell invoker.
	/// </summary>
	public class GenericActionSelector
		: IHttpActionSelector
	{
		/// <summary>
		/// The current configuration from the http server.
		/// </summary>
		private readonly HttpConfiguration _currentConfiguration;

		/// <summary>
		/// Gets the action descriptor.
		/// </summary>
		private HttpActionDescriptor ActionDescriptor
		{
			get
			{
				return new ReflectedHttpActionDescriptor(
					new HttpControllerDescriptor(_currentConfiguration, "generic", typeof(GenericController)),
					typeof(GenericController).GetMethod("ProcessRequestAsync"));
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="GenericActionSelector" /> class.
		/// </summary>
		/// <param name="configuration">The configuration of the http channel.</param>
		public GenericActionSelector(HttpConfiguration configuration)
		{
			_currentConfiguration = configuration;
		}

		/// <summary>
		/// Selects the action for the controller.
		/// </summary>
		/// <param name="controllerContext">The context of the controller.</param>
		/// <returns>
		/// The action for the controller.
		/// </returns>
		public HttpActionDescriptor SelectAction(HttpControllerContext controllerContext)
		{
			if (controllerContext.Request.RequestUri.AbsolutePath == Constants.StatusUrlPath)
				return new ReflectedHttpActionDescriptor(
					new HttpControllerDescriptor(_currentConfiguration, "generic", typeof(GenericController)),
					typeof(GenericController).GetMethod("Status"));

			// Always give the same action
			return ActionDescriptor;
		}

		/// <summary>
		/// Returns a map, keyed by action string, of all <see cref="T:System.Web.Http.Controllers.HttpActionDescriptor" /> that the selector can select.  This is primarily called by <see cref="T:System.Web.Http.Description.IApiExplorer" /> to discover all the possible actions in the controller.
		/// </summary>
		/// <param name="controllerDescriptor">The controller descriptor.</param>
		/// <returns>
		/// A map of <see cref="T:System.Web.Http.Controllers.HttpActionDescriptor" /> that the selector can select, or null if the selector does not have a well-defined mapping of <see cref="T:System.Web.Http.Controllers.HttpActionDescriptor" />.
		/// </returns>
		public ILookup<string, HttpActionDescriptor> GetActionMapping(HttpControllerDescriptor controllerDescriptor)
		{
			// Exercised only by ASP.NET Web API’s API explorer feature
			
			List<HttpActionDescriptor> descriptors = new List<HttpActionDescriptor>();

			descriptors.Add(ActionDescriptor);

			ILookup<string, HttpActionDescriptor> result = descriptors.ToLookup(
				p => "generic",
				p => p);

			return result;
		}
	}
}