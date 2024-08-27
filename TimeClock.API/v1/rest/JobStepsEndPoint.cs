using Mapster;
using Microsoft.AspNetCore.Mvc;
using TimeClock.Api.Helpers;
using TimeClock.Api.Security;
using TimeClock.Core.Models.ApiDtos;
using TimeClock.Core.Models.EntityDtos;
using TimeClock.Data;
using TimeClock.Data.Models;

namespace TimeClock.Api.v1.rest;

public class JobStepsEndPoint
{
    public static Delegate GetJobStepsDelegate = GetJobSteps;

    private static async Task<IResult> GetJobSteps(HttpContext context, IDataRepositoryFactory data, ILogger<JobStepsEndPoint> logger, JwtService jwt, ITimeClockCacheDataProvider cache, SecretsProvider secretsProvider,
        Guid deviceId, [FromQuery] string getJobStepsData, CancellationToken cancellationToken = default)
    {
        (int, GenericGetDto?) decrypted = await EndPointValidationHelpers.ValidateEncryptedPackage<GenericGetDto, JobStep, JobStepsEndPoint>(deviceId, getJobStepsData, true, data, logger);
        GenericGetDto? getJobStepsDto;

        if (decrypted.Item1 != StatusCodes.Status200OK)
            return TypedResults.StatusCode(decrypted.Item1);

        getJobStepsDto = decrypted.Item2;

        if (getJobStepsDto == null)
        {
            logger.LogAudit<JobStep>($"{nameof(JobStepsEndPoint.GetJobStepsDelegate)} getJobStepsData failed to deserialize. getJobStepsData: {getJobStepsData}",
            EventIds.IncompleteOrBadData, Guid.Empty, data.GetEventAuditsRepository());
            return TypedResults.BadRequest();
        }

        if (!EndPointValidationHelpers.ValidateDeviceGuid(deviceId, getJobStepsDto.DeviceId, data, logger, secretsProvider))
            return TypedResults.BadRequest();

        var jobSteps = await cache.GetJobSteps(cancellationToken: cancellationToken);

        return TypedResults.Ok(jobSteps.OrderBy(d => d.Description).Adapt<IEnumerable<JobStepDto>>());
    }
}
