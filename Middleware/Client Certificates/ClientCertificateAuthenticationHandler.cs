/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using System.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Thinktecture.IdentityModel.Owin
{
    public class ClientCertificateAuthenticationHandler : AuthenticationHandler<ClientCertificateAuthenticationOptions>
    {
        protected override Task<AuthenticationTicket> AuthenticateCoreAsync()
        {
            var cert = Context.Get<X509Certificate2>("ssl.ClientCertificate");

            if (cert == null)
            {
                return Task.FromResult<AuthenticationTicket>(null);
            }

            try
            {
                Options.Validator.Validate(cert);
            }
            catch (SecurityTokenValidationException)
            {
                return Task.FromResult<AuthenticationTicket>(null);
            }

            var identity = Identity.CreateFromCertificate(
                cert,
                Options.AuthenticationType,
                Options.CreateExtendedClaimSet);

            var ticket = new AuthenticationTicket(identity, new AuthenticationProperties());
            return Task.FromResult<AuthenticationTicket>(ticket);
        }
    }
}