using Mapster;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using TimeClock.Api.Helpers;
using TimeClock.Api.Security;
using TimeClock.Core;
using TimeClock.Core.Models;
using TimeClock.Core.Models.ApiDtos;
using TimeClock.Core.Models.EntityDtos;
using TimeClock.Data;
using TimeClock.Data.Models;
using TimeClock.Data.Repository;
using Device = TimeClock.Data.Models.Device;

namespace TimeClock.Api.v1.rest;

public class EquipmentEndPoint
{
    //public static readonly Delegate GetDelegate;
    public static readonly Delegate GetManyDelegate = GetEquipments;
    public static readonly Delegate GetCountDelegate = GetEquipmentsCount;
    public static readonly Delegate UpdateDelegate = Update;

    [AllowAnonymous]
    private static async Task<IResult> GetEquipments(HttpContext context, IDataRepositoryFactory data, ILogger<AuthorizationEndPoint> logger, JwtService jwt, ITimeClockCacheDataProvider cache,
         [FromQuery] string getEquipmentsData, CancellationToken cancellationToken = default)
    {
        GetEquipmentsDto? getEquipments = GetEquipmentsDto.Unwrap(getEquipmentsData, true);

        if (getEquipments == null)
            return TypedResults.BadRequest();

        Expression<Func<Equipment, bool>> predicate;
        Sorting<Equipment>? sorting = null;
        List<Expression<Func<Equipment, object>>> includes =
        [
            (q) => q.EquipmentsToLocations,
            (q) => q.EquipmentType,
        ];

        if (getEquipments.IsActive.HasValue) 
            predicate = q => q.IsActive == getEquipments.IsActive.Value;
        else 
            predicate = q => q.Id != default;

        if (getEquipments.IncludeCurrentEquipmentToUser)
            includes.Add((q) => q.EquipmentsToUsers.Where(t => t.UnLinkedOn == null));
        if (getEquipments.IncludeEquipmentToUserUser)
            includes.Add((q) => q.EquipmentsToUsers.Select(q => q.User)); // this include sucks, because it will select null items. TODO: don't select nulls
        if (getEquipments.IncludeEquipmentJobTypeSteps)
            includes.Add(q => q.JobTypeStepToEquipment);

        if (getEquipments.HistoryForUserAndWorkPeriod.HasValue)
            predicate = PredicateBuilder.And(predicate, q => q.EquipmentsToUsers.Any(l => l.UserId == getEquipments.UserId.Value && l.WorkPeriod.WorkDate == getEquipments.HistoryForUserAndWorkPeriod.Value.WorkPeriodDate.Date.ToDateOnly()));

        if (getEquipments.DepartmentId.HasValue) 
            predicate = PredicateBuilder.And(predicate, q => q.EquipmentsToLocations.Any(l => l.DepartmentsToLocation.DepartmentId == getEquipments.DepartmentId.Value));

        if (getEquipments.LocationId.HasValue)
            predicate = PredicateBuilder.And(predicate, q => q.EquipmentsToLocations.Any(l => l.DepartmentsToLocation.LocationId == getEquipments.LocationId.Value));
        
        if (getEquipments.UserId.HasValue)
            predicate = PredicateBuilder.And(predicate, q => q.EquipmentsToUsers.Any(l => l.UserId == getEquipments.UserId.Value));

        if (getEquipments.Unlinked != GetEquipmentsDto.UnlinkedOption.None)
        {
            switch (getEquipments.Unlinked)
            {
                case GetEquipmentsDto.UnlinkedOption.Null:
                    predicate = PredicateBuilder.And(predicate, q => q.EquipmentsToUsers.Any(l => l.UnLinkedOn == null));
                    break;
                case GetEquipmentsDto.UnlinkedOption.NotNull:
                    predicate = PredicateBuilder.And(predicate, q => q.EquipmentsToUsers.Any(l => l.UnLinkedOn != null));
                    break;
                case GetEquipmentsDto.UnlinkedOption.DateRange:
                    // check if we are being asked to perform an action we are unable to fulfill due to incomplete data passed in
                    if (!getEquipments.UnlinkedDateRange.HasValue)
                        return TypedResults.BadRequest();
                    predicate = PredicateBuilder.And(predicate, q => q.EquipmentsToUsers.Any(l => l.UnLinkedOn >= getEquipments.UnlinkedDateRange.Value.Start && l.UnLinkedOn <= getEquipments.UnlinkedDateRange.Value.End));
                    break;
            }
        }

        if (getEquipments.LinkedDateRange.HasValue)
            predicate = PredicateBuilder.And(predicate, q => q.EquipmentsToUsers.Any(l => l.LinkedOn >= getEquipments.LinkedDateRange.Value.Start && l.LinkedOn <= getEquipments.LinkedDateRange.Value.End));

        if (getEquipments.Sorting.Count > 0) sorting = new Sorting<Equipment>();
        foreach (SortOption<GetEquipmentsDto.SortField> sortOption in getEquipments.Sorting)
        {
            switch (sortOption.Field)
            {
                case GetEquipmentsDto.SortField.User:
                    sorting!.Sorts.Add(new Sort<Equipment>() { SortBy = (q) => q.EquipmentsToUsers.Select(u => u.RowId).FirstOrDefault(), Order = sortOption.SortOrder == Core.Models.SortOrderDto.Ascending ? SortOrder.Ascending : SortOrder.Descending });
                    break;
                case GetEquipmentsDto.SortField.DepartmentLocation:
                    sorting!.Sorts.Add(new Sort<Equipment>() { SortBy = (q) => q.EquipmentsToLocations.Select(u => u.RowId).FirstOrDefault(), Order = sortOption.SortOrder == Core.Models.SortOrderDto.Ascending ? SortOrder.Ascending : SortOrder.Descending });
                    break;
                case GetEquipmentsDto.SortField.Sku:
                    sorting!.Sorts.Add(new Sort<Equipment>() { SortBy = (q) => q.Sku, Order = sortOption.SortOrder == Core.Models.SortOrderDto.Ascending ? SortOrder.Ascending : SortOrder.Descending });
                    break;
                case GetEquipmentsDto.SortField.Name:
                    sorting!.Sorts.Add(new Sort<Equipment>() { SortBy = (q) => q.Name, Order = sortOption.SortOrder == Core.Models.SortOrderDto.Ascending ? SortOrder.Ascending : SortOrder.Descending });
                    break;
                case GetEquipmentsDto.SortField.EquipmentType:
                    sorting!.Sorts.Add(new Sort<Equipment>() { SortBy = (q) => q.EquipmentType.Name, Order = sortOption.SortOrder == Core.Models.SortOrderDto.Ascending ? SortOrder.Ascending : SortOrder.Descending });
                    break;
                case GetEquipmentsDto.SortField.LastUsed: // u.LinkedOn cannot be null, because it has default value in db. if by chance it is, db max function will handle it
                    sorting!.Sorts.Add(new Sort<Equipment>() { SortBy = (q) => q.EquipmentsToUsers.Max(u => u.UnLinkedOn ?? u.LinkedOn), Order = sortOption.SortOrder == Core.Models.SortOrderDto.Ascending ? SortOrder.Ascending : SortOrder.Descending });
                    break;
                default:
                    break;
            }
        }

        //var entities = await data.GetEquipmentsRepository().GetPagedAsync(predicate, new Paging() { PageNumber = Math.Max(getEquipments.Paging.PageNumber, 1), PageSize = getEquipments.Paging.PageSize }, tracking: false, 
        //    token: cancellationToken, includes: includes.ToArray(), sorting: sorting);

        var entities = data.GetEquipmentsRepository().GetPaged(predicate, new Paging() { PageNumber = Math.Max(getEquipments.Paging.PageNumber, 1), PageSize = getEquipments.Paging.PageSize }, tracking: false,
            includes: includes.ToArray(), sorting: sorting);

        var equipments = entities.Adapt<IEnumerable<EquipmentDto>>();

        return TypedResults.Ok(equipments);
    }

