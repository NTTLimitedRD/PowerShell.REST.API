using DynamicPowerShellApi.Jobs;
using DynamicPowerShellApi.Logging;
using Owin.Stats;

namespace DynamicPowerShellApi.Owin
{
	using Autofac;
	using Autofac.Integration.WebApi;

	using Configuration;
	using Controllers;
	using DynamicPowerShellApi;

	using Microsoft.Owin.Hosting;
	using Microsoft.Owin.Security;
	using Microsoft.Owin.Security.Jwt;
	using global::Owin;

	using Security;
	using System;
	using System.Security.Cryptography.X509Certificates;
	using System.Web.Http;
	using System.Web.Http.Controllers;
	using System.Web.Http.Dispatcher;

	/// <summary>
	/// The startup.
	/// </summary>
	public class Startup
	{
		/// <summary>
		/// This code configures Web API. The Startup class is specified as a type
		/// parameter in the WebApp.Start method.
		/// </summary>
		/// <param name="appBuilder">The application builder.</param>
		public void Configuration(IAppBuilder appBuilder)
		{
			// Configure Web API for self-host. 
			HttpConfiguration config = CreateConfiguration();

			// Construct the Autofac container
			IContainer container = BuildContainer();

			// Use autofac's dependency resolver, not the OWIN one
			config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

			// Wait for the initialization to complete (setup the socket)
			config.EnsureInitialized();

			// If the config file specifies authentication, load up the certificates and use the JWT middleware.
		    if (WebApiConfiguration.Instance.Authentication.Enabled)
		    {
		        X509Certificate2 cert = Certificate.ReadCertificate();

		        appBuilder.UseJwtBearerAuthentication(
		            new JwtBearerAuthenticationOptions
		            {
		                AllowedAudiences = new[]
		                {
			                WebApiConfiguration.Instance.Authentication.Audience
		                },
		                IssuerSecurityTokenProviders =
		                    new[]
		                    {
		                        new X509CertificateSecurityTokenProvider(cert.Subject, cert)
		                    },
		                AuthenticationType = "Bearer",
		                AuthenticationMode = AuthenticationMode.Active
		            });
		    }

			appBuilder.UseAutofacMiddleware(container);
		    appBuilder.UseWebApi(config);
			appBuilder.UseAutofacWebApi(config);
		}

		/// <summary>
		/// The build container.
		/// </summary>
		/// <returns>
		/// The <see cref="IContainer"/>.
		/// </returns>
		private IContainer BuildContainer()
		{
			ContainerBuilder builder = new ContainerBuilder();

			builder.RegisterApiControllers(typeof(GenericController).Assembly);

			builder.RegisterType<StatsProviderComponent>().SingleInstance();

			builder
				.RegisterType<JobListProvider>()
				.SingleInstance()
				.As<IJobListProvider>();

			builder.RegisterType<PowershellRunner>()
				.As<IRunner>()
				.InstancePerDependency();

			builder.RegisterType<CrashLogger>()
				.As<ICrashLogger>()
				.SingleInstance();

			return builder.Build();
		}

		/// <summary>
		/// The create configuration.
		/// </summary>
		/// <returns>
		/// The <see cref="HttpConfiguration"/>.
		/// </returns>
		private HttpConfiguration CreateConfiguration()
		{
			// Configure Web API for self-host. 
			HttpConfiguration config = new HttpConfiguration();

			config.Services.Replace(typeof(IHttpControllerSelector), new GenericControllerSelector(config));
			config.Services.Replace(typeof(IHttpActionSelector), new GenericActionSelector(config));

			config.Routes.MapHttpRoute(
				name: "DefaultApi",
				routeTemplate: "api/{controller}/{action}",
				defaults: new { category = "all", id = RouteParameter.Optional });

			return config;
		}

		/// <summary>
		/// The start.
		/// </summary>
		/// <returns>
		/// The <see cref="IDisposable"/>.
		/// </returns>
		public static IDisposable Start()
		{
			Uri baseAddress = WebApiConfiguration.Instance.HostAddress;

			// Start OWIN host 
			return WebApp.Start<Startup>(url: baseAddress.ToString());
		}
	}
}