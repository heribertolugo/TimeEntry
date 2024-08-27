using Mapster;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using TimeClock.Api.Security;
using TimeClock.Core.Models;
using TimeClock.Core.Models.EntityDtos;
using TimeClock.Core.Security;
using TimeClock.Data;
using TimeClock.Data.Models;
using TimeClock.Data.Repository;
using Device = TimeClock.Data.Models.Device;

namespace TimeClock.Api.Helpers;

internal sealed class EndPointValidationHelpers
{
    /// <summary>
    /// Will attempt to decrypt the provided <paramref name="encryptedPackage"/> to a <typeparamref name="TPackage"/> using the provided <paramref name="deviceId"/>. 
    /// An StatusCodes.Status200OK will be returned with unwrapped package if successful, 
    /// otherwise a StatusCodes.Status404NotFound will be returned if the private encryption key was not found, 
    /// a StatusCodes.Status400BadRequest is returned if packaged failed to unwrap. 
    /// Logging is performed to the LogAudit upon failure.
    /// </summary>
    /// <typeparam name="TPackage">A type implementing ICanJson, which provides a type specific means to unwrap a HTML encoded, base64 Encoded, encrypted JSON serialized object.</typeparam>
    /// <typeparam name="TEntity">An entity type implementing IEntityModel. This is used for logging purposes upon a failure.</typeparam>
    /// <param name="deviceId"></param>
    /// <param name="encryptedPackage">Unwrapped package of data</param>
    /// <param name="isHtmlEncoded"></param>
    /// <param name="data"></param>
    /// <param name="logger"></param>
    /// <param name="caller"></param>
    /// <returns>A dynamic type with a StatusCodes value for Item1, and either a valid instance of <typeparamref name="TPackage"/> or its default for Item2.</returns>
    public static ValueTask<(int, TPackage?)> ValidateEncryptedPackage<TPackage, TEntity, TEndPoint>(Guid deviceId, string encryptedPackage, bool isHtmlEncoded,
        IDataRepositoryFactory data, ILogger<TEndPoint> logger, [CallerMemberName] string caller = "", [CallerFilePath] string callerPath = "")
        where TPackage : ICanJson<TPackage> where TEntity : class, IEntityModel
    {
        string privateKeyId = string.Format(CommonValues.PrivateKeyNameFormatString, deviceId);
        string callerFile = System.IO.Path.GetFileNameWithoutExtension(callerPath);
        ICryptographyService? crypto;
        TPackage? package;

        if (!RsaCryptographyService.TryGet(privateKeyId, out crypto) || crypto is null)
        {
            logger.LogAudit<TEntity>($"{callerFile}.{caller} attempted to get a private RSA key which doesn't exist {privateKeyId}",
            EventIds.CryptoTryGet, deviceId, data.GetEventAuditsRepository());
            return ValueTask.FromResult<(int, TPackage?)>((StatusCodes.Status404NotFound, default));
        }

        package = TPackage.Unwrap(encryptedPackage, isHtmlEncoded, crypto);

        if (EqualityComparer<TPackage>.Default.Equals(package, default(TPackage)))
        {
            logger.LogAudit<TEntity>($"{callerFile}.{caller} failed to unwrap data: {encryptedPackage}",
            EventIds.CryptoTryGet, deviceId, data.GetEventAuditsRepository());
            return ValueTask.FromResult<(int, TPackage?)>((StatusCodes.Status400BadRequest, default));
        }

        return ValueTask.FromResult((StatusCodes.Status200OK, package));
    }

    public static bool ValidateDeviceGuid<T>(string deviceId, Guid unwrappedDeviceId,
        IDataRepositoryFactory data, ILogger<T> logger, SecretsProvider secretsProvider, bool isNew = false, [CallerMemberName] string caller = "", [CallerFilePath] string callerPath = "")
    {
        Guid deviceGuid;
        if (!Guid.TryParse(deviceId, out deviceGuid))
            return false;
        return ValidateDeviceGuid(deviceGuid, unwrappedDeviceId, data, logger, secretsProvider, out _ , isNew, caller, callerPath);
    }