    private static async Task<IResult> GetEquipmentsCount(HttpContext context, IDataRepositoryFactory data, ILogger<AuthorizationEndPoint> logger, JwtService jwt, ITimeClockCacheDataProvider cache,
        [FromQuery] string getEquipmentsData, CancellationToken cancellationToken = default)
    {
        GetEquipmentsDto? getEquipments = GetEquipmentsDto.Unwrap(getEquipmentsData, true);

        if (getEquipments == null)
            return TypedResults.BadRequest();

        Expression<Func<Equipment, bool>> predicate;

        if (getEquipments.IsActive.HasValue)
            predicate = q => q.IsActive == getEquipments.IsActive.Value;
        else
            predicate = q => q.Id != default;

        if (getEquipments.DepartmentId.HasValue)
            predicate = PredicateBuilder.And(predicate, q => q.EquipmentsToLocations.Any(l => l.DepartmentsToLocation.DepartmentId == getEquipments.DepartmentId.Value));

        if (getEquipments.LocationId.HasValue)
            predicate = PredicateBuilder.And(predicate, q => q.EquipmentsToLocations.Any(l => l.DepartmentsToLocation.LocationId == getEquipments.LocationId.Value));

        if (getEquipments.UserId.HasValue)
            predicate = PredicateBuilder.And(predicate, q => q.EquipmentsToUsers.Any(l => l.UserId == getEquipments.UserId.Value));

        if (getEquipments.Unlinked != GetEquipmentsDto.UnlinkedOption.None)
        {
            switch (getEquipments.Unlinked)
            {
                case GetEquipmentsDto.UnlinkedOption.Null:
                    predicate = PredicateBuilder.And(predicate, q => q.EquipmentsToUsers.Any(l => l.UnLinkedOn == null));
                    break;
                case GetEquipmentsDto.UnlinkedOption.NotNull:
                    predicate = PredicateBuilder.And(predicate, q => q.EquipmentsToUsers.Any(l => l.UnLinkedOn != null));
                    break;
                case GetEquipmentsDto.UnlinkedOption.DateRange:
                    // check if we are being asked to perform an action we are unable to fulfill due to incomplete data passed in
                    if (!getEquipments.UnlinkedDateRange.HasValue)
                        return TypedResults.BadRequest();
                    predicate = PredicateBuilder.And(predicate, q => q.EquipmentsToUsers.Any(l => l.UnLinkedOn >= getEquipments.UnlinkedDateRange.Value.Start && l.UnLinkedOn <= getEquipments.UnlinkedDateRange.Value.End));
                    break;
            }
        }

        if (getEquipments.LinkedDateRange.HasValue)
            predicate = PredicateBuilder.And(predicate, q => q.EquipmentsToUsers.Any(l => l.LinkedOn >= getEquipments.LinkedDateRange.Value.Start && l.LinkedOn <= getEquipments.LinkedDateRange.Value.End));


        var count = await data.GetEquipmentsRepository().GetCountAsync(predicate);

        return TypedResults.Ok(count);
    }

