using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using TimeClock.Api.Helpers;
using TimeClock.Api.Security;
using TimeClock.Core.Models.EntityDtos;
using TimeClock.Data;

namespace TimeClock.Api.v1.rest;

public class LocationsEndPoint
{
    public static Delegate GetLocationsDelegate = GetLocations;
    public static Delegate GetLocationDelegate = GetLocation;

    [AllowAnonymous]
    private static async Task<Results<Ok<IEnumerable<LocationDto>>, BadRequest>> GetLocations(HttpContext context, ILogger<LocationsEndPoint> logger, JwtService jwt, ITimeClockCacheDataProvider cache
        , string deviceId)
    {
        //string? token = await context.GetTokenAsync(CommonCore.TokenParameter);
        Guid deviceGuid;

        if (!Guid.TryParse(deviceId, out deviceGuid)) //token == null || 
        {
            return TypedResults.BadRequest();
        }

        var locations = await cache.GetLocations();

        return TypedResults.Ok(locations.OrderBy(l => l.Name).Adapt<IEnumerable<LocationDto>>());
    }
    [AllowAnonymous]
    private static async Task<Results<Ok<LocationDto>, BadRequest>> GetLocation(HttpContext context, IDataRepositoryFactory data, ILogger<LocationsEndPoint> logger, JwtService jwt, ITimeClockCacheDataProvider cache, SecretsProvider secretsProvider,
    string deviceId, Guid locationId)
    {
        //string? token = await context.GetTokenAsync(CommonCore.TokenParameter);
        Guid deviceGuid;

        if (!Guid.TryParse(deviceId, out deviceGuid)) //token == null || 
        {
            return TypedResults.BadRequest();
        }

        if (!EndPointValidationHelpers.ValidateDeviceGuid(deviceGuid, deviceGuid, data, logger, secretsProvider))
            return TypedResults.BadRequest();

        var locations = await cache.GetLocations();

        return TypedResults.Ok(locations.FirstOrDefault(l => l.Id == locationId).Adapt<LocationDto>());
    }
}
