using System;
using Microsoft.AspNetCore.Authentication;
using WebApplication.Auth.Handlers.ApiKey;
using WebApplication.Auth.Handlers.Oidc;

namespace WebApplication.Auth
{
    public static class AuthenticationBuilderExtensions
    {
        public static AuthenticationBuilder AddApiKeySupport(this AuthenticationBuilder authenticationBuilder, Action<ApiKeyAuthenticationOptions> options)
        {
            return authenticationBuilder.AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(ApiKeyAuthenticationOptions.DEFAULT_SCHEME, options);
        }
        public static AuthenticationBuilder AddOidcBearerTokenSupport(this AuthenticationBuilder authenticationBuilder, Action<OidcAuthenticationOptions> options)
        {
            return authenticationBuilder.AddScheme<OidcAuthenticationOptions, OidcAuthenticationHandler>(OidcAuthenticationOptions.DEFAULT_SCHEME, options);
        }
    }
}
