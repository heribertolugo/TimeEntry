using TimeClock.Api.v1.rest;

namespace TimeClock.Api.EndpointRouting;

public static class DepartmentEndPoints
{
    public static readonly string Route = $"/{Program.CurrentApiVersion}/rest/departments";
    //public static readonly string GetItemRouteName = "";
    //public static readonly string GetItemsRouteName = "";
    //public static readonly string GetItemsByUserRouteName = "";
    //public static readonly string UpdateItemRouteName = "";
    //public static readonly string CreateItemRouteName = "";
    public static IApplicationBuilder MapDepartmentEndPoints(this IApplicationBuilder app)
    {
        var group = ((WebApplication)app).MapGroup(DepartmentEndPoints.Route);

        group.MapGet("/{deviceId}", DepartmentsEndPoint.GetDepartmentsDelegate)
            .WithName("GetDepartments")
            .WithOpenApi();

        return app;
    }
}
