using TimeClock.Api.v1.rest;

namespace TimeClock.Api.EndpointRouting;

public static class JobTypesEndPoints
{
    public static readonly string Route = $"/{Program.CurrentApiVersion}/rest/jobtypes";
    //public static readonly string GetItemRouteName = "";
    public static readonly string GetItemsRouteName = "GetJobTypes";
    //public static readonly string GetItemsByUserRouteName = "";
    //public static readonly string UpdateItemRouteName = "";
    //public static readonly string CreateItemRouteName = "";

    public static IApplicationBuilder MapJobTypesEndPoints(this IApplicationBuilder app)
    {
        var group = ((WebApplication)app).MapGroup(JobTypesEndPoints.Route);

        group.MapGet("/{deviceId}", JobTypesEndPoint.GetJobTypesDelegate)
            .WithName(JobTypesEndPoints.GetItemsRouteName)
            .WithOpenApi();

        return app;
    }
}
