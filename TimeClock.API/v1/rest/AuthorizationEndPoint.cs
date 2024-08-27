using Mapster;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using TimeClock.Api.Helpers;
using TimeClock.Api.Security;
using TimeClock.Core;
using TimeClock.Core.Models.ApiDtos;
using TimeClock.Core.Models.EntityDtos;
using TimeClock.Core.Security;
using TimeClock.Data;
using TimeClock.Data.Models;
using Device = TimeClock.Data.Models.Device;
using HeaderParams = TimeClock.Core.HeaderParams;
using Location = TimeClock.Data.Models.Location;

namespace TimeClock.Api.v1.rest;

public class AuthorizationEndPoint
{
    #region API Delegates
    public static readonly Delegate VerifyConnectionDelegate = VerifyConnection;
    public static readonly Delegate CreatePublicKeyDelegate = CreatePublicKey;
    public static readonly Delegate GetRefreshTokenDelegate = GetRefreshToken;
    public static readonly Delegate RegisterDeviceDelegate = RegisterDevice;
    public static readonly Delegate ValidateClaimDelegate = ValidateClaim;
    #endregion API Delegates


    #region API Delegate Definitions
    [AllowAnonymous]
    private static async Task<Results<Ok<string>, BadRequest>> VerifyConnection(HttpContext context, IDataRepositoryFactory data, ILogger<AuthorizationEndPoint> logger,
        string deviceId, string deviceName, CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(deviceName))
            await data.GetUnregisteredDevicesRepository().AddAsync(new() { Name = deviceName }, true, cancellationToken);

