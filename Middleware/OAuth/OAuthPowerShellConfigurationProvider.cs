using Microsoft.Owin.Security.OAuth;
using System.Security.Claims;
using System.Threading.Tasks;
using Claim = System.Security.Claims.Claim;

namespace DynamicPowerShellAPI.Middleware.OAuth
{
	public class OAuthPowerShellConfigurationProvider : OAuthAuthorizationServerProvider
	{
		/// <summary>
		/// Validates the client authentication.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns>
		/// nothing.
		/// </returns>
		public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
		{
			context.Validated();
		}

		/// <summary>
		/// Called when a request to the Token endpoint arrives with a "grant_type" of "password". This occurs when the user has provided name and password
		///             credentials directly into the client application's user interface, and the client application is using those to acquire an "access_token" and 
		///             optional "refresh_token". If the web application supports the
		///             resource owner credentials grant type it must validate the context.Username and context.Password as appropriate. To issue an
		///             access token the context.Validated must be called with a new ticket containing the claims about the resource owner which should be associated
		///             with the access token. The application should take appropriate measures to ensure that the endpoint isn’t abused by malicious callers.
		///             The default behavior is to reject this grant type.
		///             See also http://tools.ietf.org/html/rfc6749#section-4.3.2
		/// </summary>
		/// <param name="context">The context of the event carries information in and results out.</param>
		/// <returns>
		/// Task to enable asynchronous execution
		/// </returns>
		public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
		{
			context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

			var identity = new ClaimsIdentity(context.Options.AuthenticationType);
			identity.AddClaim(new Claim("sub", context.UserName));
			identity.AddClaim(new Claim("role", "user"));

			context.Validated(identity);
		}
	}
}
