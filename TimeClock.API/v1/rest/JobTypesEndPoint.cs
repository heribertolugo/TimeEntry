using Mapster;
using Microsoft.AspNetCore.Mvc;
using TimeClock.Api.Helpers;
using TimeClock.Api.Security;
using TimeClock.Core.Models.ApiDtos;
using TimeClock.Core.Models.EntityDtos;
using TimeClock.Data;
using TimeClock.Data.Models;

namespace TimeClock.Api.v1.rest;

public class JobTypesEndPoint
{
    public static Delegate GetJobTypesDelegate = GetJobTypes;

    private static async Task<IResult> GetJobTypes(HttpContext context, IDataRepositoryFactory data, ILogger<JobTypesEndPoint> logger, JwtService jwt, ITimeClockCacheDataProvider cache, SecretsProvider secretsProvider, 
        Guid deviceId, [FromQuery] string getJobTypesData, CancellationToken cancellationToken = default)
    {
        (int, GenericGetDto?) decrypted = await EndPointValidationHelpers.ValidateEncryptedPackage<GenericGetDto, JobType, JobTypesEndPoint>(deviceId, getJobTypesData, true, data, logger);
        GenericGetDto? getJobTypesDto;

        if (decrypted.Item1 != StatusCodes.Status200OK)
            return TypedResults.StatusCode(decrypted.Item1);

        getJobTypesDto = decrypted.Item2;

        if (getJobTypesDto == null)
        {
            logger.LogAudit<JobStep>($"{nameof(JobTypesEndPoint.GetJobTypesDelegate)} getJobTypesData failed to deserialize. getJobTypesData: {getJobTypesData}",
            EventIds.IncompleteOrBadData, Guid.Empty, data.GetEventAuditsRepository());
            return TypedResults.BadRequest();
        }

        if (!EndPointValidationHelpers.ValidateDeviceGuid(deviceId, getJobTypesDto.DeviceId, data, logger, secretsProvider))
            return TypedResults.BadRequest();

        var jobTypes = await cache.GetJobTypes(cancellationToken: cancellationToken);

        return TypedResults.Ok(jobTypes.OrderBy(d => d.Description).Adapt<IEnumerable<JobTypeDto>>());
    }
}
