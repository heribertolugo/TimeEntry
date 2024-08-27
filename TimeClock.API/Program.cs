using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using TimeClock.Api.EndpointRouting;
using TimeClock.Api.Helpers;
using TimeClock.Api.Mapping;
using TimeClock.Api.Security;
using TimeClock.Data;

namespace TimeClock.Api;

public class Program
{
    public static readonly string CurrentApiVersion = "v1";
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        
        builder.Logging.AddProvider(new EventAuditLoggerProvider());

        IConfigurationRoot configurationRoot = builder.AddConfigFile().Build();
        
        builder.Services
            .AddSingleton<SecretsProvider>()
            .AddExceptionHandler<GlobalExceptionHandler>()
            .AddProblemDetails()
            .Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options => options.SerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles)
            // Auth services
            .AddAuthorization(options => options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme).Build())
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            .AddOpenApiSwagger()
            // Injected Domain Items
            .AddSingleton<IDataFactoryInitializerParams, DataFactoryInitializerParams>()
            .AddScoped<IDataRepositoryFactory, DataFactoryInitializer>()
            // Cache
            .AddMemoryCache()
            .AddScoped<ITimeClockCacheDataProvider, TimeClockCacheDataProvider>()
            .AddHostedService<InitializeCacheService>()
            // JWT Auth helper
            .UseJwt(configurationRoot)
            .AddScoped<JwtService>();

        // Entity Models to DTO Mappings
        RegisterMappings.RegisterAllMappings();

        WebApplication app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app
            .UseExceptionHandler()
            .UseHttpsRedirection()
            .MapAllEndPoints()
        //app.UseMiddleware<RequestAuthenticator>();
            .UseAuthentication()
            .UseAuthorization();
        //app.UseAntiforgery();

        app.Run();
    }
}    

