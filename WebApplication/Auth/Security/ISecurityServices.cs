using System.Collections.Generic;
using System.Security.Claims;
using WebApplication.Auth.Security.Models;

namespace WebApplication.Auth.Security
{
    public interface ISecurityServices
    {
        IdentityInfo GetIdentityInfoByApplicationKey(string key);
        IdentityInfo GetIdentityInfoByToken(string token);
        List<Claim> GetClaims(IdentityInfo identity);
        IdentityInfo Login(string username, string password);

    }
}
