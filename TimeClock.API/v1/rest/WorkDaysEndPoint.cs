using Mapster;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using TimeClock.Api.Helpers;
using TimeClock.Api.Security;
using TimeClock.Core;
using TimeClock.Core.Models.ApiDtos;
using TimeClock.Core.Models.EntityDtos;
using TimeClock.Data;
using TimeClock.Data.Models;
using Device = TimeClock.Data.Models.Device;

namespace TimeClock.Api.v1.rest;

public class WorkPeriodsEndPoint
{
    public static readonly Delegate GetDelegate = GetWorkPeriod;
    public static readonly Delegate GetManyDelegate = GetWorkPeriods;

    private static async Task<IResult> GetWorkPeriod(HttpContext context, IDataRepositoryFactory data, ILogger<WorkPeriodsEndPoint> logger, JwtService jwt, ITimeClockCacheDataProvider cache, SecretsProvider secretsProvider,
        Guid deviceId, Guid workPeriodId, [FromQuery] string getWorkPeriodData, CancellationToken cancellationToken = default)
    {
        GetWorkPeriodDto? getWorkPeriodDto;
        (int, GetWorkPeriodDto?) decryptDto;
        List<Expression<Func<WorkPeriod, bool>>> filters = [];
        List<Expression<Func<WorkPeriod, object>>> includes = [];
        Expression<Func<WorkPeriod, bool>>? filterPredicate = default;
        IPaging? paging = null;

        decryptDto = await EndPointValidationHelpers.ValidateEncryptedPackage<GetWorkPeriodDto, WorkPeriod, WorkPeriodsEndPoint>(deviceId, getWorkPeriodData, true, data, logger);

        if (decryptDto.Item1 != StatusCodes.Status200OK)
            return TypedResults.StatusCode(decryptDto.Item1);

        getWorkPeriodDto = decryptDto.Item2;

        if (getWorkPeriodDto == null)
            return TypedResults.BadRequest();

        if (!EndPointValidationHelpers.ValidateDeviceGuid(deviceId, getWorkPeriodDto.DeviceId, data, logger, secretsProvider))
            return TypedResults.BadRequest();
        if (getWorkPeriodDto.UserId.HasValue)
            filters.Add(w => w.UserId == getWorkPeriodDto.UserId.Value);
        if (getWorkPeriodDto.IncludeUser)
            includes.Add(w => w.User);
        if (getWorkPeriodDto.IncludeEquipment)
        {
            includes.Add(w => w.EquipmentsToUsers.Select(q => q.Equipment));
            includes.Add(w => w.EquipmentsToUsers.Select(q => q.LinkedBy));
        }
        //if (getWorkPeriodDto.IncludeJobType)
        //    includes.Add(w => w.JobType);
        //if (getWorkPeriodDto.IncludeJobStep)
        //    includes.Add(w => w.JobStep);
        if (getWorkPeriodDto.IsProcessed.HasValue)
        {
            WorkPeriodStatus[] statuses = getWorkPeriodDto.IsProcessed.Value ? [WorkPeriodStatus.Submitted, WorkPeriodStatus.Accepted, WorkPeriodStatus.Rejected] : [WorkPeriodStatus.Pending];
            filters.Add(w => statuses.Contains(w.WorkPeriodStatusHistories.OrderByDescending(h => h.DateTime).First().Status));
        }
        //if (getWorkPeriodDto.JobTypeIds.Count > 0)
        //{
        //    filters.Add(w => getWorkPeriodDto.JobTypeIds.Contains(w.JobTypeId.Value));
        //    includes.Add(w => w.JobType);
        //}
        if (getWorkPeriodDto.WorkDate.HasValue)
            filters.Add(w => w.WorkDate == getWorkPeriodDto.WorkDate.Value.ToDateOnly());

        filters.Add(w => w.Id == workPeriodId);

        // aggregate filters. if this causes issues, use long version. see: EquipmentEndPoint.GetEquipments for long version
        filterPredicate = filters.DefaultIfEmpty(p => true).Aggregate((a, b) => a.And(b));

        WorkPeriod? workPeriod = getWorkPeriodDto.IncludePunchEntries
            ? (await data.GetWorkPeriodsRepository().GetWorkPeriodsWithPunchesAsync(predicate: filterPredicate, tracking: false, token: cancellationToken, includes: includes.ToArray())).FirstOrDefault()
            : (await data.GetWorkPeriodsRepository().GetAsync(predicate: filterPredicate, tracking: false, token: cancellationToken, includes: includes.ToArray())).FirstOrDefault();

        return TypedResults.Ok(workPeriod.Adapt<WorkPeriodDto>());
    }
    private static async Task<IResult> GetWorkPeriods(HttpContext context, IDataRepositoryFactory data, ILogger<WorkPeriodsEndPoint> logger, JwtService jwt, ITimeClockCacheDataProvider cache, SecretsProvider secretsProvider,
        Guid deviceId, [FromQuery] string getWorkPeriodsData, CancellationToken cancellationToken = default)
    {
        GetWorkPeriodsDto? getWorkPeriodsDto;
        (int, GetWorkPeriodsDto?) decryptDto;
        List<Expression<Func<WorkPeriod, bool>>> filters = [];
        List<Expression<Func<WorkPeriod, object>>> includes = [];
        Expression<Func<WorkPeriod, bool>>? filterPredicate = default;
        IPaging? paging = null;

        decryptDto = await EndPointValidationHelpers.ValidateEncryptedPackage<GetWorkPeriodsDto, WorkPeriod, WorkPeriodsEndPoint>(deviceId, getWorkPeriodsData, true, data, logger);

        if (decryptDto.Item1 != StatusCodes.Status200OK)
            return TypedResults.StatusCode(decryptDto.Item1);

        getWorkPeriodsDto = decryptDto.Item2;

        if (getWorkPeriodsDto == null)
            return TypedResults.BadRequest();

        if (!EndPointValidationHelpers.ValidateDeviceGuid(deviceId, getWorkPeriodsDto.DeviceId, data, logger, secretsProvider))
            return TypedResults.BadRequest();
        if (getWorkPeriodsDto.UserId.HasValue)
            filters.Add(w => w.UserId == getWorkPeriodsDto.UserId.Value);
        if (getWorkPeriodsDto.IncludeUser)
            includes.Add(w => w.User);
        if (getWorkPeriodsDto.IncludeEquipment)
        {
            includes.Add(w => w.EquipmentsToUsers.Select(q => q.Equipment));
            includes.Add(w => w.EquipmentsToUsers.Select(q => q.LinkedBy));
        }
        //if (getWorkPeriodsDto.IncludeJobType)
        //    includes.Add(w => w.JobType);
        //if (getWorkPeriodsDto.IncludeJobStep)
        //    includes.Add(w => w.JobStep);
        if (getWorkPeriodsDto.IsProcessed.HasValue)
        {
            //WorkPeriodStatus[] statuses = getWorkPeriodsDto.IsProcessed.Value ? [WorkPeriodStatus.Submitted, WorkPeriodStatus.Accepted, WorkPeriodStatus.Rejected] : [WorkPeriodStatus.Pending];
            if (getWorkPeriodsDto.IsProcessed.Value)
                filters.Add(w => w.WorkPeriodStatusHistories.OrderByDescending(h => h.DateTime).Select(h => h.Status).First() != WorkPeriodStatus.Pending);
            else
                filters.Add(w => w.WorkPeriodStatusHistories.OrderByDescending(h => h.DateTime).Select(h => h.Status).First() == WorkPeriodStatus.Pending);
        }
        //if (getWorkPeriodsDto.JobTypeIds.Count > 0)
        //{
        //    filters.Add(w => getWorkPeriodsDto.JobTypeIds.Contains(w.JobTypeId.Value));
        //    includes.Add(w => w.JobType);
        //}
        if (getWorkPeriodsDto.DateRange.HasValue)
            filters.Add(w => w.WorkDate >= getWorkPeriodsDto.DateRange.Value.Start.ToDateOnly() && w.WorkDate <= getWorkPeriodsDto.DateRange.Value.End.EndOfDay().ToDateOnly());
        if (getWorkPeriodsDto.Paging is not null)
            paging = getWorkPeriodsDto.Paging.Adapt<IPaging>();

        // aggregate filters. if this causes issues, use long version. see: EquipmentEndPoint.GetEquipments for long version
        filterPredicate = filters.DefaultIfEmpty(p => true).Aggregate((a, b) => a.And(b));

        IEnumerable<WorkPeriod> workperiods = getWorkPeriodsDto.IncludePunchEntries
            ? await data.GetWorkPeriodsRepository().GetWorkPeriodsWithPunchesAsync(predicate: filterPredicate, punchActions: getWorkPeriodsDto.PunchActions.Adapt<IEnumerable<PunchAction>>(), tracking: false, token: cancellationToken, includes: includes.ToArray())
            : await data.GetWorkPeriodsRepository().GetAsync(predicate: filterPredicate, tracking: false, token: cancellationToken, includes: includes.ToArray());
        
        return TypedResults.Ok(workperiods.Adapt<IEnumerable<WorkPeriodDto>>());
    }
}
