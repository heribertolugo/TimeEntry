using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

namespace TimeClock.Api.Helpers;

public static class BuilderHelpers
{
    /// <summary>
    /// Adds json config file per Environment (developer, release, etc)
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IConfigurationBuilder AddConfigFile(this WebApplicationBuilder builder)
    {
        IWebHostEnvironment env = builder.Environment;
        return builder.Configuration.SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
    }

    public static IServiceCollection AddOpenApiSwagger(this IServiceCollection services)
    {
        return services.AddEndpointsApiExplorer()
            .AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new Microsoft.OpenApi.Models.OpenApiSecurityScheme()
                {
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Please enter Bearer",
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey
                });

                OpenApiSecurityScheme scheme = new()
                {
                    Reference = new OpenApiReference()
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = JwtBearerDefaults.AuthenticationScheme
                    }
                };

                options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    { scheme, Array.Empty<string>() }
                });
            });
    }
}