    private static async Task<IResult> Update(HttpContext context, IDataRepositoryFactory data, ILogger<EquipmentEndPoint> logger, SecretsProvider secretsProvider, JwtService jwt, ITimeClockCacheDataProvider cache,
        Guid deviceId, [FromQuery] string updateEquipmentToUserData, CancellationToken cancellationToken = default)
    {
        string? token = await context.GetTokenAsync(HeaderParams.TokenParameter);
        UpdateEquipmentToUserDto dto;
        (int, UpdateEquipmentToUserDto?) decrypted;
        AuthorizationClaimsDefinition claim;
        bool userIsAuthorized = false;
        EquipmentsToUser? equipmentsToUser = null;
        DateTime now = DateTime.Now;
        int roundMinutesBy = secretsProvider.GetTimeEntryInterval();
        DateTime effectiveNow = now.RoundMinutes(roundMinutesBy);
        Device? device;

        decrypted = await EndPointValidationHelpers.ValidateEncryptedPackage<UpdateEquipmentToUserDto, EquipmentsToUser, EquipmentEndPoint>(deviceId, updateEquipmentToUserData, true, data, logger);

        if (decrypted.Item1 != StatusCodes.Status200OK || decrypted.Item2 is null)
            return TypedResults.StatusCode(decrypted.Item1);

        dto = decrypted.Item2;

        if (!EndPointValidationHelpers.ValidateDeviceGuid(deviceId, dto.DeviceId, data, logger, secretsProvider, out device))
            return TypedResults.BadRequest();

        if (dto.ActionById == dto.UserId)
            claim = AuthorizationClaimsDefinition.CanSelectEquipment;
        else
            claim = AuthorizationClaimsDefinition.CanEditOthersPunches;

        userIsAuthorized = await data.GetUserClaimsRepository().UserHasClaimAsync(dto.ActionById, claim.Type, tracking: false);

        if (!userIsAuthorized)
            return TypedResults.Unauthorized();

        if (!dto.PunchEntry.UserId.HasValue)
            return TypedResults.BadRequest();
        if (dto.PunchEntry.UserId.Value != dto.UserId)
            return TypedResults.BadRequest();
        if (dto.PunchEntry.PunchAction != PunchActionDto.SelfEquipmentSelect && dto.PunchEntry.PunchAction != PunchActionDto.AdminEquipmentSelect)
            return TypedResults.BadRequest();

        // Equipment Unlink Process
        if (dto.EquipmentToUserId.HasValue)
        {
            equipmentsToUser = await data.GetEquipmentsRepository().UnlinkToUserAsync(dto.EquipmentToUserId, dto.EquipmentId, 
                dto.UserId, dto.ActionById, now, effectiveNow,
                dto.IsPunchingOut ? null : dto.JobStepId, dto.IsPunchingOut ? null : dto.JobTypeId, 
                true, cancellationToken);

            if (equipmentsToUser is null)
                return TypedResults.BadRequest();

            if (dto.IsPunchingOut)
            {
                PunchEntry entry = new() { UserId = dto.UserId, Id = Guid.NewGuid(), WorkPeriod = equipmentsToUser.WorkPeriod, WorkPeriodId = equipmentsToUser.WorkPeriodId };
                PunchEntriesHistory history = new()
                {
                    Action = dto.PunchEntry.PunchAction.Adapt<PunchAction>(),
                    ActionById = dto.ActionById,
                    DateTime = now,
                    DeviceId = deviceId,
                    EffectiveDateTime = effectiveNow,
                    Latitude = dto.PunchEntry.Latitude,
                    Longitude = dto.PunchEntry.Longitude,
                    PunchType = dto.PunchEntry.PunchType.Adapt<PunchType>(),
                    UtcTimeStamp = now.ToUniversalTime(),
                    PunchEntryId = entry.Id,
                    PunchEntry = entry,
                    JobTypeId = dto.JobTypeId,
                    JobStepId = dto.JobStepId
                };
                entry.PunchEntriesHistories.Add(history);
                await data.GetPunchEntriesRepository().AddAsync(entry, 0, dto.JobTypeId, dto.JobStepId, true, cancellationToken);
            }
        }
        // Equipment Link Process
        else
        {
            PunchEntriesHistory history = new()
            {
                Id = Guid.NewGuid(),
                ActionById = dto.ActionById,
                Action = PunchAction.SelfEquipmentSelect,
                DateTime = now,
                EffectiveDateTime = effectiveNow,
                DeviceId = deviceId,
                Latitude = dto.PunchEntry.Latitude,
                Longitude = dto.PunchEntry.Longitude,
                JobTypeId = dto.JobTypeId,
                JobStepId = dto.JobStepId
                // punch type is missing
            };

            dto.JobTypeId = await CommonValues.GetJobTypeId(dto.LocationDivisionCode, dto.UnionCode, dto.JobTypeId, data, secretsProvider);

            equipmentsToUser = await data.GetEquipmentsRepository().LinkToUserAsync(dto.EquipmentId, dto.UserId, dto.ActionById,
                dto.JobTypeId, dto.JobStepId, secretsProvider.GetWorkPeriodThresholdHours(), now, effectiveNow, 
                dto.WorkPeriodId, history, true, cancellationToken);
        }

        return TypedResults.Ok(equipmentsToUser.Adapt<EquipmentsToUserDto>());
    }
}
