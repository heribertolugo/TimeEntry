using Mapster;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using TimeClock.Api.Helpers;
using TimeClock.Api.Security;
using TimeClock.Core.Models.ApiDtos;
using TimeClock.Core.Models.EntityDtos;
using TimeClock.Data;
using TimeClock.Data.Models;
using Device = TimeClock.Data.Models.Device;

namespace TimeClock.Api.v1.rest;

public class UsersEndPoint
{
    public static readonly Delegate GetUsersDelegate = GetUsers;
    public static readonly Delegate GetUserDelegate = GetUser;
    public static readonly Delegate UpdateUserDelegate = UpdateUser;
    public static readonly Delegate GetUserJobTypeStepsDelegate = GetUserJobTypeSteps;

    private static async Task<IResult> GetUsers(HttpContext context, IDataRepositoryFactory data, ILogger<UsersEndPoint> logger, JwtService jwt, ITimeClockCacheDataProvider cache, SecretsProvider secretsProvider,
        Guid deviceId, [FromQuery] string getUsersData, CancellationToken cancellationToken = default)
    {
        GetUsersDto? getUsersDto = GetUsersDto.Unwrap(getUsersData, true);
        List<Expression<Func<User, bool>>> filters = [];
        List<Expression<Func<User, object>>> includes = [];
        Expression<Func<User, bool>>? filterPredicate = default;
        IEnumerable<UserDto> usersDto;
        ISorting<User>? sorting = null;

        if (getUsersDto == null)
        {
            logger.LogAudit<User>($"{nameof(UsersEndPoint.GetUsers)} getUsersData failed to deserialize. getUsersData: {getUsersData}",
            EventIds.IncompleteOrBadData, Guid.Empty, data.GetEventAuditsRepository());
            return TypedResults.BadRequest();
        }

        if (!EndPointValidationHelpers.ValidateDeviceGuid(deviceId, getUsersDto.DeviceId, data, logger, secretsProvider))
            return TypedResults.BadRequest();

        if (getUsersDto.LocationId.HasValue)
            filters.Add(u => u.DepartmentsToLocation.LocationId == getUsersDto.LocationId.Value);
        if (getUsersDto.DepartmentId.HasValue)
            filters.Add(u => u.DepartmentsToLocation.DepartmentId == getUsersDto.DepartmentId.Value);
        if (getUsersDto.UserActiveState.HasValue)
            filters.Add(u => u.IsActive == getUsersDto.UserActiveState.Value);

        if (getUsersDto.IncludeEquipmentToUser)
            includes.Add(u => u.EquipmentsToDepartmentLocations.Select(q => q.Equipment));
        if (getUsersDto.IncludeClaims)
            includes.Add(u => u.UserClaims.Select(c => c.AuthorizationClaim));
        if (getUsersDto.IncludeJobType)
            includes.Add(u => u.DefaultJobType);
        if (getUsersDto.IncludeJobStep)
            includes.Add(u => u.DefaultJobStep);
        if (getUsersDto.IncludeBarCode)
            includes.Add(u => u.Barcodes.Where(b => b.DeactivatedOn == null));
        if (getUsersDto.IncludeJobTypeSteps)
        {
            if (getUsersDto.IncludedJobTypeStepsActiveState.HasValue)
                includes.Add(u => u.JobTypeSteps.Where(b => b.IsActive == getUsersDto.IncludedJobTypeStepsActiveState.Value));
            includes.Add(u => u.JobTypeSteps.Select(j => j.JobType));
            includes.Add(u => u.JobTypeSteps.Select(j => j.JobStep));
        }

        // aggregate filters. if this causes issues, use long version. see: EquipmentEndPoint.GetEquipments for long version
        filterPredicate = filters.DefaultIfEmpty(p => true).Aggregate((a, b) => a.And(b));

        var users = await data.GetUsersRepository().GetPagedAsync(filterPredicate, getUsersDto.Paging.Adapt<IPaging>(), sorting, false, cancellationToken, [.. includes]);

        usersDto = users.Adapt<IEnumerable<UserDto>>();

        return TypedResults.Ok(usersDto);
    }

