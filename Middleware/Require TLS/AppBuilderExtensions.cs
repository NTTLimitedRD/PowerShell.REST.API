/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using Thinktecture.IdentityModel.Owin;

namespace Owin
{
	using System;

	public static class AppBuilderExtensions
    {
        public static IAppBuilder UseRequireTls(this IAppBuilder app, bool requireClientCertificate = false)
        {
			if (app == null) 
				throw new ArgumentNullException("app", "Argument cannot be null");

            app.Use(typeof(RequireTlsMiddleware), new RequireTlsOptions { RequireClientCertificate = requireClientCertificate });
            return app;
        }
    }
}
