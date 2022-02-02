using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSwag;
using NSwag.Generation.Processors.Security;
using WebApplication.Auth;
using WebApplication.Auth.Handlers.ApiKey;
using WebApplication.Auth.JwtBearerToken;
using WebApplication.Auth.Security;
using WebApplication.Auth.Security.Impl;
using WebApplication.Auth.Security.Models;

namespace WebApplication
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SecurityConfigurationModel>(configuration.GetSection("Security"));

            services.AddAutoMapper(expression =>
            {
                expression.AddProfile(new MappingProfile());
            });
            services.AddTransient<AppConfiguration>();
            services.AddTransient<ISecurityServices, SecurityServices>();
            ConfigureJwtBearerTokenAndApiKey(services, configuration);

            services.AddOpenApiDocument(cfg =>
            {
                cfg.DocumentProcessors.Add(new SecurityDefinitionAppender("ApiKey", new OpenApiSecurityScheme
                {
                    Type = OpenApiSecuritySchemeType.ApiKey,
                    Name = AppConfiguration.ApiKey,
                    In = OpenApiSecurityApiKeyLocation.Header,
                    Description = "El ApiKey"
                }));
                cfg.OperationProcessors.Add(new OperationSecurityScopeProcessor("ApiKey"));
                cfg.PostProcess = document =>
                {
                    document.Info.Title = "Student App";
                    document.Info.Description = "Student App";
                };
            });

            return services;
        }
        public static void ConfigureJwtBearerTokenAndApiKey(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = ApiKeyAuthenticationOptions.DEFAULT_SCHEME;
                    options.DefaultChallengeScheme = ApiKeyAuthenticationOptions.DEFAULT_SCHEME;
                })
                .AddApiKeySupport(options => { })
                .AddOidcBearerTokenSupport(options => { });

            services.AddSingleton<IJwtAuthenticationService, JwtAuthenticationService>();
            services.AddAuthorization();
        }
    }
}
