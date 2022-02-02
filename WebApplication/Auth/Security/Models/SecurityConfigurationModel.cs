namespace WebApplication.Auth.Security.Models
{
    public class SecurityConfigurationModel
    {
        public ApiKeyConfigurationModel[] AuthorizedApiKeys { get; set; }
        public UserInfo[] BasicAuthentication { get; set; }
    }
    public class ApiKeyConfigurationModel
    {
        public string Name { get; set; }
        public string ApiKey { get; set; }
    }
    public class UserInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
