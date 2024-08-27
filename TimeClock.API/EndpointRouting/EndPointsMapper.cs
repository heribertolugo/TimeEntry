namespace TimeClock.Api.EndpointRouting;

public static class EndPointsMapper
{
    // End Point Mappings
    /* End points should adhere to the following for consistency:
     * POST - create
     * PUT - replace
     * PATCH - update 1 or more fields
     * GET - read
     * DELETE - delete
     */
    public static IApplicationBuilder MapAllEndPoints(this IApplicationBuilder app)
    {
        return app
            .MapAuthorizationEndPoints()
            .MapPunchEntryEndPoints()
            .MapDepartmentEndPoints()
            .MapLocationEndPoints()
            .MapEquipmentsEndPoints()
            .MapUsersEndPoints()
            .MapDevicesEndPoints()
            .MapWorkPeriodsEndPoints()
            .MapJobStepsEndPoints()
            .MapJobTypesEndPoints();
    }
}
