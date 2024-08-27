using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using TimeClock.Api.Helpers;
using TimeClock.Api.Security;
using TimeClock.Core.Models.ApiDtos;
using TimeClock.Core.Models.EntityDtos;
using TimeClock.Data;
using Device = TimeClock.Data.Models.Device;

namespace TimeClock.Api.v1.rest;

public class DevicesEndPoint
{
    public static readonly Delegate GetDeviceDelegate = GetDevice;

    private static async Task<IResult> GetDevice(HttpContext context, IDataRepositoryFactory data, ILogger<DevicesEndPoint> logger, JwtService jwt, ITimeClockCacheDataProvider cache, SecretsProvider secretsProvider,
        Guid deviceId, [FromQuery] string getDeviceData, CancellationToken cancellationToken = default)
    {
        GetDeviceDto? getDeviceDto;
        DeviceDto? device;
        (int, GetDeviceDto?) decryptDto;

        decryptDto = await EndPointValidationHelpers.ValidateEncryptedPackage<GetDeviceDto, Device, DevicesEndPoint>(deviceId, getDeviceData, true, data, logger);

        if (decryptDto.Item1 != StatusCodes.Status200OK)
            return TypedResults.BadRequest();

        getDeviceDto = decryptDto.Item2 as GetDeviceDto;

        if (getDeviceDto == null)
            return TypedResults.BadRequest();

        if (!EndPointValidationHelpers.ValidateDeviceGuid(deviceId, getDeviceDto.DeviceId, data, logger, secretsProvider))
            return TypedResults.BadRequest();

        if (!(await cache.GetDevices(getDeviceDto.ForceRefresh)).TryGetValue(deviceId, out device))
            return TypedResults.BadRequest();

        if (getDeviceDto.IncludeDepartment || getDeviceDto.IncludeLocation)
        {
            List<Expression<Func<Data.Models.DepartmentsToLocation, object>>> includes = new();

            if (getDeviceDto.IncludeDepartment)
                includes.Add((d) => d.Department);
            if (getDeviceDto.IncludeLocation)
                includes.Add((d) => d.Location);

            device.DepartmentsToLocations = (await cache.GetOrSetCachedDbCollection<Data.Models.DepartmentsToLocation, DepartmentsToLocationDto>(
                TimeClockCacheDataProvider.DepartmentsToLocationsKey, TimeClockCacheDataProvider.DepartmentsToLocationsSemaphore, 
                data.GetDepartmentsToLocationsRepository(), cancellationToken: cancellationToken, includes: includes.ToArray())
                ).FirstOrDefault(t => t.Id == device.DepartmentsToLocationsId);

            if (device.DepartmentsToLocations is null)
                return TypedResults.BadRequest();
        }

        if (getDeviceDto.IncludeConfiguredBy)
        {
            device.ConfiguredBy = (await cache.GetUsers(cancellationToken: cancellationToken)).FirstOrDefault(u => u.Id == device.ConfiguredById);
        }
            

        return TypedResults.Ok(device);
    }
}
