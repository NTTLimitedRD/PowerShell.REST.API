/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System.IdentityModel.Selectors;
using System.Security.Cryptography.X509Certificates;
using Thinktecture.IdentityModel.Owin;

namespace Owin
{
    public static class ClientCertificateAuthenticationExtensions
    {
        public static IAppBuilder UseClientCertificateAuthentication(this IAppBuilder app, X509RevocationMode revocationMode = X509RevocationMode.Online, bool createExtendedClaims = false)
        {
            var policy = new X509ChainPolicy
            {
                RevocationMode = revocationMode
            };

            var validator = X509CertificateValidator.CreateChainTrustValidator(true, policy);

            var options = new ClientCertificateAuthenticationOptions
            {
                Validator = validator,
                CreateExtendedClaimSet = createExtendedClaims
            };

            return app.UseClientCertificateAuthentication(options);
        }

        public static IAppBuilder UseClientCertificateAuthentication(this IAppBuilder app, ClientCertificateAuthenticationOptions options)
        {
            app.Use<ClientCertificateAuthenticationMiddleware>(options);
            return app;
        }
    }
}