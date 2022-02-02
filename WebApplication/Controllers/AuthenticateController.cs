using Microsoft.AspNetCore.Mvc;
using WebApplication.Auth.JwtBearerToken;
using WebApplication.Dto.RequestDto.Login;

namespace WebApplication.Controllers
{
    [Route("auth")]
    public class AuthenticateController : ControllerBase
    {
        private readonly IJwtAuthenticationService _authentication;

        public AuthenticateController(IJwtAuthenticationService authentication)
        {
            _authentication = authentication;
        }

        [HttpPost]
        [Route("login")]
        public IActionResult Login([FromBody] LoginReqDto model)
        {
            var token = _authentication.Authenticate(model.Login, model.Password);
            if (token == null)
            {
                return Unauthorized();
            }
            return Ok(token);
        }
    }
}
