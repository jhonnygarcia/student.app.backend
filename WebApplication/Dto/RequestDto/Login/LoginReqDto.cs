using System.Text.Json.Serialization;

namespace WebApplication.Dto.RequestDto.Login
{
    public class LoginReqDto
    {
        [JsonPropertyName("login")]
        public string Login { get; set; }
        [JsonPropertyName("password")]
        public string Password { get; set; }
    }
}