    private static async Task<IResult> GetUser(HttpContext context, IDataRepositoryFactory data, ILogger<UsersEndPoint> logger, JwtService jwt, ITimeClockCacheDataProvider cache, SecretsProvider secretsProvider,
        Guid deviceId, Guid userId, [FromQuery] string getUserData, CancellationToken cancellationToken = default)
    {
        GetUserDto? getUserDto = GetUserDto.Unwrap(getUserData, true);
        List<PunchAction>? actions = getUserDto?.PunchesStates.Adapt<List<PunchAction>>();
        List<Expression<Func<User, bool>>> filters = [];
        List<Expression<Func<User, object>>> includes = [];
        Expression<Func<User, bool>>? filterPredicate = default;

        if (getUserDto == null)
        {
            logger.LogAudit<User>($"{nameof(UsersEndPoint.GetUser)} getUserData failed to deserialize. getUserData: {getUserData}",
            EventIds.IncompleteOrBadData, Guid.Empty, data.GetEventAuditsRepository());
            return TypedResults.BadRequest();
        }

        if (!EndPointValidationHelpers.ValidateDeviceGuid(deviceId, getUserDto.DeviceId, data, logger, secretsProvider))
            return TypedResults.BadRequest();

        if (getUserDto.EquipmentDateRange.HasValue)
        {
            //includes.Add(u => u.EquipmentsToDepartmentLocations.Where(q => q.LinkedOn >= getUserDto.EquipmentDateRange.Value.Start && q.LinkedOn <= getUserDto.EquipmentDateRange.Value.End).Select(q => q.Equipment));
            //filters.Add(u => u.EquipmentsToUsers.Any(q => q.LinkedOn >= getUserDto.EquipmentDateRange.Value.Start && q.LinkedOn <= getUserDto.EquipmentDateRange.Value.End));
        }

        if (getUserDto.UserActiveState.HasValue)
            filters.Add(u => u.IsActive == getUserDto.UserActiveState.Value);

        if (getUserDto.IncludeClaims)
            includes.Add(u => u.UserClaims.Select(c => c.AuthorizationClaim));
        if (getUserDto.IncludeJobType)
            includes.Add(u => u.DefaultJobType);
        if (getUserDto.IncludeJobStep)
            includes.Add(u => u.DefaultJobStep);

        if (userId != (getUserDto.UserId ?? Guid.Empty))
        {
            return TypedResults.BadRequest();
        }

        if (userId == default && !getUserDto.JdeId.HasValue)
        {
            return TypedResults.BadRequest();
        } 
        else if (userId != default)
        {
            filters.Add((u) => u.Id == userId);
        }
        else
        {
            filters.Add(u => u.JdeId == getUserDto.JdeId);
        }

        filterPredicate = filters.DefaultIfEmpty(p => true).Aggregate((a, b) => a.And(b));

        var user = (await data.GetUsersRepository().GetAsync(filterPredicate, tracking: false, token: cancellationToken, includes: includes.ToArray())).FirstOrDefault();

        // we run a separate query to get punches because it is tricky to filter them when you include them
        if (getUserDto.PunchesDateRange.HasValue && user is not null)
        {
#warning this should pull workperiods for user rather than punch entries
            List<Expression<Func<PunchEntry, bool>>> punchFilters = [];
            List<Expression<Func<PunchEntry, object>>> punchIncludes = [];
            Expression<Func<PunchEntry, bool>>? punchFilterPredicate = default;

            if (actions is not null && actions.Count != 0)
                punchFilters.Add(p =>
                    p.CurrentState.PunchEntriesHistory.DateTime >= getUserDto.PunchesDateRange.Value.Start &&
                    p.CurrentState.PunchEntriesHistory.DateTime <= getUserDto.PunchesDateRange.Value.End &&
                    actions.Contains(p.CurrentState.PunchEntriesHistory.Action));
            else
                punchFilters.Add(p =>
                    p.CurrentState.PunchEntriesHistory.DateTime >= getUserDto.PunchesDateRange.Value.Start &&
                    p.CurrentState.PunchEntriesHistory.DateTime <= getUserDto.PunchesDateRange.Value.End);

            punchFilters.Add(p => user.Id == p.UserId);
            punchIncludes.Add(p => p.WorkPeriod);

            punchFilterPredicate = punchFilters.DefaultIfEmpty(p => true).Aggregate((a, b) => a.And(b));

            user.PunchEntries = (ICollection<PunchEntry>)(await data.GetPunchEntriesRepository().GetAsync(predicate: punchFilterPredicate, null, false, cancellationToken, punchIncludes.ToArray()));
        }

        return TypedResults.Ok(user.Adapt<UserDto>());
    }

