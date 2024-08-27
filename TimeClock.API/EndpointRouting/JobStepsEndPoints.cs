using TimeClock.Api.v1.rest;

namespace TimeClock.Api.EndpointRouting;

public static class JobStepsEndPoints
{
    public static readonly string Route = $"/{Program.CurrentApiVersion}/rest/jobsteps";
    //public static readonly string GetItemRouteName = "";
    public static readonly string GetItemsRouteName = "GetJobSteps";
    //public static readonly string GetItemsByUserRouteName = "";
    //public static readonly string UpdateItemRouteName = "";
    //public static readonly string CreateItemRouteName = "";

    public static IApplicationBuilder MapJobStepsEndPoints(this IApplicationBuilder app)
    {
        var group = ((WebApplication)app).MapGroup(JobStepsEndPoints.Route);

        group.MapGet("/{deviceId}", JobStepsEndPoint.GetJobStepsDelegate)
            .WithName(JobStepsEndPoints.GetItemsRouteName)
            .WithOpenApi();

        return app;
    }
}
