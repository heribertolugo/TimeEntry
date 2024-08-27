using Mapster;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.DirectoryServices.ActiveDirectory;
using System.Linq.Expressions;
using System.Text.Json;
using TimeClock.Api.Helpers;
using TimeClock.Api.Security;
using TimeClock.Core;
using TimeClock.Core.Helpers;
using TimeClock.Core.Models;
using TimeClock.Core.Models.ApiDtos;
using TimeClock.Core.Models.EntityDtos;
using TimeClock.Core.Security;
using TimeClock.Data;
using TimeClock.Data.Helpers;
using TimeClock.Data.Models;
using TimeClock.Data.Repository;
using CommonCore = TimeClock.Core.HeaderParams;
using Device = TimeClock.Data.Models.Device;

namespace TimeClock.Api.v1.rest;

public class PunchEntryEndPoint
{
    public static readonly Delegate CreateDelegate = Create;
    public static readonly Delegate UpdateDelegate = Update;
    public static readonly Delegate GetDelegate = GetPunchEntry;
    public static readonly Delegate GetManyDelegate = GetPunchEntries;

    #region Create Punch Entry
    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="data"></param>
    /// <param name="logger"></param>
    /// <param name="jwt"></param>
    /// <param name="cache"></param>
    /// <param name="deviceId"></param>
    /// <param name="createPunchEntryData">This value is an encrypted JSON serialized CreatePunchEntryDto object</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private static async Task<IResult> Create(HttpContext context, IDataRepositoryFactory data, ILogger<PunchEntryEndPoint> logger, SecretsProvider secretsProvider,
        Guid deviceId, [FromQuery] string createPunchEntryData, CancellationToken cancellationToken = default)
    {
        string? token = await context.GetTokenAsync(CommonCore.TokenParameter);
        CreatePunchEntryDto dto;
        UserDto? userDto;
        User? user;
        PunchEntry entry;
        PunchEntriesHistory history;
        (int, CreatePunchEntryDto?) decryptPunch;
        Device? device = null;

        decryptPunch = await EndPointValidationHelpers.ValidateEncryptedPackage<CreatePunchEntryDto, PunchEntry, PunchEntryEndPoint>(deviceId, createPunchEntryData, true, data, logger);

        if (decryptPunch.Item1 != StatusCodes.Status200OK)
            return TypedResults.BadRequest();

        dto = decryptPunch.Item2!;

        if (!EndPointValidationHelpers.ValidateDeviceGuid(deviceId, dto.DeviceId, data, logger, secretsProvider, out device))
            return TypedResults.BadRequest();

        // this should really not bypass validating credentials. however, management wants a more streamline simple process with less entering passwords.
        // we will still validate the user IDs and access rights.
        switch (dto.PunchAction)
        {
            case PunchActionDto.AdminPunch:
            case PunchActionDto.AdminEdit:
            case PunchActionDto.Void:
            case PunchActionDto.AdminApproved:
            case PunchActionDto.NewRequest:
            case PunchActionDto.EditRequest:
                if (dto.UserId is null || !dto.UserId.HasValue)
                {
                    logger.LogAudit<PunchEntry>($"{nameof(PunchEntryEndPoint.Create)} access denied. userId is null. targetUser: {(dto.ActionById.HasValue ? dto.ActionById.Value : Guid.Empty)} device: {dto.DeviceId}",
                        EventIds.User, dto.Id, data.GetEventAuditsRepository());
                    if (device is not null)
                    {
                        await EndPointValidationHelpers.UpdateDeviceFailure(deviceId, true, data, secretsProvider);
                    }
                    return TypedResults.BadRequest();
                }
                user = (await data.GetUsersRepository().GetAsync(dto.UserId.Value, false, cancellationToken));
                userDto = user.Adapt<UserDto?>();
                break;
            case PunchActionDto.SelfEquipmentSelect:
            case PunchActionDto.AdminEquipmentSelect:
                user = null;
                userDto = null;
                break;
            case PunchActionDto.Self:
            case PunchActionDto.None:
            default:
                user = await EndPointValidationHelpers.ValidateUserCredentials<User, PunchEntryEndPoint>(dto.UserName, dto.Password, dto.PunchType, data, logger, secretsProvider);
                userDto = user.Adapt<UserDto?>();
                break;
        }

        if (user is null)
        {
            if (device is not null)
            {
                await EndPointValidationHelpers.UpdateDeviceFailure(deviceId, true, data, secretsProvider);
            }

            logger.LogAudit<Device>($"Create punch entry was called but user was not found. DeviceId: {deviceId}. DeviceFailures: {device?.FailureCount}. UserId: {dto.UserId}",
                EventIds.EntityNotFound,
                Guid.Empty, data.GetEventAuditsRepository());
            return TypedResults.BadRequest();
        }

        if (user.FailureCount >= secretsProvider.GetMaxUserFailures())
        {
            logger.LogAudit<Device>($"User has exceeded failure count. DeviceId: {deviceId}. DeviceFailures: {device?.FailureCount}. UserId: {user.Id}",
                EventIds.EntityNotFound,
                Guid.Empty, data.GetEventAuditsRepository());
            return TypedResults.BadRequest();
        }

        if (dto.ActionById is null && dto.PunchAction != PunchActionDto.None)
            dto.ActionById = user.Id;
        else if (dto.ActionById is null && dto.PunchAction == PunchActionDto.None)
            dto.ActionById = Guid.Empty;

        if (!(await EndPointValidationHelpers.UserHasAccessToCreatePunch(user.Id, dto.ActionById!.Value, dto.DateTime, dto.PunchAction, data)))
        {
            logger.LogAudit<PunchEntry>($"{nameof(PunchEntryEndPoint.Create)} access denied. user cannot create punch. userId: {user.Id} targetUser: {dto.ActionById.Value} device: {dto.DeviceId}",
            EventIds.User, dto.Id, data.GetEventAuditsRepository());
          
            if (!string.IsNullOrWhiteSpace(user.UserName))
                await EndPointValidationHelpers.UpdateUserFailure(user.UserName, true, data, secretsProvider);
            else
                await EndPointValidationHelpers.UpdateUserFailure(user.Id, true, data, secretsProvider);
            return TypedResults.BadRequest();
        }

        if (string.IsNullOrWhiteSpace(user.UserName))
            await EndPointValidationHelpers.UpdateUserFailure(user.Id, false, data, secretsProvider);
        else
            await EndPointValidationHelpers.UpdateUserFailure(user.UserName, false, data, secretsProvider);

        entry = new PunchEntry() { UserId = user.Id, Id = dto.Id, DeviceId = dto.DeviceId };
        history = new PunchEntriesHistory()
        {
            Action = dto.PunchAction.Adapt<PunchAction>(),
            ActionById = dto.ActionById.Value,
            DateTime = dto.DateTime,
            DeviceId = dto.DeviceId,
            EffectiveDateTime = dto.DateTime.RoundMinutes(secretsProvider.GetTimeEntryInterval()),
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            PunchType = dto.PunchType.Adapt<PunchType>(),
            UtcTimeStamp = DateTime.UtcNow,
            Note = dto.Note
        };
#warning need to validate WorkPeriodId
        if (dto.WorkPeriodId.HasValue && dto.WorkPeriodId.Value != default)
            entry.WorkPeriodId = dto.WorkPeriodId.Value;
        entry.PunchEntriesHistories.Add(history);

        // user did not have multiple job type and step to choose from.
        // assign them from the user defaults
        if (!dto.IsJobTypeStepSet && !dto.JobStepId.HasValue) dto.JobStepId = user.DefaultJobStepId;
        // generate the job type if needed
        if (!dto.IsJobTypeStepSet && !dto.JobTypeId.HasValue) dto.JobTypeId = user.DefaultJobTypeId;
        // we are not going to try and generate the job type. these will have some type of logical mapping. for now, just use defaults
        //dto.JobTypeId = await CommonValues.GetJobTypeId(dto.LocationDivisionCode, dto.UnionCode, dto.JobTypeId, data, secretsProvider);
        //if (!dto.JobTypeId.HasValue) dto.JobTypeId = user.DefaultJobTypeId;

        history.JobStepId = dto.JobStepId;
        history.JobTypeId = dto.JobTypeId;

        await data.GetPunchEntriesRepository().AddAsync(entry, secretsProvider.GetWorkPeriodThresholdHours(), dto.JobTypeId, dto.JobStepId, false, cancellationToken);

        if ((dto.PunchAction == PunchActionDto.Self) && // || (dto.PunchAction == PunchActionDto.Self || dto.PunchAction == PunchActionDto.AdminPunch) &&
            !dto.IsJobTypeStepSet &&
            await data.GetJobTypeStepToUserRepository().GetCountForUserAsync(user.Id) > 0)
        {
            // if we are punched in, don't request redirect. instead just punch user out
            if (entry.CurrentState.StableStatus != PunchStatus.Out)
            {
                // if user has options to select job type, and IsJobTypeStepSet was not set -> return 303 status
                return Results.Json(
                    new PunchEntryDto() { User = userDto, UserId = user.Id },
                    statusCode: StatusCodes.Status303SeeOther);
            }
        }

        IEnumerable<EquipmentsToUser> unlinked = [];

        // if user isn't creating an actual punch, don't alter user equipment state
        // adding a punch entry already assigns job type step to work period
        // unlinking equipment does the same. null params to unlink so we don't try to add job type to work period again
        if (entry.CurrentState.StableStatus == PunchStatus.Out && dto.PunchAction != PunchActionDto.NewRequest && dto.PunchAction != PunchActionDto.EditRequest)
            unlinked = await data.GetEquipmentsRepository().UnlinkToUserByWorkPeriodAsync(entry.WorkPeriodId, history.ActionById, 
                history.DateTime, history.EffectiveDateTime, null, null,
                false, cancellationToken);

        await data.SaveAllAsync(cancellationToken);

        PunchEntryDto? entryDto = null;

        entryDto = entry.Adapt<PunchEntryDto>();

        if (dto.IncludeUser)
        {
            entryDto.User = userDto;
        }

        if (entryDto.WorkPeriod?.IsPreviousMissingPunch ?? false)
        {
            entryDto.WorkPeriod.PreviousMissingPunchDate = (await data.GetWorkPeriodsRepository().GetPreviousIfMissingPunchAsync(entryDto.WorkPeriodId, false, cancellationToken))
                .OrderByDescending(w => w.RowId).FirstOrDefault()?.WorkDate;
        }

        await SendEmailIfPunchRequest(dto, userDto, data, logger, secretsProvider, cancellationToken);

        data.Dispose();
        return TypedResults.Created("", entryDto);
    }
    private static async Task<IResult> Update(HttpContext context, IDataRepositoryFactory data, ILogger<PunchEntryEndPoint> logger, SecretsProvider secretsProvider,
        Guid deviceId, Guid userId, [FromQuery] string createPunchEntryData, CancellationToken cancellationToken = default)
    {
        string? token = await context.GetTokenAsync(CommonCore.TokenParameter);
        CreatePunchEntryDto dto;
        User? user;
        UserDto? userDto;
        PunchEntry? entry;
        PunchEntriesHistory history;
        (int, CreatePunchEntryDto?) decrypted;
        Device? device = null; 
        bool isValidDevice = false;

        decrypted = await EndPointValidationHelpers.ValidateEncryptedPackage<CreatePunchEntryDto, PunchEntry, PunchEntryEndPoint>(deviceId, createPunchEntryData, true, data, logger);

        if (decrypted.Item1 != StatusCodes.Status200OK)
            return TypedResults.StatusCode(decrypted.Item1);

        dto = decrypted.Item2!;

        if (!EndPointValidationHelpers.ValidateDeviceGuid(deviceId, dto.DeviceId, data, logger, secretsProvider, out device))
            return TypedResults.BadRequest();

        if (!dto.ActionById.HasValue)
            return TypedResults.BadRequest();
        if (!dto.UserId.HasValue)
            return TypedResults.BadRequest();

        user = (await data.GetUsersRepository().GetAsync(dto.UserId.Value, false, cancellationToken));
        userDto = user.Adapt<UserDto>();

        if (user is null)
        {
            if (device is not null)
            {
                await EndPointValidationHelpers.UpdateDeviceFailure(deviceId, true, data, secretsProvider);
            }

            logger.LogAudit<Device>($"Create punch entry was called but user was not found. DeviceId: {deviceId}. DeviceFailures: {device?.FailureCount}. UserId: {dto.UserId.Value}",
                EventIds.EntityNotFound,
                Guid.Empty, data.GetEventAuditsRepository());
            return TypedResults.BadRequest();
        }

        if (user.FailureCount >= secretsProvider.GetMaxUserFailures())
        {
            logger.LogAudit<Device>($"User has exceeded failure count. DeviceId: {deviceId}. DeviceFailures: {device?.FailureCount}. UserId: {user.Id}",
                EventIds.EntityNotFound,
                Guid.Empty, data.GetEventAuditsRepository());
            return TypedResults.BadRequest();
        }

        if (!(await EndPointValidationHelpers.UserHasAccessToCreatePunch(user.Id, dto.ActionById.Value, dto.DateTime, dto.PunchAction, data)))
        {
            logger.LogAudit<PunchEntry>($"{nameof(PunchEntryEndPoint.Update)} access denied. user cannot create punch. userId: {user.Id} targetUser: {dto.ActionById.Value} device: {dto.DeviceId}",
            EventIds.User, dto.Id, data.GetEventAuditsRepository());
            if (user.UserName is null)
                await EndPointValidationHelpers.UpdateUserFailure(user.Id, true, data, secretsProvider);
            else
                await EndPointValidationHelpers.UpdateUserFailure(user.UserName, true, data, secretsProvider);
            return TypedResults.BadRequest();
        }

        if (dto.ActionById is null)
            dto.ActionById = user.Id;

        entry = await data.GetPunchEntriesRepository().GetAsync(dto.Id, tracking: false, token: cancellationToken);

        if (entry is null || entry.UserId != user.Id)
        {
            logger.LogAudit<PunchEntry>($"{nameof(PunchEntryEndPoint.Update)} access denied. punch entry not found or user mismatch. userId: {user.Id} actionBy: {(dto.ActionById.HasValue ? dto.ActionById.Value : string.Empty)} punchId: {dto.Id} device: {dto.DeviceId}",
            EventIds.User, dto.Id, data.GetEventAuditsRepository());

            await EndPointValidationHelpers.UpdateUserFailure(user.Id, true, data, secretsProvider);
            return TypedResults.BadRequest();
        }

        await EndPointValidationHelpers.UpdateUserFailure(user.Id, false, data, secretsProvider);

        history = new PunchEntriesHistory()
        {
            Id = Guid.NewGuid(),
            Action = dto.PunchAction.Adapt<PunchAction>(),
            ActionById = dto.ActionById.Value,
            DateTime = dto.DateTime,
            DeviceId = dto.DeviceId,
            EffectiveDateTime = dto.DateTime.RoundMinutes(secretsProvider.GetTimeEntryInterval()),
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            PunchType = dto.PunchType.Adapt<PunchType>(),
            UtcTimeStamp = DateTime.UtcNow,
            PunchEntryId = entry.Id,
            Note = dto.Note
        };
#warning need to validate WorkPeriodId

        // generate the job type if needed
        dto.JobTypeId = await CommonValues.GetJobTypeId(dto.LocationDivisionCode, dto.UnionCode, dto.JobTypeId, data, secretsProvider);

        await data.GetPunchEntriesRepository().AddHistoryAsync(history, secretsProvider.GetWorkPeriodThresholdHours(), dto.JobTypeId, dto.JobStepId, true, cancellationToken);
        IEnumerable<EquipmentsToUser> unlinked = [];

        // if user isn't creating an actual punch, don't alter user equipment state
        if (dto.PunchAction != PunchActionDto.NewRequest && dto.PunchAction != PunchActionDto.EditRequest)
            unlinked = await data.GetEquipmentsRepository().UnlinkToUserByWorkPeriodAsync(entry.WorkPeriodId, history.ActionById, 
                history.DateTime, history.EffectiveDateTime, dto.JobStepId, dto.JobTypeId, true);

        await SendEmailIfPunchRequest(dto, userDto, data, logger, secretsProvider);

        return TypedResults.Created("url", history.PunchEntry.Adapt<PunchEntryDto>());
    }
    private static async Task SendEmailIfPunchRequest(CreatePunchEntryDto dto, UserDto user, 
        IDataRepositoryFactory data, ILogger<PunchEntryEndPoint> logger, SecretsProvider secretsProvider, CancellationToken cancellationToken = default)
    {
        string userManagerEmail = string.Empty;

        if (dto.PunchAction == PunchActionDto.EditRequest || dto.PunchAction == PunchActionDto.NewRequest)
        {
#if DEBUG

#warning get local active directory domain from config file, use if exists by uncommenting and fixing code below
            //try
            //{

            //    if (!"corporate.local".Equals(Domain.GetComputerDomain().Name, StringComparison.InvariantCultureIgnoreCase))
            //        return;
            //}
            //catch (ActiveDirectoryObjectNotFoundException ex)
            //{
            //    return;
            //}

#warning get debug email address from settings
            userManagerEmail = "heribertolugo@company.com";
#else
            userManagerEmail = (await data.GetUsersRepository().GetAsync(u => u.JdeId == user.SupervisorJdeId)).FirstOrDefault()?.PrimaryEmail ?? string.Empty;
#endif
            var email = await Emailer.SendTimeEntryRequest(userManagerEmail,
                new(user.FullNameOr, dto.DateTime, dto.PunchAction == PunchActionDto.EditRequest, dto.Note),
#warning need to check if it is first time email
                true,
                secretsProvider.GetEmailConnectivity(), secretsProvider.GetPunchRequestBccParties(), cancellationToken);
            await data.GetSentEmailsRepository().AddAsync(new SentEmail()
            {
                SentOn = DateTime.Now,
                SentTo = email.To,
                Signature = email.Hash,
                Message = email.Message,
                Subject = email.Subject
            });
        }
    }
    private static async Task<UserDto?> GetValidatedBarcodeUser(CreatePunchEntryDto punchEntry, ITimeClockCacheDataProvider cache, CancellationToken cancellationToken)
    {
        //data.GetUsersRepository().GetByBarcodeAsync(punchEntry.UserName);
        var barcodes = await cache.GetBarcodes(cancellationToken: cancellationToken);
        var barcode = barcodes.FirstOrDefault(b => b.Value == punchEntry.UserName);
        if (barcode == null)
            return null;
        var users = await cache.GetUsers();
        var user = users.FirstOrDefault(u => u.Id == barcode.UserId);

        return user;
    }
    private static async Task<UserDto?> GetValidatedDomainUser(CreatePunchEntryDto punchEntry, IDataRepositoryFactory data, ITimeClockCacheDataProvider cache, ILogger<AuthorizationEndPoint> logger)
    {
        if (string.IsNullOrWhiteSpace(punchEntry.Password))
            throw new Exception("PunchEntry password was missing at CreateDomainPunch");
        bool isValidCredentials = ActiveDirectoryHelper.AuthenticateWithEntry(punchEntry.UserName, punchEntry.Password, data, logger);

        if (!isValidCredentials)
            return null;

        //return data.GetUsersRepository().GetByUserNameAsync(punchEntry.UserName);

        return (await cache.GetUsers()).FirstOrDefault(u => u.UserName == punchEntry.UserName);
    }    

