using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using TimeClock.Api.Security;

namespace TimeClock.Api.Helpers;

public static class JwtBuilderExtensions
{
    private static Task LogAttempt(HttpRequest httpRequest, string eventType)
    {
        JwtSecurityToken? token = JwtService.GetFromRequest(httpRequest);

        if (token is null)
        {
            //log jwt not present
            Debug.WriteLine($"{eventType} token was null");
        }
        else
        {
            Debug.WriteLine($"{eventType} token was present");
            // log expiration (jwt.ValidTo.ToLongTimeString()), event type
        }

        return Task.CompletedTask;
    }
    public static IServiceCollection UseJwt(this IServiceCollection services, IConfiguration configuration)
    {
        SecretsProvider secrets = new(configuration);
        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IServiceProvider>((options, serviceProvider) => {
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateAudience = true,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,

                    ValidIssuer = secrets.GetJwtIssuer(),
                    ValidAudience = secrets.JwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(secrets.GetIssuerSigningKey()),
                    ClockSkew = JwtService.ClockSkew ?? new TimeSpan(0, 0, 5)
                    //, IssuerSigningKeyValidator = MyIssuerSigningKeyValidator
                    //, SignatureValidator = MySignatureValidator
                };
                options.Events = new JwtBearerEvents()
                {
                    //OnChallenge = c => LogAttempt(c.Request, nameof(JwtBearerEvents.OnChallenge)),
                    //OnTokenValidated = c => LogAttempt(c.Request, nameof(JwtBearerEvents.OnTokenValidated)),
                    //OnForbidden = c => LogAttempt(c.Request, nameof(JwtBearerEvents.OnForbidden)),
                    //OnAuthenticationFailed = (c) => LogAttempt(c.Request, nameof(JwtBearerEvents.OnAuthenticationFailed))
                };
                options.TokenHandlers.Clear();
                IServiceScopeFactory? serviceScopeFactory = serviceProvider.GetService<IServiceScopeFactory>();
                // we override null because we want it to fail if we get a null
                using (IServiceScope scope = serviceScopeFactory!.CreateScope())
                {
                    ITimeClockCacheDataProvider? cache = scope.ServiceProvider.GetService(typeof(ITimeClockCacheDataProvider)) as ITimeClockCacheDataProvider;
                    options.TokenHandlers.Add(new CustomValuesJwtTokenHandler(cache!, secrets));
                }
            });

        services.AddAuthentication(j =>
        {
            j.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            j.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            j.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer();
        return services;
    }

    public static bool MyIssuerSigningKeyValidator(SecurityKey securityKey, SecurityToken securityToken, TokenValidationParameters validationParameters)
    {
        return true;
    }
#warning revisit
    public static SecurityToken MySignatureValidator(string token, TokenValidationParameters validationParameters)
    {
        return new JwtSecurityToken(token);
    }
}
