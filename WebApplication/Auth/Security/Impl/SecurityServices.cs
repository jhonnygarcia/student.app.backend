using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WebApplication.Auth.Security.Models;

namespace WebApplication.Auth.Security.Impl
{
    public class SecurityServices : ISecurityServices
    {
        private readonly SecurityConfigurationModel _configuration;
        private readonly ILogger _logger;
        public SecurityServices(IOptions<SecurityConfigurationModel> configuration,
            ILogger<SecurityServices> logger)
        {
            _configuration = configuration.Value;
            _logger = logger;
        }
        public IdentityInfo GetIdentityInfoByApplicationKey(string key)
        {
            var authorizedApiKeys = _configuration.AuthorizedApiKeys;

            var apiKey = authorizedApiKeys?.FirstOrDefault(a => a.ApiKey == key);
            if (apiKey == null)
            {
                return null;
            }

            return new IdentityInfo
            {
                Id = apiKey.ApiKey,
                Name = apiKey.Name
            };
        }
        public IdentityInfo GetIdentityInfoByToken(string token)
        {
            var identityInfo = GetUserIdentityInfoByToken(token);
            return identityInfo;
        }
        public List<Claim> GetClaims(IdentityInfo identity)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, identity.Id)
            };

            if (!string.IsNullOrEmpty(identity.Login))
            {
                claims.Add(new Claim(ClaimTypes.Upn, identity.Login));
            }
            if (!string.IsNullOrEmpty(identity.Name))
            {
                claims.Add(new Claim(ClaimTypes.Name, identity.Name));
            }
            return claims;
        }
        public IdentityInfo Login(string username, string password)
        {
            var users = _configuration.BasicAuthentication;
            var first = users.FirstOrDefault(u => u.Login.Equals(username, StringComparison.OrdinalIgnoreCase)
                                                  && u.Password == password);

            if (first == null)
            {
                _logger.LogDebug("El usuario o la contrase no son correctas");
                return null;
            }

            return new IdentityInfo
            {
                Login = first.Name,
                Id = first.Id,
                Name = first.Name,
            };
        }

        private IdentityInfo GetUserIdentityInfoByToken(string token)
        {
            try
            {
                var claims = GetClaimsFromToken(token);
                if (claims == null)
                {
                    return null;
                }

                var claimsIdentities = claims
                    .Select(claim =>
                    {
                        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.TryGetValue(claim.Type, out var claimType);
                        if (string.IsNullOrWhiteSpace(claimType))
                            return new ClaimsIdentity(new[]
                            {
                                new Claim(claim.Type, claim.Value)
                            });

                        return new ClaimsIdentity(new[]
                        {
                            new Claim(claimType, claim.Value)
                        });
                    })
                    .ToArray();

                var claimsPrincipal = new ClaimsPrincipal(claimsIdentities);
                var identityInfo = new IdentityInfo
                {
                    Id = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                    Name = claimsPrincipal.FindFirst(ClaimTypes.Name)?.Value,
                    Login = claimsPrincipal.FindFirst(ClaimTypes.Upn)?.Value
                };

                return identityInfo;
            }
            catch (Exception exception)
            {
                _logger.LogDebug(exception, "El token enviado esta corrupto");
                return null;
            }
        }
        private IEnumerable<Claim> GetClaimsFromToken(string authToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateLifetime = false,
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidIssuer = AppConfiguration.Issuer,
                ValidAudience = AppConfiguration.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AppConfiguration.KeySecret))
            };
            var principal = handler.ValidateToken(authToken,
                validationParameters, out var validatedToken);

            return validatedToken.ValidTo < DateTime.UtcNow ? null : principal.Claims;
        }
    }
}
