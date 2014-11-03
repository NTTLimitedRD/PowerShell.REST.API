/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using Microsoft.Owin;
using Microsoft.Owin.Security.Infrastructure;

namespace Thinktecture.IdentityModel.Owin
{
    public class ClientCertificateAuthenticationMiddleware : AuthenticationMiddleware<ClientCertificateAuthenticationOptions>
    {
        public ClientCertificateAuthenticationMiddleware(OwinMiddleware next, ClientCertificateAuthenticationOptions options)
            : base(next, options)
        { }

        protected override AuthenticationHandler<ClientCertificateAuthenticationOptions> CreateHandler()
        {
            return new ClientCertificateAuthenticationHandler();
        }
    }
}