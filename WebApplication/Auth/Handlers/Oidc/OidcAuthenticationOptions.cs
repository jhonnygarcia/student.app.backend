using Microsoft.AspNetCore.Authentication;

namespace WebApplication.Auth.Handlers.Oidc
{
    public class OidcAuthenticationOptions : AuthenticationSchemeOptions
    {
        public const string DEFAULT_SCHEME = "Facturation Bearer Token";
        public string Scheme => DEFAULT_SCHEME;
        public string AuthenticationType = DEFAULT_SCHEME;
    }
}
