using TimeClock.Api.v1.rest;

namespace TimeClock.Api.EndpointRouting;

public static class UsersEndPoints
{
    public static readonly string Route = $"/{Program.CurrentApiVersion}/rest/users";
    public static readonly string GetItemRouteName = "GetUser";
    public static readonly string GetItemsRouteName = "GetUsers";
    //public static readonly string GetItemsByUserRouteName = "";
    public static readonly string UpdateItemRouteName = "UpdateUser";
    //public static readonly string CreateItemRouteName = "";
    public static IApplicationBuilder MapUsersEndPoints(this IApplicationBuilder app)
    {
        var group = ((WebApplication)app).MapGroup(UsersEndPoints.Route);

        group.MapGet("/{deviceId}", UsersEndPoint.GetUsersDelegate)
            .WithName(UsersEndPoints.GetItemsRouteName)
            .WithOpenApi();

        group.MapGet("/{deviceId}/{userId}/", UsersEndPoint.GetUserDelegate)
            .WithName(UsersEndPoints.GetItemRouteName)
            .WithOpenApi();

        group.MapPatch("/{deviceId}/{userId}/update/", UsersEndPoint.UpdateUserDelegate)
            .WithName(UsersEndPoints.UpdateItemRouteName)
            .WithOpenApi();

        group.MapGet("/{deviceId}/{userId}/jobtypestep/", UsersEndPoint.GetUserJobTypeStepsDelegate)
            .WithName("GetUserJobTypeSteps")
            .WithOpenApi();

        return app;
    }
}
