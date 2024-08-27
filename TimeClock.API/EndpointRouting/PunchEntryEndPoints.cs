using TimeClock.Api.v1.rest;

namespace TimeClock.Api.EndpointRouting;

public static class PunchEntryEndPoints
{
    public static readonly string Route = $"/{Program.CurrentApiVersion}/rest/punchentries";
    //public static readonly string GetItemRouteName = "GetPunchEntry";
    /// <summary>
    /// Route/GetItemsRouteName/{deviceId}
    /// </summary>
    /// <remarks>this</remarks>
    public static readonly string GetItemsRouteName = "GetPunchEntries";
    public static readonly string GetItemsByIdRouteName = "GetUserPunchEntries";
    public static readonly string UpdateItemRouteName = "PutUserPunchEntry";
    public static readonly string CreateItemRouteName = "CreatePunchEntry";
    public static IApplicationBuilder MapPunchEntryEndPoints(this IApplicationBuilder app)
    {
        var group = ((WebApplication)app).MapGroup(PunchEntryEndPoints.Route);

        /*
         *    GET    | punchentries/deviceId?options=                | get by date range
         *    GET    | punchentries/deviceId/userId?options=         | get for user for date range
         *    POST   | punchentries/deviceId?options=                | new punch for user
         *    UPDATE | punchentries/deviceId/userId?options=         | add history state to punch entry (edit)
         *    GET    | punchentries/deviceId/userId/punchId?options= | get punch by id      
         */

        // [ get multiple punches   | GET   ] punchentries/deviceId/?paging=&sort=&data=
        group.MapGet("/{deviceId}", PunchEntryEndPoint.GetManyDelegate)
            .WithName(PunchEntryEndPoints.GetItemsRouteName)
            .WithOpenApi();
        // [ get punch entry by id  | GET   ] punchentries/deviceId/userId?paging=&sort=&data=
        group.MapGet("/{deviceId}/{punchEntryId}", PunchEntryEndPoint.GetDelegate)
            .WithName(PunchEntryEndPoints.GetItemsByIdRouteName)
            .WithOpenApi();
        // [ update punch for user | PATCH ] punchentries/deviceId/userId?paging=&sort=&data=
        group.MapPatch("/{deviceId}/{userId}", PunchEntryEndPoint.UpdateDelegate)
            .WithName(PunchEntryEndPoints.UpdateItemRouteName)
            .WithOpenApi();
        // [ add punch entry       | POST  ] punchentries/deviceId?punchData=
        group.MapPost("/{deviceId}", PunchEntryEndPoint.CreateDelegate)
            .WithName(PunchEntryEndPoints.CreateItemRouteName)
            .WithOpenApi();

        return app;
    }
}
