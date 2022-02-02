using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using WebApplication.Auth.Security;

namespace WebApplication.Auth.JwtBearerToken
{
    public class JwtAuthenticationService : IJwtAuthenticationService
    {
        private readonly ISecurityServices _securityService;
        public JwtAuthenticationService(ISecurityServices securityService)
        {
            _securityService = securityService;
        }
        public Dictionary<string, object> Authenticate(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            var identify = _securityService.Login(username, password);
            if (identify == null)
            {
                return null;
            }

            var claims = _securityService.GetClaims(identify);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.ASCII.GetBytes(AppConfiguration.KeySecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = AppConfiguration.Issuer,
                Audience = AppConfiguration.Audience,
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
            var expireIn = token.ValidTo - token.ValidFrom;
            return new Dictionary<string, object>
            {
                {"token_type", "bearer"},
                {"access_token", tokenHandler.WriteToken(token)},
                {"expire_in", Convert.ToInt32(expireIn.TotalSeconds)}
            };
        }
    }
}
