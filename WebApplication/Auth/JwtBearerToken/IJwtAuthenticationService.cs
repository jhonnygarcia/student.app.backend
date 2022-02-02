using System.Collections.Generic;

namespace WebApplication.Auth.JwtBearerToken
{
    public interface IJwtAuthenticationService
    {
        Dictionary<string, object> Authenticate(string username, string password);
    }
}
