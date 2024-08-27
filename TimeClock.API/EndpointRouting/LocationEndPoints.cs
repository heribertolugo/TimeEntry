using TimeClock.Api.v1.rest;

namespace TimeClock.Api.EndpointRouting;

public static class LocationEndPoints
{
    public static readonly string Route = $"/{Program.CurrentApiVersion}/rest/locations";
    //public static readonly string GetItemRouteName = "";
    //public static readonly string GetItemsRouteName = "";
    //public static readonly string GetItemsByUserRouteName = "";
    //public static readonly string UpdateItemRouteName = "";
    //public static readonly string CreateItemRouteName = "";
    public static IApplicationBuilder MapLocationEndPoints(this IApplicationBuilder app)
    {
        var group = ((WebApplication)app).MapGroup(LocationEndPoints.Route);

        group.MapGet("/{deviceId}", LocationsEndPoint.GetLocationsDelegate)
            .WithName("GetLocations")
            .WithOpenApi();
        group.MapGet("/{deviceId}/{locationId}", LocationsEndPoint.GetLocationDelegate)
            .WithName("GetLocation")
            .WithOpenApi();

        return app;
    }
}
