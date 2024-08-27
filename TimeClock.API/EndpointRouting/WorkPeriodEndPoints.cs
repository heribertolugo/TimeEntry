using TimeClock.Api.v1.rest;

namespace TimeClock.Api.EndpointRouting;

public static class WorkPeriodEndPoints
{
    public static readonly string Route = $"/{Program.CurrentApiVersion}/rest/workperiods";
    public static readonly string GetItemRouteName = "GetWorkPeriod";
    public static readonly string GetItemsRouteName = "GetWorkPeriods";
    //public static readonly string GetItemsByUserRouteName = "";
    //public static readonly string UpdateItemRouteName = "";
    //public static readonly string CreateItemRouteName = "";
    public static IApplicationBuilder MapWorkPeriodsEndPoints(this IApplicationBuilder app)
    {
        var group = ((WebApplication)app).MapGroup(WorkPeriodEndPoints.Route);

        group.MapGet("/{deviceId}", WorkPeriodsEndPoint.GetManyDelegate)
            .WithName(WorkPeriodEndPoints.GetItemsRouteName)
            .WithOpenApi();

        group.MapGet("/{deviceId}/{workperiodId}/", WorkPeriodsEndPoint.GetDelegate)
            .WithName(WorkPeriodEndPoints.GetItemRouteName)
            .WithOpenApi();

        return app;
    }
}
