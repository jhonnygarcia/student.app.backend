using Microsoft.AspNetCore.Authentication;

namespace WebApplication.Auth.Handlers.ApiKey
{
    public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
    {
        public const string DEFAULT_SCHEME = "API Key";
        public string Scheme => DEFAULT_SCHEME;
        public string AuthenticationType = DEFAULT_SCHEME;
    }
}