    public static bool ValidateDeviceGuid<T>(Guid deviceId, Guid unwrappedDeviceId,
        IDataRepositoryFactory data, ILogger<T> logger, SecretsProvider secretsProvider, bool isNew = false, [CallerMemberName] string caller = "", [CallerFilePath] string callerPath = "")
    {
        return ValidateDeviceGuid(deviceId, unwrappedDeviceId, data, logger, secretsProvider, out _, isNew, caller, callerPath);
    }

    public static bool ValidateDeviceGuid<T>(string deviceId, Guid unwrappedDeviceId,
        IDataRepositoryFactory data, ILogger<T> logger, SecretsProvider secretsProvider, out Device? device, bool isNew = false, [CallerMemberName] string caller = "", [CallerFilePath] string callerPath = "")
    {
        Guid deviceGuid;
        if (!Guid.TryParse(deviceId, out deviceGuid))
        {
            device = null;
            return false;
        }
        return ValidateDeviceGuid(deviceId, unwrappedDeviceId, data, logger, secretsProvider, out device, isNew, caller, callerPath);
    }

    public static bool ValidateDeviceGuid<T>(Guid deviceId, Guid unwrappedDeviceId,
        IDataRepositoryFactory data, ILogger<T> logger, SecretsProvider secretsProvider, out Device? device, bool isNew = false, [CallerMemberName] string caller = "", [CallerFilePath] string callerPath = "")
    {
        string callerFile = System.IO.Path.GetFileNameWithoutExtension(callerPath);
        device = data.GetDevicesRepository().Get(deviceId, false, d => d.DepartmentsToLocations.Location);

        if (deviceId != unwrappedDeviceId)
        {
            logger.LogAudit<Device>($"{callerFile}.{caller} was called but deviceId mismatch. DeviceId: {deviceId}. Unwrapped DeviceId: {unwrappedDeviceId}.",
                EventIds.IncompleteOrBadData,
                Guid.Empty, data.GetEventAuditsRepository());

            if (device is not null)
            {
                UpdateDeviceFailure(deviceId, true, data, secretsProvider).GetAwaiter().GetResult();
                UpdateDeviceFailure(unwrappedDeviceId, true, data, secretsProvider).GetAwaiter().GetResult();
            }
            return false;
        }

        if (!isNew && device is null)
        {
            logger.LogAudit<Device>($"{callerFile}.{caller} was called but deviceId not found. DeviceId: {deviceId}. ",
                EventIds.EntityNotFound,
                Guid.Empty, data.GetEventAuditsRepository());
            return false;
        }

        if (!isNew && device!.FailureCount >= secretsProvider.GetMaxDeviceFailures())
        {
            logger.LogAudit<Device>($"{callerFile}.{caller} was called but device reached failure count. DeviceId: {deviceId}. ",
                EventIds.GetDevice,
                Guid.Empty, data.GetEventAuditsRepository());
            return false;
        }

        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TEndPoint"></typeparam>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <param name="punchType"></param>
    /// <param name="data"></param>
    /// <param name="logger"></param>
    /// <param name="employeeId">Used to validate domain user against database after successfully validating against domain. 
    /// After successful validation will assign provided username to the user, to link domain user to local entity, and then save changes. 
    /// If this parameter is null, validation will be attempted using username against local entity after successful domain validation.</param>
    /// <param name="cancellationToken"></param>
    /// <param name="caller"></param>
    /// <param name="callerPath"></param>
    /// <returns></returns>
    public static async Task<User?> ValidateUserCredentials<TEntity, TEndPoint>(string username, string? password, PunchTypeDto punchType,
        IDataRepositoryFactory data, ILogger<TEndPoint> logger, SecretsProvider secretsProvider, string? employeeId = null, CancellationToken cancellationToken = default, [CallerMemberName] string caller = "", [CallerFilePath] string callerPath = "")
        where TEntity : class, IEntityModel
    {
        User? entity = null;
        string callerFile = System.IO.Path.GetFileNameWithoutExtension(callerPath);
        Expression<Func<User, object>>[] includes =
        {
            (u) => u.UserClaims.Select(c => c.AuthorizationClaim),
            (u) => u.DefaultJobStep,
            (u) => u.DefaultJobType
        };

        switch (punchType)
        {
            case PunchTypeDto.Barcode:                
                    entity = await data.GetUsersRepository().GetByBarcodeAsync(username, false, cancellationToken, includes);
                if (entity is null)
                {
                    logger.LogAudit<TEntity>($"{callerFile}.{caller} was called with invalid barcode credentials. user: {username}. password: {password}", EventIds.User,
                        Guid.Empty, data.GetEventAuditsRepository());
                }
                break;
            case PunchTypeDto.Domain:
                if (!ActiveDirectoryHelper.AuthenticateWithEntry(username, password ?? string.Empty, data, logger))
                {
                    await UpdateUserFailure(username, true, data, secretsProvider);
                    //logger.LogAudit<TEntity>($"{callerFile}.{caller} was called with invalid domain credentials. user: {username}. password: {password}", EventIds.User,
                    //    Guid.Empty, data.GetEventAuditsRepository());
                    break;
                }
                try
                {
                    if (employeeId is not null)
                    {
                        entity = await data.GetUsersRepository().GetByEmployeeNumberAsync(employeeId, false, cancellationToken, includes);

                        if (entity is not null && entity.UserName is null)
                        {
                            entity.UserName = username;
                            await data.GetUsersRepository().UpdateAsync(entity, true, cancellationToken);
                        }
                    }
                    else
                    {
                        entity = await data.GetUsersRepository().GetByUserNameAsync(username, false, cancellationToken, includes);
                    }
                    if (entity is null)
                    {
                        logger.LogAudit<TEntity>($"{callerFile}.{caller} user not found. username: {username}",
                        EventIds.User, Guid.Empty, data.GetEventAuditsRepository());
                        break;
                    }
                }
                catch (Exception ex)
                {
                    logger.LogAudit<TEntity>($"{callerFile}.{caller} was called but exception occurred. Exception: {ex.Message} user: {username}. punchType: {Enum.GetName(punchType)}", EventIds.User,
                        Guid.Empty, data.GetEventAuditsRepository());
                }
                break;
            case PunchTypeDto.None:
            case PunchTypeDto.User:
            default:
                logger.LogAudit<TEntity>($"{callerFile}.{caller} was called but PunchTypeDto was invalid or not implemented. PunchTypeDto: {Enum.GetName(punchType)}.",
                EventIds.IncompleteOrBadData,
                Guid.Empty, data.GetEventAuditsRepository());
                break;
        }

        //if (entity is not null)
        //    user = entity.Adapt<UserDto>();// new UserDto(entity.Id, entity.FirstName, entity.LastName, entity.EmployeeNumber, entity.UserName, entity.IsActive ?? true, entity.FailureCount, entity.LastActionOn, entity.LockedOutOn,
                //entity.PunchEntries.Adapt<IEnumerable<PunchEntryDto>>(), Enumerable.Empty<EquipmentsToUserDto>(),
                //entity.UserClaims.Select(c => new UserClaimDto()
                //{
                //    AuthorizationClaimId = c.AuthorizationClaimId,
                //    Id = c.Id,
                //    RowId = c.RowId,
                //    UserId = c.UserId,
                //    AuthorizationClaim = new AuthorizationClaimDto()
                //    { Id = c.AuthorizationClaimId, RowId = c.AuthorizationClaim.RowId, Type = c.AuthorizationClaim.Type, Value = c.AuthorizationClaim.Value }
                //})) { SupervisorJdeId = entity.SupervisorJdeId };

        return entity;
    }

    public static Task<bool> UserHasAccessToCreatePunch(Guid targetUser, Guid actionByUser, DateTime punchDateTime, PunchActionDto punchAction, IDataRepositoryFactory data)
    {
        // the purpose of checking the DateTime of the target punch is to prevent a user with non-edit rights from creating a punch that should be created by an admin.
        // if the user is in fact creating a legitimate punch, the date and time should be within reason to the current date and time.
        // however we must consider some lag, so we will pick a reasonable arbitrary amount of time and ensure the attempted punch is not past that point or before the target punch date time.
        // the aforementioned only applies to PunchAction.Self. If PunchAction represents a request for self, Date and Time does not matter
        Func<DateTime, PunchActionDto, bool> isWithinReasonableTime = (d, a) =>
        {
            int arbitraryBufferForLag = 1; //in minutes
            DateTime now = DateTime.Now;
#warning IMPLEMENTATION NOT FINISHED
            switch (a)
            {
                case PunchActionDto.Self: return d < now && d >= now.AddMinutes(-1 * arbitraryBufferForLag);
                case PunchActionDto.NewRequest:
                case PunchActionDto.EditRequest: return true;
                // originally, these would've been false if targetUser == actionByUser
                // left here in case some useful logic needs to be added here.
                case PunchActionDto.AdminPunch:
                case PunchActionDto.AdminEdit:
                case PunchActionDto.Void:
                case PunchActionDto.AdminApproved:
                default: return true;
            }
        };

        if (targetUser == actionByUser)
            return Task.FromResult(isWithinReasonableTime(punchDateTime, punchAction));

        return data.GetUserClaimsRepository().UserHasClaimAsync(actionByUser, AuthorizationClaimsDefinition.CanEditOthersPunches.Type, tracking: false);
    }

    public static async Task UpdateUserFailure(string username, bool isFailure, IDataRepositoryFactory data, SecretsProvider secretsProvider)
    {
        var n = await data.GetUsersRepository().GetByUserNameAsync(username, false);
        if (n is null) return;
        int failCount = secretsProvider.GetMaxUserFailures();
        DateTime? lockoutOn = n.FailureCount >= failCount - 1 ? DateTime.Now : null;

        switch (isFailure)
        {
            case true:
                await data.GetUsersRepository().UpdateToDbAsync(n.Id, (p) => p.SetProperty(u => u.FailureCount, n.FailureCount + 1).SetProperty(u => u.LockedOutOn, lockoutOn));
                break;
            default:
                await data.GetUsersRepository().UpdateToDbAsync(n.Id, (p) => p.SetProperty(u => u.FailureCount, 0).SetProperty(u => u.LockedOutOn, (DateTime?)null));
                break;
        }
    }
    public static async Task UpdateUserFailure(Guid userId, bool isFailure, IDataRepositoryFactory data, SecretsProvider secretsProvider)
    {
        var n = await data.GetUsersRepository().GetAsync(userId, false);
        if (n is null) return;
        int failCount = secretsProvider.GetMaxUserFailures();
        DateTime? lockoutOn = n.FailureCount >= failCount - 1 ? DateTime.Now : null;

        switch (isFailure)
        {
            case true:
                await data.GetUsersRepository().UpdateToDbAsync(n.Id, (p) => p.SetProperty(u => u.FailureCount, n.FailureCount + 1).SetProperty(u => u.LockedOutOn, lockoutOn));
                break;
            default:
                await data.GetUsersRepository().UpdateToDbAsync(n.Id, (p) => p.SetProperty(u => u.FailureCount, 0).SetProperty(u => u.LockedOutOn, (DateTime?)null));
                break;
        }
    }
    public static async Task UpdateDeviceFailure(Guid deviceId, bool isFailure, IDataRepositoryFactory data, SecretsProvider secretsProvider)
    {
        var n = await data.GetDevicesRepository().GetAsync(deviceId, false);
        if (n is null) return;
        int failCount = secretsProvider.GetMaxUserFailures();
        DateTime? lockoutOn = n.FailureCount >= failCount - 1 ? DateTime.Now : null;

        switch (isFailure)
        {
            case true:
                await data.GetDevicesRepository().UpdateToDbAsync(n.Id, (p) => p.SetProperty(u => u.FailureCount, n.FailureCount + 1).SetProperty(u => u.LockedOutOn, lockoutOn));
                break;
            default:
                await data.GetDevicesRepository().UpdateToDbAsync(n.Id, (p) => p.SetProperty(u => u.FailureCount, 0).SetProperty(u => u.LockedOutOn, (DateTime?)null));
                break;
        }
    }
}
