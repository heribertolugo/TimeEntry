using TimeClock.Api.Security;
using TimeClock.Api.v1.rest;

namespace TimeClock.Api.EndpointRouting;

public static class AuthorizationEndPoints
{
    public static readonly string Route = $"/{Program.CurrentApiVersion}/rest/authorization";
    //public static readonly string GetItemRouteName = "";
    //public static readonly string GetItemsRouteName = "";
    //public static readonly string GetItemsByUserRouteName = "";
    //public static readonly string UpdateItemRouteName = "";
    //public static readonly string CreateItemRouteName = "";
    public static IApplicationBuilder MapAuthorizationEndPoints(this IApplicationBuilder app)
    {
        var group = ((WebApplication)app).MapGroup(AuthorizationEndPoints.Route);

        group.MapGet("/verify/{deviceId}/{deviceName}", AuthorizationEndPoint.VerifyConnectionDelegate)
            .WithName("Verify")
            .WithOpenApi();
        group.MapGet("/{deviceId}/{deviceName}", AuthorizationEndPoint.CreatePublicKeyDelegate)
            .WithName("CreatePublicKey")
            .WithOpenApi();
        group.MapGet("/refreshtoken/{deviceId}", AuthorizationEndPoint.GetRefreshTokenDelegate)
            .WithName("GetRefreshToken")
            .WithOpenApi().AddEndpointFilter<JwtRequestFilter>();
        group.MapPost("/registerdevice/{deviceId}", AuthorizationEndPoint.RegisterDeviceDelegate)
            .WithName("RegisterDevice")
            .WithOpenApi();
        group.MapGet("/validateclaim/{deviceId}", AuthorizationEndPoint.ValidateClaimDelegate)
            .WithName("ValidateClaim")
            .WithOpenApi();
        return app;
    }
}
