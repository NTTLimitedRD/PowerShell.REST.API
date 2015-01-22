﻿using DynamicPowerShellApi.Logging;

namespace DynamicPowerShellApi.Owin
{
	using System;
	using System.Security.Cryptography.X509Certificates;
	using System.Web.Http;
	using System.Web.Http.Controllers;
	using System.Web.Http.Dispatcher;

	using Autofac;
	using Autofac.Integration.WebApi;

	using DynamicPowerShellApi;
	using DynamicPowerShellApi.Configuration;
	using DynamicPowerShellApi.Controllers;
	using DynamicPowerShellApi.Security;

	using Microsoft.Owin.Hosting;
	using Microsoft.Owin.Security;
	using Microsoft.Owin.Security.Jwt;

	using global::Owin;

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

			config.DependencyResolver = new AutofacWebApiDependencyResolver(BuildContainer());
			
			config.EnsureInitialized();
			X509Certificate2 cert = Certificate.ReadCertificate();
			
			appBuilder.UseJwtBearerAuthentication(
				new JwtBearerAuthenticationOptions
					{
						AllowedAudiences = new[] { WebApiConfiguration.Instance.Authentication.Audience },
						IssuerSecurityTokenProviders =
							new[]
								{
									new X509CertificateSecurityTokenProvider(cert.Subject, cert)
								},
						AuthenticationType = "Bearer",
						AuthenticationMode = AuthenticationMode.Active
					});
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