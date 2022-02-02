using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebApplication.Auth.Security;

namespace WebApplication.Auth.Handlers.ApiKey
{
    public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
    {
        private const string PROBLEM_DETAILS_CONTENT_TYPE = "application/problem+json";
        private const string API_KEY_HEADER_NAME = "X-Api-Key";

        private readonly ISecurityServices _securityService;

        public ApiKeyAuthenticationHandler(
            IOptionsMonitor<ApiKeyAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            ISecurityServices securityService) : base(options, logger, encoder, clock)
        {
            _securityService = securityService;
        }

#pragma warning disable 1998
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
#pragma warning restore 1998
        {
            if (!Request.Headers.TryGetValue(API_KEY_HEADER_NAME, out var apiKeyHeaderValues))
            {
                return AuthenticateResult.Fail("No API Key Header");
            }

            var providedApiKey = apiKeyHeaderValues.FirstOrDefault();

            if (apiKeyHeaderValues.Count == 0 || string.IsNullOrWhiteSpace(providedApiKey))
            {
                return AuthenticateResult.Fail("No API Key Value");
            }

            var appIdentity = _securityService.GetIdentityInfoByApplicationKey(providedApiKey);
            if (appIdentity == null)
            {
                return AuthenticateResult.Fail("Invalid API Key");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, appIdentity.Id),
                new Claim(ClaimTypes.Name, appIdentity.Name)
            };

            var identity = new ClaimsIdentity(claims, Options.AuthenticationType);
            var identities = new List<ClaimsIdentity> { identity };
            var principal = new ClaimsPrincipal(identities);
            var ticket = new AuthenticationTicket(principal, Options.Scheme);

            return AuthenticateResult.Success(ticket);
        }

        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.StatusCode = 401;
            Response.ContentType = PROBLEM_DETAILS_CONTENT_TYPE;
            var problemDetails = new UnauthorizedProblemDetails();

            await Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
        }

        protected override async Task HandleForbiddenAsync(AuthenticationProperties properties)
        {
            Response.StatusCode = 403;
            Response.ContentType = PROBLEM_DETAILS_CONTENT_TYPE;
            var problemDetails = new ForbiddenProblemDetails();

            await Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
        }
    }
}
