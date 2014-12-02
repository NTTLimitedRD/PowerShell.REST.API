/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Thinktecture.IdentityModel.Owin
{
    public class RequireTlsMiddleware
    {
        readonly Func<IDictionary<string, object>, Task> _next;
        private readonly RequireTlsOptions _options;

        public RequireTlsMiddleware(Func<IDictionary<string, object>, Task> next, RequireTlsOptions options)
        {
            _next = next;
            _options = options;
        }

        public async Task Invoke(IDictionary<string, object> env)
        {
            var context = new OwinContext(env);

            if (context.Request.Uri.Scheme != Uri.UriSchemeHttps)
            {
                context.Response.StatusCode = 403;
                context.Response.ReasonPhrase = "SSL is required.";

                return;
            }

            if (_options.RequireClientCertificate)
            {
                var cert = context.Get<X509Certificate2>("ssl.ClientCertificate");
                if (cert == null)
                {
                    context.Response.StatusCode = 403;
                    context.Response.ReasonPhrase = "SSL client certificate is required.";

                    return;
                }
            }

            await _next(env);
        }
    }
}