        return TypedResults.Ok(deviceId + deviceName);
    }

    [AllowAnonymous]
    private static async Task<IResult> CreatePublicKey(HttpContext context, IDataRepositoryFactory data, ILogger<AuthorizationEndPoint> logger, JwtService jwt, ITimeClockCacheDataProvider cache, 
        string deviceId, string deviceName, CancellationToken cancellationToken = default)
    {
        string privateKeyName;
        string? publicKey;
        Guid deviceGuid;
        RsaCryptographyService? crypto;
        string? unregisteredDeviceName = null;
        UnregisteredDevice? uDevice = null;
        TokenPackage? tokenPackage;

        if (!Guid.TryParse(deviceId, out deviceGuid))
        {
            // log device id to audit table as fail
            logger.LogAudit<Device>($"{nameof(AuthorizationEndPoint.CreatePublicKey)} was called with invalid deviceId {deviceId} name: {deviceName}", EventIds.ParseEntityId, 
                Guid.Empty, data.GetEventAuditsRepository());
            return TypedResults.BadRequest();
        }

        privateKeyName = string.Format(CommonValues.PrivateKeyNameFormatString, deviceGuid);
        // if device does not exists in UnregisteredDevices, exit
        // if it does exist, remove it from the table, since it is being assigned a encryption key
        uDevice = (await data.GetUnregisteredDevicesRepository().GetAsync(u => u.Name == deviceName, token: cancellationToken)).FirstOrDefault();
        unregisteredDeviceName = uDevice?.Name;

        if (unregisteredDeviceName != Core.Helpers.DeviceGuidHelper.GuidToFriendly(deviceGuid))
        {
            logger.LogAudit<Device>($"{nameof(AuthorizationEndPoint.CreatePublicKey)} was called with non-match device ID to name. device name: {deviceName} id: {deviceId}", EventIds.DeviceNameInvalid,
                deviceGuid, data.GetEventAuditsRepository());
            return TypedResults.Unauthorized();
        }

        if (string.IsNullOrWhiteSpace(unregisteredDeviceName) || uDevice is null)
        {
            // log device id to audit table as fail
            logger.LogAudit<Device>($"{nameof(AuthorizationEndPoint.CreatePublicKey)} was called with nonexistent device name: {deviceName} id: {deviceId}", EventIds.DeviceNameInvalid,
                Guid.Empty, data.GetEventAuditsRepository());
            return TypedResults.BadRequest();
#warning this should return 403 Forbidden
        }

        // check if device is already setup with encryption keys.
        // if it is, credentials are required to access the info
        // in that case, return access violation and null string
        // this will be an issue if device setup started but was not finished. 
        // however, saying it is ok to pass through this API method multiple times seems like it would open up a vulnerability
        // this needs to be revisited. once a device calls this API method, it should never call again.
        // we could just return the key if we find it, but should we really be returning a encryption key just because we have a valid GUID?
        // the encryption key was created and given to that GUID already. there is nothing preventing a random joe from calling this endpoint 
        // with a valid guid to get the key. this endpoint is the initialization, nothing else. 
        if (RsaCryptographyService.IdExists(privateKeyName))
        {
            // log device id to audit table as fail
            logger.LogAudit<Device>($"{nameof(AuthorizationEndPoint.CreatePublicKey)} attempted to create existing public key for deviceId {deviceId} name: {deviceName}",
                EventIds.RsaKeyAlreadyExists, Guid.Empty, data.GetEventAuditsRepository());
            return TypedResults.BadRequest();
#warning this should return 403 Forbidden or 418 I'm a teapot
        }

        using (crypto = new RsaCryptographyService(privateKeyName))
        {
            publicKey = crypto.PublicKey;
        }

        tokenPackage = jwt.NewTokenPackage(deviceGuid, deviceName, cache);
        uDevice.RefreshToken = tokenPackage.RefreshToken;

        await data.GetUnregisteredDevicesRepository().UpdateAsync(uDevice, true);
        await cache.GetPendingDevices(true);

        context.Response.Headers.Set(HeaderParams.CustomCryptoKeyParameter, publicKey);
        context.Response.Headers.Set(HeaderParams.CustomTokenParameter, tokenPackage.Wrap());

        // log device id to audit table as success
        logger.LogInformation("{Method} created public key for deviceId {deviceId}", nameof(AuthorizationEndPoint.CreatePublicKey), deviceId);
        return TypedResults.Ok();
    }

    [AllowAnonymous]
    private static async Task<Results<Ok<Guid>, BadRequest, ForbidHttpResult>> RegisterDevice(HttpContext context, IDataRepositoryFactory data, ILogger<AuthorizationEndPoint> logger, JwtService jwt, ITimeClockCacheDataProvider cache, SecretsProvider secretsProvider,
        Guid deviceId, [FromQuery] string deviceData, CancellationToken cancellationToken = default)
    {
        Guid? departmentToLocationId;
        //string? token = await context.GetTokenAsync(HeaderParams.TokenParameter); //context.Request.Headers.GetBearer();
        string privateKeyId = string.Format(CommonValues.PrivateKeyNameFormatString, deviceId);
        ICryptographyService? crypto;
        Device device;
        RegisterDeviceDto? registerDeviceData = null;
        UnregisteredDeviceDto? unregisteredDevice;
        (int, RegisterDeviceDto?) decryptedData;
        TokenPackage tokenPackage;
        UserDto? user = null;

        decryptedData = await EndPointValidationHelpers.ValidateEncryptedPackage<RegisterDeviceDto, Device, AuthorizationEndPoint>(deviceId, deviceData, true, data, logger);

        if (decryptedData.Item1 != StatusCodes.Status200OK)
            return TypedResults.BadRequest();

        registerDeviceData = decryptedData.Item2!;

        if (registerDeviceData.DeviceId == default || string.IsNullOrWhiteSpace(registerDeviceData.DeviceName) || registerDeviceData.LocationId == default || registerDeviceData.DepartmentId == default || string.IsNullOrEmpty(registerDeviceData.RefreshToken))
        {
            logger.LogAudit<Device>($"{nameof(AuthorizationEndPoint.RegisterDevice)} was called but some data was invalid. DeviceId: {registerDeviceData.DeviceId}. DeviceName: {registerDeviceData.DeviceName}. LocationId: {registerDeviceData.LocationId}. DepartmentId: {registerDeviceData.DepartmentId}. RefreshToken: {registerDeviceData.RefreshToken}", 
                EventIds.IncompleteOrBadData,
                Guid.Empty, data.GetEventAuditsRepository());
            return TypedResults.BadRequest();
        }

        if (string.IsNullOrWhiteSpace(registerDeviceData.Username) || string.IsNullOrWhiteSpace(registerDeviceData.Password) ||
            (user = (await EndPointValidationHelpers.ValidateUserCredentials<Device, AuthorizationEndPoint>(registerDeviceData.Username, registerDeviceData.Password, PunchTypeDto.Domain, data,logger, secretsProvider, registerDeviceData.EmployeeId.ToString(), cancellationToken))?.Adapt<UserDto>()) is null)
        {
            await EndPointValidationHelpers.UpdateUserFailure(registerDeviceData.Username, true, data, secretsProvider);
            return TypedResults.Forbid();
        }

        await EndPointValidationHelpers.UpdateUserFailure(registerDeviceData.Username, false, data, secretsProvider);

        departmentToLocationId = (await data.GetDepartmentsToLocationsRepository().GetAsync(t => t.DepartmentId == registerDeviceData.DepartmentId && t.LocationId == registerDeviceData.LocationId, token: cancellationToken))
            .FirstOrDefault()?.Id;

        if (!departmentToLocationId.HasValue)
        {
            logger.LogAudit<Device>($"{nameof(AuthorizationEndPoint.RegisterDevice)} was called but departmentToLocationId does not exists. department: {registerDeviceData.DepartmentId} location: {registerDeviceData.LocationId}", 
                EventIds.EntityNotFound, Guid.Empty, data.GetEventAuditsRepository());
            return TypedResults.BadRequest();
        }

        unregisteredDevice = (await cache.GetPendingDevices(refresh: true)).FirstOrDefault(d => d.Name == registerDeviceData.DeviceName);

        if (unregisteredDevice is null)
        {
            logger.LogAudit<Device>($"{nameof(AuthorizationEndPoint.RegisterDevice)} was called but the specified unregistered device was not found. deviceName: {registerDeviceData.DeviceName}. deviceId: {registerDeviceData.DeviceId}", 
                EventIds.DeviceNameInvalid, Guid.Empty, data.GetEventAuditsRepository());
            return TypedResults.Forbid();
        }

        if (unregisteredDevice.RefreshToken != registerDeviceData.RefreshToken)
        {
            logger.LogAudit<Device>($"{nameof(AuthorizationEndPoint.RegisterDevice)} was called but the provided refreshToken {registerDeviceData.RefreshToken} does not match existing {unregisteredDevice.RefreshToken}. deviceName: {registerDeviceData.DeviceName}. deviceId: {registerDeviceData.DeviceId}", 
                EventIds.Jwt, Guid.Empty, data.GetEventAuditsRepository());
            return TypedResults.Forbid();
        }

        // generate new jwt & refresh
        tokenPackage = jwt.NewTokenPackage(registerDeviceData.DeviceId, registerDeviceData.DeviceName, cache);   
        // save device with refresh to db
        device = new Device()
        {
            Id = registerDeviceData.DeviceId, Name = registerDeviceData.DeviceName, DepartmentsToLocationsId = departmentToLocationId.Value, 
            RefreshToken = tokenPackage.RefreshToken, RefreshTokenExpiration = tokenPackage.Expiration, ConfiguredById = user.Id, LastActionOn = DateTime.Now, 
            IsPublic = registerDeviceData.IsPublic,
        };
        // update geo data for location if its available and needed
        if (registerDeviceData.Latitude.HasValue || registerDeviceData.Longitude.HasValue)
        {
            Location? location = await data.GetLocationsRepository().GetAsync(registerDeviceData.LocationId);
            if (location is not null)
            {
                if (!location.Latitude.HasValue && registerDeviceData.Latitude.HasValue)
                    location.Latitude = registerDeviceData.Latitude;
                if (!location.Longitude.HasValue && registerDeviceData.Longitude.HasValue)
                    location.Longitude = registerDeviceData.Longitude;
            }
        }
        // save device to db - do not save operation
        await data.GetDevicesRepository().AddAsync(device, false);         
        // delete unregistered from db - do not save operation
        data.GetUnregisteredDevicesRepository().Delete(unregisteredDevice.Id, false);
        // call to save all operations to db in 1 shot
        await data.GetDevicesRepository().SaveAsync();
        // update cache
        await cache.GetDevices(true);
        await cache.GetPendingDevices(true);
        // return jwt
        context.Response.Headers.Set(HeaderParams.CustomTokenParameter, tokenPackage.Wrap());
        return TypedResults.Ok(user.Id);
    }


    [AllowExpiredJwt]
    private static async Task<Results<Ok, BadRequest, ForbidHttpResult, IResult>> GetRefreshToken(HttpContext context, IDataRepositoryFactory data, ILogger<AuthorizationEndPoint> logger, JwtService jwt, ITimeClockCacheDataProvider cache, SecretsProvider secretsProvider, 
        Guid deviceId, [FromQuery] string getRefreshTokenData, CancellationToken cancellationToken)
    {
        string? token = await context.GetTokenAsync(HeaderParams.TokenParameter);
        string privateKeyName;
        Device? device = null;
        DeviceDto? deviceDto = null;
        TokenPackage? tokenPackage = null;
        GetRefreshTokenDto? refreshTokenDto = null;
        RsaCryptographyService crypto;
        bool needsNewRefresh = false;
        JwtSecurityToken jwtTokenObject;

        if (token is null)
        {
            logger.LogAudit<Device>($"{nameof(AuthorizationEndPoint.GetRefreshToken)} was called but JWT was null. DeviceId: {deviceId}", EventIds.Jwt,
                Guid.Empty, data.GetEventAuditsRepository());
            return TypedResults.Forbid();
        }

        privateKeyName = string.Format(CommonValues.PrivateKeyNameFormatString, deviceId);

        if (!RsaCryptographyService.IdExists(privateKeyName))
        {
            logger.LogAudit<Device>($"{nameof(AuthorizationEndPoint.GetRefreshToken)} attempted to get not found key container for deviceId {deviceId}. container key: {privateKeyName}",
                EventIds.CryptoTryGet, Guid.Empty, data.GetEventAuditsRepository());
            return TypedResults.Forbid();
        }

        using (crypto = new RsaCryptographyService(privateKeyName))
        {
            refreshTokenDto = GetRefreshTokenDto.Unwrap(getRefreshTokenData, cryptographyService: crypto);
        }

        if (refreshTokenDto is null || string.IsNullOrWhiteSpace(refreshTokenDto.RefreshToken))
        {
            logger.LogAudit<Device>($"{nameof(AuthorizationEndPoint.GetRefreshToken)} was called with getRefreshTokenData which failed to unwrap. deviceId: {deviceId}. getRefreshTokenData: {getRefreshTokenData}", EventIds.Jwt,
                Guid.Empty, data.GetEventAuditsRepository());
            return TypedResults.BadRequest();
        }
        

        // force refresh devices to make sure we have the latest refresh token
        (await cache.GetDevices(true)).TryGetValue(deviceId, out deviceDto);// await cache.GetDevice(deviceId);

        if (deviceDto is null)
        {
            logger.LogAudit<Device>($"{nameof(AuthorizationEndPoint.GetRefreshToken)} was called with invalid deviceId {deviceId}. No such device exists", EventIds.GetDevice,
                Guid.Empty, data.GetEventAuditsRepository());
            return TypedResults.BadRequest();
        }

        if (deviceDto.RefreshToken != refreshTokenDto.RefreshToken)
        {
            logger.LogAudit<Device>($"{nameof(AuthorizationEndPoint.GetRefreshToken)} was called with invalid/mismatched RefreshToken. token submitted: {refreshTokenDto.RefreshToken}. token found: {deviceDto.RefreshToken}", EventIds.Jwt,
                Guid.Empty, data.GetEventAuditsRepository());
            return TypedResults.Forbid();
        }

        // if our refresh token expired, we need valid credentials to generate new tokens
        if (deviceDto.RefreshTokenExpiration < DateTime.UtcNow)
        {
            if (string.IsNullOrWhiteSpace(refreshTokenDto.UserName) || string.IsNullOrWhiteSpace(refreshTokenDto.Password))
            {
                logger.LogAudit<Device>($"{nameof(AuthorizationEndPoint.GetRefreshToken)} was called with expired RefreshToken, but no credentials. tokenExpiration: {deviceDto.RefreshTokenExpiration}. token: {deviceDto.RefreshToken}", EventIds.Jwt,
                    Guid.Empty, data.GetEventAuditsRepository());
                return TypedResults.BadRequest();
            }

            if (!ActiveDirectoryHelper.AuthenticateWithEntry(refreshTokenDto.UserName, refreshTokenDto.Password, data, logger))
            {
                await EndPointValidationHelpers.UpdateUserFailure(refreshTokenDto.UserName, true, data, secretsProvider);
                
                logger.LogAudit<Device>($"{nameof(AuthorizationEndPoint.GetRefreshToken)} was called with expired RefreshToken, and invalid credentials. user: {refreshTokenDto.UserName}. password: {refreshTokenDto.Password}. tokenExpiration: {deviceDto.RefreshTokenExpiration}. token: {deviceDto.RefreshToken}", EventIds.User,
                    Guid.Empty, data.GetEventAuditsRepository());
                return TypedResults.Forbid();
            }

            await EndPointValidationHelpers.UpdateUserFailure(refreshTokenDto.UserName, false, data, secretsProvider);

            needsNewRefresh = true;
        }

        // if the refresh token was issued more than 24 hours ago, create a new refresh
        // if the refresh token will expire in the next 24 hours, create a new refresh
        if ((deviceDto.RefreshTokenIssuedOn.HasValue && (DateTime.UtcNow.Subtract(deviceDto.RefreshTokenIssuedOn.Value).TotalDays > 1)) || deviceDto.RefreshTokenExpiration < DateTime.UtcNow.AddDays(1).EndOfDay())
            needsNewRefresh = true;

        // generate new refresh token and new JWT
        if (needsNewRefresh)
            tokenPackage = jwt.GetRefresh(token, refreshTokenDto.RefreshToken, deviceDto, cache);
        else
        {
            // new refresh token not needed, so just generate new JWT
            jwtTokenObject = jwt.NewToken(deviceDto.Id, deviceDto.Name);
            RefreshToken refresh = RefreshToken.Create(jwtTokenObject.ValidTo.Add(JwtService.RefreshExpiration!.Value));
            tokenPackage = new TokenPackage(
                new CustomValuesJwtTokenHandler(cache, secretsProvider).WriteToken(jwtTokenObject),
                jwtTokenObject.ValidTo, refresh.Value, refresh.Expiration);
        }

        if (tokenPackage is null)
        {
            //when JWT or RefreshToken failed to generate due to JWT Principal is null or RefreshToken mismatch
            logger.LogAudit<Device>($"{nameof(AuthorizationEndPoint.GetRefreshToken)} was called but JWT refresh token failed to generate. token:{token}. refresh token:{refreshTokenDto.RefreshToken}. deviceId: {deviceDto.Id}. deviceName: {deviceDto.Name}", EventIds.Jwt,
                Guid.Empty, data.GetEventAuditsRepository());
            return TypedResults.Forbid();
        }

        // we created a new refresh token, so we need to update the db and the cache
        if (needsNewRefresh)
        {
            try
            {
                device = await data.GetDevicesRepository().GetAsync(deviceId, token: cancellationToken);
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            if (device is null) return TypedResults.Forbid(); // this may happen if client disconnects, through cancellation token

            device.RefreshToken = tokenPackage.RefreshToken;
            device.RefreshTokenExpiration = tokenPackage.Expiration;

            await data.GetDevicesRepository().UpdateAsync(device, true);
            // refresh/repopulate cache
            await cache.GetPendingDevices(true);
            await cache.GetDevices(true);
        }

        // this is pretty ugly. the db will already have updated the refresh token, but the client will not have the token
        // which means the client will not be able to get authenticated on subsequent requests
        //if (cancellationToken.IsCancellationRequested)
        //    return TypedResults.BadRequest();

        // set and return jwt
        context.Response.Headers.Set(HeaderParams.CustomTokenParameter, tokenPackage.Wrap());

        return TypedResults.Ok();
    }


    private static async Task<IResult> ValidateClaim(HttpContext context, IDataRepositoryFactory data, ILogger<AuthorizationEndPoint> logger, JwtService jwt, ITimeClockCacheDataProvider cache, SecretsProvider secretsProvider,
        Guid deviceId, [FromQuery] string validateClaimData, CancellationToken cancellationToken)
    {
        ValidateClaimDto? validateClaimDto;
        string privateKeyId = string.Format(CommonValues.PrivateKeyNameFormatString, deviceId);
        ICryptographyService? crypto;
        UserDto? user;
        (int, ValidateClaimDto?) decryptValidation;

        decryptValidation = await EndPointValidationHelpers.ValidateEncryptedPackage<ValidateClaimDto, Device, AuthorizationEndPoint>(deviceId, validateClaimData, true, data,logger);

        if (decryptValidation.Item1 != StatusCodes.Status200OK)
            return TypedResults.Forbid();
        
        validateClaimDto = decryptValidation.Item2;

        if (string.IsNullOrWhiteSpace(validateClaimDto!.UserName) || validateClaimDto.Claims == default || validateClaimDto.PunchTypeDto == default)
        {
            logger.LogAudit<Device>($"{nameof(AuthorizationEndPoint.RegisterDevice)} was called but some data was invalid. DeviceId: {validateClaimDto.DeviceId}. UserName: {validateClaimDto.UserName}. ClaimType: {validateClaimDto.Claims}. PunchTypeDto: {validateClaimDto.PunchTypeDto}.",
                EventIds.IncompleteOrBadData,
                Guid.Empty, data.GetEventAuditsRepository());
            return TypedResults.Forbid();
        }

        if (!EndPointValidationHelpers.ValidateDeviceGuid(deviceId, validateClaimDto.DeviceId, data, logger, secretsProvider))
            return TypedResults.Forbid();

        user = (await EndPointValidationHelpers.ValidateUserCredentials<User, AuthorizationEndPoint>(validateClaimDto.UserName, validateClaimDto.Password, 
            validateClaimDto.PunchTypeDto, data, logger, secretsProvider, cancellationToken:cancellationToken))?.Adapt<UserDto>();

#warning need to increment user failure and log
        if (user == null)
            return TypedResults.Forbid();
        // check if the claim being requested does not need any special claim, otherwise check if the user has the requested request claim 
        if (!validateClaimDto.Claims.Any(c => c == Core.Models.AuthorizationClaimType.Unknown) &&
            !user.UserClaims.Any(c => validateClaimDto.Claims.Any(v => c.AuthorizationClaim.Type.Equals(Enum.GetName(v), StringComparison.CurrentCultureIgnoreCase))))
            return TypedResults.Forbid();

        return TypedResults.Ok(user);
    }
    #endregion API Delegate Definitions
}

