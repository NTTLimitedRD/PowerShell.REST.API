using DynamicPowerShellApi.Model;

namespace DynamicPowerShellApi.Tests
{
	using System;
	using System.Collections.Generic;
	using System.Net.Http;

	using Autofac;

	using Configuration;
	using Controllers;
	using Exceptions;

	using Microsoft.VisualStudio.TestTools.UnitTesting;

	using Moq;

	using Newtonsoft.Json.Linq;

	/// <summary>
	/// The generic controller tests.
	/// </summary>
	[TestClass]
	public class GenericControllerTests
	{
		/// <summary>
		/// The create container.
		/// </summary>
		/// <param name="cb">
		/// The container builder.
		/// </param>
		/// <returns>
		/// The <see cref="IContainer"/>.
		/// </returns>
		IContainer createContainer(Action<ContainerBuilder> cb)
		{
			var builder = new ContainerBuilder();
			builder.RegisterType<GenericController>().InstancePerDependency();

			builder.RegisterType<PowershellRunner>().As<IRunner>().InstancePerDependency();

			cb.Invoke(builder);
			return builder.Build();
		}

		/// <summary>
		/// The process request async successfully.
		/// </summary>
		[TestMethod]
		public void ProcessRequestAsyncSuccessfully()
		{
			JObject returnValue = new JObject { { "Name", "TestName" } };

			Uri requestUri =
				new Uri(
					string.Format(
						"http://bla.com/api/{0}/{1}?{2}=TestValue1",
						WebApiConfiguration.Instance.Apis[0].Name,
						WebApiConfiguration.Instance.Apis[0].WebMethods[0].Name,
						WebApiConfiguration.Instance.Apis[0].WebMethods[0].Parameters[0].Name));

			Mock<IRunner> runnerMock = new Mock<IRunner>(MockBehavior.Strict);
			runnerMock.Setup(
				m => m.ExecuteAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IList<KeyValuePair<string, string>>>(), It.IsAny<bool>()))
				.ReturnsAsync(new PowershellReturn
				{
					PowerShellReturnedValidData = true,
					ActualPowerShellData = returnValue.ToString()
				});

			var ioc = createContainer(cb => cb.RegisterInstance(runnerMock.Object));

			var genericController = ioc.Resolve<GenericController>();
			genericController.Request = new HttpRequestMessage(HttpMethod.Get, requestUri);
			var res = genericController.ProcessRequestAsync().Result;

			Assert.AreEqual(
				returnValue.ToString(),
				res.Content.ReadAsStringAsync().Result,
				"Return value from process request is incorrect");
		}

		/// <summary>
		/// The execute async failed with too little segments in uri.
		/// </summary>
		/// <exception cref="Exception">
		/// </exception>
		[TestMethod]
		[ExpectedException(typeof(MalformedUriException))]
		public void ProcessRequestAsync_TooLittleSegmentsInUri()
		{
			Uri requestUri = new Uri("http://bla.com/api/api1?param1=TestValue1");
			var ioc = createContainer(cb => { });

			var controller = ioc.Resolve<GenericController>();
			controller.Request = new HttpRequestMessage(HttpMethod.Get, requestUri);
			try
			{
				controller.ProcessRequestAsync().Wait();
			}
			catch (AggregateException ae)
			{
				if (ae.InnerExceptions.Count == 1)
				{
					throw ae.InnerException;
				}
			}
		}
	}
}