namespace DynamicPowerShellApi.IntegrationTests
{
	using System;
	using System.IdentityModel.Tokens;
	using System.Net;
	using System.Net.Http;
	using System.Security.Claims;

	using DynamicPowerShellApi.Owin;
	using DynamicPowerShellApi.Security;

	using Microsoft.Owin.Hosting;
	using Microsoft.VisualStudio.TestTools.UnitTesting;

	/// <summary>
	/// The generic controller tests.
	/// </summary>
	[TestClass]
	public class GenericControllerTests
	{
		/// <summary>
		/// The issue token.
		/// </summary>
		/// <returns>
		/// The <see cref="string"/>.
		/// </returns>
		private static string IssueToken()
		{
			JwtSecurityToken jwt = new JwtSecurityToken(
				issuer: "Dummy Cloud STS",
				audience: "http://aperture.identity/connectors",
				claims: new[]
							{
								// TODO: Add your claims here.
								new Claim("sub", "someone@foo.bar.com")
							},
				notBefore: DateTime.UtcNow,
				expires: DateTime.UtcNow.AddHours(1),
				signingCredentials: new X509SigningCredentials(Certificate.ReadCertificate())
				// signingCredentials: new HmacSigningCredentials(GetIssuerKey())
			);
			JwtSecurityTokenHandler jwtHandler = new JwtSecurityTokenHandler();

			return jwtHandler.WriteToken(jwt);
		}

		/// <summary>
		/// The process request run test script.
		/// </summary>
		[TestCategory("Integration Tests")]
		[TestMethod]
		public void ProcessRequest_RunTestScript()
		{
			using (WebApp.Start<Startup>("http://localhost:9000"))
			{
				var client = new HttpClient { BaseAddress = new Uri("http://localhost:9000") };
				client.SetBearerToken(IssueToken());
				var response = client.GetAsync("/api/Exchange/CreateMailbox?MailBoxSize=99").Result;

				Assert.AreNotEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Could not authenticate!");
				Assert.IsTrue(response.IsSuccessStatusCode, "Response was not successful.");
			}
		}
	}
}
