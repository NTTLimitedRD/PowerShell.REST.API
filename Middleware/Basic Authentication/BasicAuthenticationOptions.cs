/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * contributed by Pedro Felix
 * see license.txt
 */

using Microsoft.Owin.Security;

namespace Thinktecture.IdentityModel.Owin
{
    public class BasicAuthenticationOptions : AuthenticationOptions
    {
        public BasicAuthenticationMiddleware.CredentialValidationFunction CredentialValidationFunction { get; private set; }
        public string Realm { get; private set; }

        public BasicAuthenticationOptions(string realm, BasicAuthenticationMiddleware.CredentialValidationFunction validationFunction)
            : base("Basic")
        {
            Realm = realm;
            CredentialValidationFunction = validationFunction;
        }
    }
}