    private static async Task<IResult> UpdateUser(HttpContext context, IDataRepositoryFactory data, ILogger<UsersEndPoint> logger, JwtService jwt, ITimeClockCacheDataProvider cache, SecretsProvider secretsProvider,
        Guid deviceId, Guid userId, [FromQuery] string updateUserData, CancellationToken cancellationToken = default)
    {
        UpdateUserDto? updateUser = UpdateUserDto.Unwrap(updateUserData, true);
        User? user;
        Device? device = null;

        if (updateUser == null)
        {
            logger.LogAudit<User>($"{nameof(UsersEndPoint.GetUser)} updateUserData failed to deserialize. updateUser: {updateUser}",
            EventIds.IncompleteOrBadData, Guid.Empty, data.GetEventAuditsRepository());
            return TypedResults.BadRequest();
        }

        if (!EndPointValidationHelpers.ValidateDeviceGuid(deviceId, updateUser.DeviceId, data, logger, secretsProvider, out device))
            return TypedResults.BadRequest();

        user = (await data.GetUsersRepository().GetAsync(updateUser.UserId, true, cancellationToken));

        if (user is null)
        {
            if (device is not null)
            {
                await EndPointValidationHelpers.UpdateDeviceFailure(deviceId, true, data, secretsProvider);
            }
            logger.LogAudit<Device>($"Update user was called but user was not found. DeviceId: {deviceId}. DeviceFailures: {device?.FailureCount}. UserId: {updateUser.UserId}",
                EventIds.EntityNotFound, Guid.Empty, data.GetEventAuditsRepository());
            return TypedResults.BadRequest();
        }
        // should be if user has access to modify user
        if (!(await EndPointValidationHelpers.UserHasAccessToCreatePunch(user.Id, updateUser.ActionById, DateTime.Now, PunchActionDto.AdminPunch, data)))
        {
            logger.LogAudit<PunchEntry>($"{nameof(UsersEndPoint.UpdateUser)} access denied. user cannot modify user. userId: {user.Id} targetuser: {updateUser.ActionById} device: {updateUser.DeviceId}",
            EventIds.User, user.Id, data.GetEventAuditsRepository());
            await EndPointValidationHelpers.UpdateUserFailure(user.Id, true, data, secretsProvider);
            return TypedResults.BadRequest();
        }

        if (!string.IsNullOrWhiteSpace(updateUser.Barcode))
            await data.GetBarcodesRepository().AddAsync(updateUser.UserId, updateUser.ActionById, updateUser.Barcode, false, cancellationToken);
        else
            await data.GetBarcodesRepository().DeleteAsync(updateUser.Barcode ?? string.Empty, updateUser.UserId, false, cancellationToken);
        
        if (updateUser.JobTypeSteps.Any())
        {
            user.JobTypeSteps = updateUser.JobTypeSteps.Select(j => new JobTypeStepToUser()
            {
                IsActive = j.IsActive,
                Id = j.Id,
                JobStepId = j.JobStepId,
                JobTypeId = j.JobTypeId,
                UserId = j.UserId
            }).ToList();
        }

        user.UserName = updateUser.Username?.Trim();
        await data.GetUsersRepository().UpdateAsync(user, true, cancellationToken);

        return TypedResults.Ok();
    }

    private static async Task<IResult> SetUserJobSteps(User user, IDataRepositoryFactory data, Guid[] jobLinks)
    {
        //var existing = repo.GetAsync(r => )
        return TypedResults.BadRequest();
    }

    private static async Task<IResult> GetUserJobTypeSteps(HttpContext context, IDataRepositoryFactory data, ILogger<UsersEndPoint> logger, JwtService jwt, ITimeClockCacheDataProvider cache, SecretsProvider secretsProvider,
        Guid deviceId, Guid userId, [FromQuery] string getUserJobTypeStepsData, CancellationToken cancellationToken = default)
    {
        GetUserJobTypeStepsDto? dto = GetUserJobTypeStepsDto.Unwrap(getUserJobTypeStepsData, true);
        ISorting<JobTypeStepToUser> sorting = new Sorting<JobTypeStepToUser>();
        sorting.Sorts.Add(new Sort<JobTypeStepToUser>() { Order = SortOrder.Ascending, SortBy = j => j.JobType.JdeId });

#warning validate userid and deviceid

        return TypedResults.Ok((await data.GetJobTypeStepToUserRepository().GetAsync(
            predicate: j => j.UserId == dto.UserId,
            sorting: sorting,
            token: cancellationToken,
            includes: [j => j.JobStep, j => j.JobType])).Adapt<IEnumerable<JobTypeStepToUserDto>>());
    }
}
