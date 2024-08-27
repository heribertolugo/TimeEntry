using TimeClock.Api.v1.rest;

namespace TimeClock.Api.EndpointRouting;

public static class EquipmentsEndPoints
{
    public static readonly string Route = $"/{Program.CurrentApiVersion}/rest/equipment";
    //public static readonly string GetItemRouteName = "";
    public static readonly string GetItemsRouteName = "GetEquipments";
    //public static readonly string GetItemsByUserRouteName = "";
    public static readonly string UpdateItemRouteName = "UpdateEquipmentToUser";
    //public static readonly string CreateItemRouteName = "";
    public static readonly string GetCountRouteName = "GetEquipmentsCount";
    public static IApplicationBuilder MapEquipmentsEndPoints(this IApplicationBuilder app)
    {
        var group = ((WebApplication)app).MapGroup(EquipmentsEndPoints.Route);

        group.MapGet("/", EquipmentEndPoint.GetManyDelegate)
            .WithName(GetItemsRouteName)
            .WithOpenApi();

        group.MapGet($"/count", EquipmentEndPoint.GetCountDelegate)
            .WithName(GetCountRouteName)
            .WithOpenApi();

        group.MapPatch("/{deviceId}", EquipmentEndPoint.UpdateDelegate)
            .WithName(UpdateItemRouteName)
            .WithOpenApi();

        return app;
    }
}
