using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebApplication.Auth.Security;

namespace WebApplication.Auth.Handlers.Oidc
{
    public class OidcAuthenticationHandler : AuthenticationHandler<OidcAuthenticationOptions>
    {
        private const string PROBLEM_DETAILS_CONTENT_TYPE = "application/problem+json";
        private readonly ISecurityServices _securityService;

        public OidcAuthenticationHandler(
            ISecurityServices securityService,
            IOptionsMonitor<OidcAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock) : base(options, logger, encoder, clock)
        {
            _securityService = securityService;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var accessToken = GetAccessToken();
            if (string.IsNullOrEmpty(accessToken))
            {
                return Task.FromResult(AuthenticateResult.Fail("No Token"));
            }

            var userIdentity = _securityService.GetIdentityInfoByToken(accessToken);
            if (userIdentity == null)
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid Token"));
            }

            var claims = _securityService.GetClaims(userIdentity);

            var identity = new ClaimsIdentity(claims, Options.AuthenticationType);
            var identities = new List<ClaimsIdentity> { identity };
            var principal = new ClaimsPrincipal(identities);
            var ticket = new AuthenticationTicket(principal, Options.Scheme);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }

        protected virtual string GetAccessToken()
        {
            var authHeader = Request.Headers["Authorization"];
            if (authHeader.Count != 1)
            {
                return null;
            }

            var pair = authHeader[0].Split(' ');
            if (pair.Length != 2)
            {
                return null;
            }

            return pair[1];
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
