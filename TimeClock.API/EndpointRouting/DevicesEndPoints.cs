using TimeClock.Api.v1.rest;

namespace TimeClock.Api.EndpointRouting;

public static class DevicesEndPoints
{
    public static readonly string Route = $"/{Program.CurrentApiVersion}/rest/devices";
    //public static readonly string GetItemRouteName = "";
    //public static readonly string GetItemsRouteName = "";
    //public static readonly string GetItemsByUserRouteName = "";
    //public static readonly string UpdateItemRouteName = "";
    //public static readonly string CreateItemRouteName = "";
    public static IApplicationBuilder MapDevicesEndPoints(this IApplicationBuilder app)
    {
        var group = ((WebApplication)app).MapGroup(DevicesEndPoints.Route);

        group.MapGet("/{deviceId}", DevicesEndPoint.GetDeviceDelegate)
            .WithName("GetDevice")
            .WithOpenApi();

        return app;
    }
}