#endregion Create Punch Entry


    #region Update Geo Location
    private static async Task<Results<Ok, BadRequest>> UpdateGeoForPunch(HttpContext context, IDataRepositoryFactory data, ILogger<AuthorizationEndPoint> logger, JwtService jwt, ITimeClockCacheDataProvider cache,
        Guid deviceId, string updatedGeoForPunch, CancellationToken cancellationToken = default)
    {
        string? token = await context.GetTokenAsync(CommonCore.TokenParameter);
        string privateKeyId = string.Format(CommonValues.PrivateKeyNameFormatString, deviceId);
        ICryptographyService? crypto;
        UpdateGeoForPunchDataDto? updateData;
        PunchEntry? punchEntry;

        if (!RsaCryptographyService.TryGet(privateKeyId, out crypto) || crypto is null)
        {
            logger.LogAudit<PunchEntriesHistory>($"{nameof(PunchEntryEndPoint.UpdateGeoForPunch)} attempted to get a pivate RSA key which doesnt exist {privateKeyId} UpdateGeoForPunchData: {updatedGeoForPunch}",
            EventIds.CryptoTryGet, deviceId, data.GetEventAuditsRepository());
            return TypedResults.BadRequest();
        }

        updateData = CanJson<UpdateGeoForPunchDataDto>.FromJson(crypto.Decrypt(updatedGeoForPunch));

        if (updateData is null)
        {
            logger.LogAudit<PunchEntriesHistory>($"{nameof(PunchEntryEndPoint.UpdateGeoForPunch)} punch geo info failed to deserialize. pk: {privateKeyId} UpdateGeoForPunchData: {updatedGeoForPunch}",
            EventIds.CryptoTryGet, deviceId, data.GetEventAuditsRepository());
            return TypedResults.BadRequest();
        }

        if (updateData.DeviceId != deviceId)
        {
            logger.LogAudit<PunchEntriesHistory>($"{nameof(PunchEntryEndPoint.UpdateGeoForPunch)} device IDs mismatch. #1: {deviceId} #2: {updateData.DeviceId}",
            EventIds.ParseEntityId, updateData.PunchEntryHistoryId, data.GetEventAuditsRepository());
            return TypedResults.BadRequest();
        }

        if (updateData.UserId != updateData.ActionById)
        {
            if (!(await data.GetUserClaimsRepository().UserHasClaimAsync(updateData.ActionById, AuthorizationClaimsDefinition.CanEditOthersPunches.Type, tracking: false)))
            {
                logger.LogAudit<PunchEntriesHistory>($"{nameof(PunchEntryEndPoint.GetPunchEntries)} access denied. user cannot edit others punch. userId: {updateData.ActionById} targetuser: {updateData.UserId} device: {deviceId}",
                EventIds.User, updateData.ActionById, data.GetEventAuditsRepository());
                return TypedResults.BadRequest();
            }
        }

        if ((punchEntry = await data.GetPunchEntriesRepository().GetByHistoryIdAsync(updateData.PunchEntryHistoryId, false, cancellationToken)) == null)
        {
            logger.LogAudit<PunchEntriesHistory>($"{nameof(PunchEntryEndPoint.GetPunchEntries)} history item or punch not found by id. userId: {updateData.ActionById} targetuser: {updateData.UserId} device: {deviceId} punch: {updateData.PunchEntryId} history:{updateData.PunchEntryHistoryId}",
            EventIds.User, updateData.ActionById, data.GetEventAuditsRepository());
            return TypedResults.BadRequest();
        }

        if (punchEntry.Id != updateData.PunchEntryId)
        {
            logger.LogAudit<PunchEntriesHistory>($"{nameof(PunchEntryEndPoint.GetPunchEntries)} attempting to update a punch history with incorrect punch id. userId: {updateData.ActionById} targetuser: {updateData.UserId} device: {deviceId} punch: {updateData.PunchEntryId} history:{updateData.PunchEntryHistoryId}",
            EventIds.User, updateData.ActionById, data.GetEventAuditsRepository());
            return TypedResults.BadRequest();
        }

        PunchEntriesHistory history = punchEntry.PunchEntriesHistories.FirstOrDefault(h => h.Id == updateData.PunchEntryHistoryId)!;
        history.Latitude = updateData.Latitude;
        history.Longitude = updateData.Longitude;

        await data.GetPunchEntriesRepository().UpdateAsync(punchEntry);

        return TypedResults.Ok();
    }
    #endregion Update Geo Location


    #region Get Punches
    private static async Task<Results<Ok<PunchEntryDto>, BadRequest>> GetPunchEntry(HttpContext context, IDataRepositoryFactory data, ILogger<AuthorizationEndPoint> logger, JwtService jwt, ITimeClockCacheDataProvider cache,
        Guid deviceId, Guid punchEntryId, [FromQuery] string getPunchEntry, CancellationToken cancellationToken = default)
    {
        string? token = await context.GetTokenAsync(CommonCore.TokenParameter);
        string privateKeyId = string.Format(CommonValues.PrivateKeyNameFormatString, deviceId);
        ICryptographyService? crypto;
        GetPunchEntryDto? getPunchData;
        UserDto? requestingUser;
        PunchEntryDto? punchEntry;

        if (!RsaCryptographyService.TryGet(privateKeyId, out crypto) || crypto is null)
        {
            logger.LogAudit<PunchEntry>($"{nameof(PunchEntryEndPoint.GetPunchEntry)} attempted to get a private RSA key which doesn't exist {privateKeyId} getPunchEntry: {getPunchEntry}",
            EventIds.CryptoTryGet, Guid.Empty, data.GetEventAuditsRepository());
            return TypedResults.BadRequest();
        }

        getPunchData = JsonSerializer.Deserialize<GetPunchEntryDto>(crypto.Decrypt(getPunchEntry));

        if (getPunchData is null)
        {
            logger.LogAudit<PunchEntry>($"{nameof(PunchEntryEndPoint.GetPunchEntry)} punch entry failed to deserialize. pk: {privateKeyId} punchEntry: {getPunchEntry}",
            EventIds.CryptoTryGet, Guid.Empty, data.GetEventAuditsRepository());
            return TypedResults.BadRequest();
        }

        if (getPunchData.DeviceId != deviceId)
        {
            logger.LogAudit<PunchEntry>($"{nameof(PunchEntryEndPoint.GetPunchEntry)} device IDs mismatch. #1: {deviceId} #2: {getPunchData.DeviceId}",
            EventIds.ParseEntityId, getPunchData.Id, data.GetEventAuditsRepository());
            return TypedResults.BadRequest();
        }

        requestingUser = (await data.GetUsersRepository().GetAsync(getPunchData.RequestedById, tracking: false)).Adapt<UserDto>();

        if (requestingUser is null)
        {
            logger.LogAudit<PunchEntry>($"{nameof(PunchEntryEndPoint.GetPunchEntry)} user not found. requestUserId: {getPunchData.RequestedById} deviceId: {deviceId}",
            EventIds.User, Guid.Empty, data.GetEventAuditsRepository());
            return TypedResults.BadRequest();
        }

        // get punch entry
        punchEntry = (await data.GetPunchEntriesRepository().GetAsync(getPunchData.Id, tracking: false)).Adapt<PunchEntryDto>();

        if (punchEntry is null)
        {
            logger.LogAudit<PunchEntry>($"{nameof(PunchEntryEndPoint.GetPunchEntry)} punchEntry not found. punchEntryId: {getPunchData.Id} deviceId: {deviceId} requestedById {getPunchData.RequestedById}",
            EventIds.User, Guid.Empty, data.GetEventAuditsRepository());
            return TypedResults.BadRequest();
        }

        // check requesting user has permissions to make this request
        if (getPunchData.RequestedById != punchEntry.UserId)
        {
            if (!(await data.GetUserClaimsRepository().UserHasClaimAsync(getPunchData.RequestedById, AuthorizationClaimsDefinition.CanViewOthersPunches.Type, tracking: false)))
            {
                logger.LogAudit<PunchEntry>($"{nameof(PunchEntryEndPoint.GetPunchEntry)} access denied. user cannot view others punch. username: {getPunchData.RequestedById} targetuser: {punchEntry.UserId} device: {deviceId}",
                EventIds.User, getPunchData.RequestedById, data.GetEventAuditsRepository());
                return TypedResults.BadRequest();
            }
        }

        // return punch entry
        return TypedResults.Ok(punchEntry);
    }


    private static async Task<Results<Ok<IEnumerable<PunchEntryDto>>, BadRequest>> GetPunchEntries(HttpContext context, IDataRepositoryFactory data, ILogger<AuthorizationEndPoint> logger, JwtService jwt, ITimeClockCacheDataProvider cache, SecretsProvider secretsProvider,
        Guid deviceId, [FromQuery] string getPunchEntriesData, CancellationToken cancellationToken = default)
    {
        GetPunchEntriesDto? getPunchEntries = GetPunchEntriesDto.Unwrap(getPunchEntriesData, true);
        List<Expression<Func<PunchEntry, object>>> includes = [];
        List<Expression<Func<PunchEntry, bool>>> filters = [];
        List<PunchAction>? actions = getPunchEntries?.CurrentStates.Adapt<List<PunchAction>>();
        Expression<Func<PunchEntry, bool>>? filterPredicate = default;
        ISorting<PunchEntry>? sorting = null;
        UserDto? requestingUser;
        Device? device = null;

        List<Expression<Func<Equipment, object>>> equipmentIncludes = [];
        List<Expression<Func<Equipment, bool>>> equipmentFilters = [];
        Expression<Func<Equipment, bool>>? equipmentFilterPredicate = default;
        ISorting<Equipment>? equipmentSorting = null;
        IEnumerable<Equipment> equipments = [];

        if (getPunchEntries is null)
        {
            logger.LogAudit<PunchEntry>($"{nameof(PunchEntryEndPoint.GetPunchEntries)} punch entries failed to deserialize. punchentries: {getPunchEntriesData}",
            EventIds.CryptoTryGet, Guid.Empty, data.GetEventAuditsRepository());
            return TypedResults.BadRequest();
        }

        if (!EndPointValidationHelpers.ValidateDeviceGuid(deviceId, getPunchEntries.DeviceId, data, logger, secretsProvider, out device))
            return TypedResults.BadRequest();

        requestingUser = (await data.GetUsersRepository().GetAsync(getPunchEntries.RequestedById, tracking: false)).Adapt<UserDto>();

        if (requestingUser is null)
        {
            logger.LogAudit<PunchEntry>($"{nameof(PunchEntryEndPoint.GetPunchEntries)} user not found. requestUserId: {getPunchEntries.RequestedById} deviceId: {deviceId}",
            EventIds.User, Guid.Empty, data.GetEventAuditsRepository());
            return TypedResults.BadRequest();
        }

        // check if the owning user is requesting punch history, if not check that the requester does have permission to perform this request
        if (getPunchEntries.UserId != getPunchEntries.RequestedById)
        {
            if (!(await data.GetUserClaimsRepository().UserHasClaimAsync(getPunchEntries.RequestedById, AuthorizationClaimsDefinition.CanViewOthersPunches.Type, tracking: false)))
            {
                logger.LogAudit<PunchEntry>($"{nameof(PunchEntryEndPoint.GetPunchEntries)} access denied. user cannot view others punch. userId: {getPunchEntries.RequestedById} targetuser: {getPunchEntries.UserId?.ToString() ?? "all"} device: {deviceId}",
                EventIds.User, getPunchEntries.RequestedById, data.GetEventAuditsRepository());
                return TypedResults.BadRequest();
            }
        }

        // build the query using the provided options

        // includes requested
        if (getPunchEntries.IncludeUser)
            includes.Add(p => p.User);
        if (getPunchEntries.IncludeHistory)
            includes.Add(p => p.PunchEntriesHistories);
        if (getPunchEntries.IncludeDepartment)
            includes.Add(p => p.CurrentState.PunchEntriesHistory.Device.DepartmentsToLocations.Department);
        if (getPunchEntries.IncludeLocation)
            includes.Add(p => p.CurrentState.PunchEntriesHistory.Device.DepartmentsToLocations.Location);
        if (getPunchEntries.IncludeStableState)
            includes.Add(p => p.CurrentState.StablePunchEntriesHistory);
        //if (getPunchEntries.IncludeWorkPeriodJobType)
        //{
        //    includes.Add(p => p.WorkPeriod.WorkPeriodJobTypeSteps.Select(t => t.JobType));
        //    includes.Add(p => p.WorkPeriod.WorkPeriodJobTypeSteps.Select(t => t.JobStep));
        //}
        else if (getPunchEntries.IncludeWorkPeriod)
            includes.Add(p => p.WorkPeriod);

        if (getPunchEntries.GetIfStableState)
            filters.Add(p => p.CurrentState.StablePunchEntriesHistoryId != null);

        // filter by date range provided
        filters.Add(p => p.CurrentState.PunchEntriesHistory.DateTime!.Value >= getPunchEntries.DateRange.Start.StartOfDay()
                    && p.CurrentState.PunchEntriesHistory.DateTime.Value <= getPunchEntries.DateRange.End.EndOfDay());

        // filter by user if specified
        if (getPunchEntries.UserId.HasValue)
            filters.Add(p => p.UserId == getPunchEntries.UserId);

        // filter by department if specified
        if (getPunchEntries.DepartmentId != null)
        {
            filters.Add(p => (p.CurrentState.PunchEntriesHistory.Device == null)
            ? false : p.CurrentState.PunchEntriesHistory.Device.DepartmentsToLocations.DepartmentId == getPunchEntries.DepartmentId);
        }

        // filter by location if specified
        if (getPunchEntries.LocationId != null)
        {
            filters.Add(p => (p.CurrentState.PunchEntriesHistory.Device == null)
            ? false : p.CurrentState.PunchEntriesHistory.Device.DepartmentsToLocations.LocationId == getPunchEntries.LocationId);
        }

        // filter by punch actions if specified
        if (actions is not null && actions.Count != 0)
            filters.Add(p => actions.Any(a => a == p.CurrentState.PunchEntriesHistory.Action));

        // setup sorting requested
        if (getPunchEntries.Sorting.Count > 0) sorting = new Sorting<PunchEntry>();
        foreach (SortOption<GetPunchEntriesDto.SortField> sortOption in getPunchEntries.Sorting)
        {
            switch (sortOption.Field)
            {
                case GetPunchEntriesDto.SortField.DateTime:
                    sorting!.Sorts.Add(new Sort<PunchEntry>() { SortBy = p => p.CurrentState.PunchEntriesHistory.DateTime, Order = sortOption.SortOrder == SortOrderDto.Ascending ? SortOrder.Ascending : SortOrder.Descending });
                    break;
                case GetPunchEntriesDto.SortField.User:
                    sorting!.Sorts.Add(new Sort<PunchEntry>() { SortBy = p => p.UserId, Order = sortOption.SortOrder == SortOrderDto.Ascending ? SortOrder.Ascending : SortOrder.Descending });
                    break;
                default:
                    break;
            }
        }

        // aggregate filters. if this causes issues, use long version. see: EquipmentEndPoint.GetEquipments for long version
        filterPredicate = filters.DefaultIfEmpty(p => true).Aggregate((a, b) => a.And(b));

        if (getPunchEntries.IncludeEquipment)
        {
            equipmentFilters.Add(q => q.EquipmentsToUsers.Any(u => u.LinkedOn >= getPunchEntries.DateRange.Start.StartOfDay() && u.LinkedOn <= getPunchEntries.DateRange.End.EndOfDay()));
            if (getPunchEntries.DepartmentId != default)
                equipmentFilters.Add(q => q.EquipmentsToLocations.Any(u => u.DepartmentsToLocation.DepartmentId == getPunchEntries.DepartmentId));
            if (getPunchEntries.LocationId != default)
                equipmentFilters.Add(q => q.EquipmentsToLocations.Any(u => u.DepartmentsToLocation.LocationId == getPunchEntries.LocationId));
            equipmentIncludes.Add(q => q.EquipmentsToUsers);
            equipmentSorting = new Sorting<Equipment>();
            equipmentFilterPredicate = equipmentFilters.DefaultIfEmpty(p => true).Aggregate((a, b) => a.And(b));

            equipments = data.GetEquipmentsRepository().Get(equipmentFilterPredicate, equipmentSorting, false, equipmentIncludes.ToArray());
        }

        //IEnumerable<PunchEntry> punchEntries = await data.GetPunchEntriesRepository().GetAsync(filterPredicate, sorting, false, cancellationToken, includes.ToArray());
        IEnumerable<PunchEntry> punchEntries = data.GetPunchEntriesRepository().Get(filterPredicate, sorting, false, includes.ToArray());
        IEnumerable<PunchEntryDto> punchDtos = punchEntries.Adapt<IEnumerable<PunchEntry>, IEnumerable<PunchEntryDto>>();
        IEnumerable<EquipmentDto> equipmentDtos = equipments.Adapt<IEnumerable<EquipmentDto>>();

        foreach (var link in equipmentDtos.SelectMany(q => q.EquipmentsToUsers))
        {
            foreach (var user in punchDtos.Where(p => p.UserId == link.UserId).Select(p => p.User))
            {
                if (user is null) continue;
                user.EquipmentsToUsers ??= new List<EquipmentsToUserDto>();
                link.Equipment ??= equipmentDtos.FirstOrDefault(q => q.Id == link.EquipmentId);
                user.EquipmentsToUsers.Add(link);
            }
        }

        return TypedResults.Ok(punchDtos);
    }
    #endregion Get Punches
}
