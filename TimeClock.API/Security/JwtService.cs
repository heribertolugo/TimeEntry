using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Security.Cryptography;
using TimeClock.Api.Helpers;
using TimeClock.Api.v1.rest;
using TimeClock.Core;
using TimeClock.Core.Models.ApiDtos;
using TimeClock.Core.Models.EntityDtos;
using TimeClock.Core.Security;
using TimeClock.Data;
using Device = TimeClock.Data.Models.Device;
using HeaderParams = TimeClock.Core.HeaderParams;

namespace TimeClock.Api.Security;

internal sealed class JwtService
{
    public static TimeSpan? ClockSkew { get; private set; }
    public static TimeSpan? Expiration { get; private set; }
    public static TimeSpan? RefreshExpiration { get; private set; }
    private static readonly SemaphoreSlim ClockScrewSemaphore = new(1, 1);
    private static readonly SemaphoreSlim ExpirationSemaphore = new(1, 1);
    private static readonly SemaphoreSlim RefreshExpirationSemaphore = new(1, 1);

    private SecretsProvider SecretsProvider { get; init; }

    public JwtService(SecretsProvider secretsProvider)
    {
        this.SecretsProvider = secretsProvider;
        this.SetTimedValues();
    }

    private void SetTimedValues()
    {
        if (JwtService.ClockSkew is null)
        {
            JwtService.ClockScrewSemaphore.Wait();
            JwtService.ClockSkew = this.SecretsProvider.GetJwtClockScrew();
            JwtService.ClockScrewSemaphore.Release();
        }
        if (JwtService.Expiration is null)
        {
            JwtService.ExpirationSemaphore.Wait();
            JwtService.Expiration = this.SecretsProvider.GetJwtTokenExpiration();
            JwtService.ExpirationSemaphore.Release();
        }
        if (JwtService.RefreshExpiration is null)
        {
            JwtService.RefreshExpirationSemaphore.Wait();
            JwtService.RefreshExpiration = this.SecretsProvider.GetJwtRefreshExpiration();
            JwtService.RefreshExpirationSemaphore.Release();
        }
    }

    public JwtSecurityToken NewToken(Guid id, string name)
    {
        IEnumerable<Claim> claims =
        [
            new(ClaimTypes.NameIdentifier, id.ToString()),
            new(ClaimTypes.Name, name),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        ];
        byte[]? secret = this.SecretsProvider.GetIssuerSigningKey() ?? throw new SecurityTokenException("Secret was not found");
        SymmetricSecurityKey key = new SymmetricSecurityKey(secret);
        JwtSecurityToken token = new JwtSecurityToken(
            issuer: this.SecretsProvider.GetJwtIssuer(),
            audience: $"{this.SecretsProvider.JwtAudience}{id:N}",
            expires: DateTime.UtcNow.Add(JwtService.Expiration!.Value), // if Expiration doesnt have value, something is wrong. crash the app
            claims: claims,
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );
        return token;
    }

    public TokenPackage NewTokenPackage(Guid guid, string name, ITimeClockCacheDataProvider cache)
    {
        JwtSecurityToken token = this.NewToken(guid, name);
        RefreshToken refresh =  RefreshToken.Create(token.ValidTo.Add(JwtService.RefreshExpiration!.Value)); // if RefreshExpiration doesnt have value, something is wrong. crash the app
        return new TokenPackage(new CustomValuesJwtTokenHandler(cache, this.SecretsProvider).WriteToken(token), token.ValidTo,
            refresh.Value, refresh.Expiration);
    }

    /// <summary>
    /// Creates a new JWT token and writes the token into a string.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    /// <returns>The JWT token as a string</returns>
    public string NewTokenString(Guid id, string name, ITimeClockCacheDataProvider cache)
    {            
        return new CustomValuesJwtTokenHandler(cache, this.SecretsProvider).WriteToken(this.NewToken(id, name));
    }

    public TokenPackage? GetRefresh(string expiredAcessToken, string expiredRefreshToken, DeviceDto device, ITimeClockCacheDataProvider cache)
    {
        // device really shouldnt be passed in, to prevent tight coupling
        ClaimsPrincipal? principal = this.GetClaimsPrincipalFromExpiredToken(expiredAcessToken, device.Id, cache);

        if (principal?.Identity?.Name is null)
            return null;

        if (device is null || device.RefreshToken != expiredRefreshToken) // || device.RefreshTokenExpiration < DateTime.Now
            return null;

        JwtSecurityToken token = this.NewToken(device.Id, device.Name);
        RefreshToken refresh = RefreshToken.Create(token.ValidTo.Add(JwtService.RefreshExpiration!.Value)); // if RefreshExpiration doesnt have value, something is wrong. crash the app

        return new TokenPackage(new CustomValuesJwtTokenHandler(cache, this.SecretsProvider).WriteToken(token), token.ValidTo, refresh.Value, refresh.Expiration);
    }

    public ClaimsPrincipal? GetClaimsPrincipalFromExpiredToken(string? token, Guid deviceId, ITimeClockCacheDataProvider cache)
    {
        TokenValidationParameters validation = new TokenValidationParameters()
        {                
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = false,
            ValidateIssuer = true,

            ValidIssuer = this.SecretsProvider.GetJwtIssuer(),
            ValidAudience = $"{this.SecretsProvider.JwtAudience}{deviceId:N}",
            IssuerSigningKey = new SymmetricSecurityKey(this.SecretsProvider.GetIssuerSigningKey()),
        };
        SecurityToken securityToken;
        ClaimsPrincipal principal = new CustomValuesJwtTokenHandler(cache, this.SecretsProvider).ValidateToken(token, validation, out securityToken);
        JwtSecurityToken? jwtSecurityToken =  securityToken as JwtSecurityToken;
        
        if (jwtSecurityToken is null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid Token");

        return principal;
    }

    public static JwtSecurityToken? GetFromRequest(HttpRequest httpRequest)
    {
        IHeaderDictionary headers = httpRequest.Headers;
        
        string? jwtString = headers.GetBearer();

        if (jwtString is null) return null;

        return new JwtSecurityToken(jwtString);
    }

    /// <summary>
    /// Refreshes the JWT and the RefreshToken when called. 
    /// Will also update the database with RefreshToken and insert the JWT into response headers. 
    /// Will attempt to authenticate credentials against domain if RefeshToken has expired. 
    /// Null is returned if JWT or RefreshToken cannot be refreshed or authenticated. 
    /// AuditLog will be used whenever a null will be returned.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="data"></param>
    /// <param name="logger"></param>
    /// <param name="jwt"></param>
    /// <param name="cache"></param>
    /// <param name="deviceId"></param>
    /// <param name="getRefreshTokenData"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="caller"></param>
    /// <returns></returns>
    public async Task<TokenPackage?> DoRefresh(HttpContext context, IDataRepositoryFactory data, ILogger<AuthorizationEndPoint> logger, JwtService jwt, ITimeClockCacheDataProvider cache,
        Guid deviceId, string getRefreshTokenData, CancellationToken cancellationToken, [CallerMemberName] string? caller = null)
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
            logger.LogAudit<Device>($"{caller} was called but JWT was null. DeviceId: {deviceId}", EventIds.Jwt,
                Guid.Empty, data.GetEventAuditsRepository());
            return null;
        }

        privateKeyName = string.Format(CommonValues.PrivateKeyNameFormatString, deviceId);

        if (!RsaCryptographyService.IdExists(privateKeyName))
        {
            logger.LogAudit<Device>($"{caller} attempted to get not found key container for deviceId {deviceId}. container key: {privateKeyName}",
                EventIds.CryptoTryGet, Guid.Empty, data.GetEventAuditsRepository());
            return null;
        }

        using (crypto = new RsaCryptographyService(privateKeyName))
        {
            refreshTokenDto = GetRefreshTokenDto.Unwrap(getRefreshTokenData, cryptographyService: crypto);
        }

        if (refreshTokenDto is null || string.IsNullOrWhiteSpace(refreshTokenDto.RefreshToken))
        {
            logger.LogAudit<Device>($"{caller} was called with getRefreshTokenData which failed to unwrap. deviceId: {deviceId}. getRefreshTokenData: {getRefreshTokenData}", EventIds.Jwt,
                Guid.Empty, data.GetEventAuditsRepository());
            return null;
        }

        deviceDto = await cache.GetDevice(deviceId);

        if (deviceDto is null)
        {
            logger.LogAudit<Device>($"{caller} was called with invalid deviceId {deviceId}. No such device exists", EventIds.GetDevice,
                Guid.Empty, data.GetEventAuditsRepository());
            return null;
        }

        if (deviceDto.RefreshToken != refreshTokenDto.RefreshToken)
        {
            logger.LogAudit<Device>($"{caller} was called with invalid/mismatched RefreshToken. token submitted: {refreshTokenDto.RefreshToken}. token found: {deviceDto.RefreshToken}", EventIds.Jwt,
                Guid.Empty, data.GetEventAuditsRepository());
            return null;
        }

        // if our refresh token expired, we need valid credentials to generate new tokens
        if (deviceDto.RefreshTokenExpiration < DateTime.UtcNow)
        {
            if (string.IsNullOrWhiteSpace(refreshTokenDto.UserName) || string.IsNullOrWhiteSpace(refreshTokenDto.Password))
            {
                logger.LogAudit<Device>($"{caller} was called with expired RefreshToken, but no credentials. tokenExpiration: {deviceDto.RefreshTokenExpiration}. token: {deviceDto.RefreshToken}", EventIds.Jwt,
                    Guid.Empty, data.GetEventAuditsRepository());
                return null;
            }

            // user credentials wrong 
            if (!ActiveDirectoryHelper.AuthenticateWithEntry(refreshTokenDto.UserName, refreshTokenDto.Password, data, logger))
            {
                logger.LogAudit<Device>($"{caller} was called with expired RefreshToken, and invalid credentials. user: {refreshTokenDto.UserName}. password: {refreshTokenDto.Password}. tokenExpiration: {deviceDto.RefreshTokenExpiration}. token: {deviceDto.RefreshToken}", EventIds.User,
                    Guid.Empty, data.GetEventAuditsRepository());
                return null;
            }

            needsNewRefresh = true;
        }

        // if the refresh token was issued more than 24 hours ago, create a new refresh
        // if the refresh token will expire in the next 24 hours, create a new refresh
        if (deviceDto.RefreshTokenIssuedOn.HasValue && (DateTime.UtcNow.Subtract(deviceDto.RefreshTokenIssuedOn.Value).TotalDays > 1) ||  deviceDto.RefreshTokenExpiration < DateTime.UtcNow.AddDays(1).EndOfDay())
            needsNewRefresh = true;

        // genereate new refresh token and new JWT
        if (needsNewRefresh)
            tokenPackage = jwt.GetRefresh(token, refreshTokenDto.RefreshToken, deviceDto, cache);
        else
        {
            // new refesh token not needed, so just generate new JWT
            jwtTokenObject = this.NewToken(deviceDto.Id, deviceDto.Name);
            RefreshToken refresh = RefreshToken.Create(jwtTokenObject.ValidTo.Add(JwtService.RefreshExpiration!.Value));
            tokenPackage = new TokenPackage(
                new CustomValuesJwtTokenHandler(cache, this.SecretsProvider).WriteToken(jwtTokenObject),
                jwtTokenObject.ValidTo, refresh.Value, refresh.Expiration);
        }

        if (tokenPackage is null)
        {
            //when JWT or RefreshToken failed to generate due to JWT Principal is null or RefreshToken mismatch
            logger.LogAudit<Device>($"{caller} was called but JWT refresh token failed to generate. token:{token}. refresh token:{refreshTokenDto.RefreshToken}. deviceId: {deviceDto.Id}. deviceName: {deviceDto.Name}", EventIds.Jwt,
                Guid.Empty, data.GetEventAuditsRepository());
            return null;
        }

        // we created a new refesh token, so we need to update the db and the cache
        if (needsNewRefresh)
        {
            device = await data.GetDevicesRepository().GetAsync(deviceId, token: cancellationToken);

            if (device is null) return null; // this may happen if client disconnects, through cancellation token

            device.RefreshToken = tokenPackage.RefreshToken;
            device.RefreshTokenExpiration = tokenPackage.Expiration;

            await data.GetDevicesRepository().UpdateAsync(device, true);
            // refresh/repopulate cache
            await cache.GetPendingDevices(true);
            await cache.GetDevices(true);
        }

        // this is pretty ugly. the db will already have updated the refresh token, but the client will not have the token
        // which means the client will not be able to get authenticated on subseuquent requests
        if (cancellationToken.IsCancellationRequested) 
            return null;

        // set and return jwt
        context.Response.Headers.Set(HeaderParams.CustomTokenParameter, tokenPackage.Wrap());

        return tokenPackage;
    }
}

/// <summary>
/// Structure which contains values for a JWT refresh token.
/// </summary>
/// <param name="Value"></param>
/// <param name="Expiration"></param>
internal sealed record class RefreshToken(string Value, DateTime Expiration)
{
    /// <summary>
    /// Creates a new refresh token.
    /// </summary>
    /// <param name="expiration"></param>
    /// <returns></returns>
    public static RefreshToken Create(DateTime expiration)
    {
        byte[] randomNumber = new byte[64];
        using RandomNumberGenerator generator = RandomNumberGenerator.Create();

        generator.GetBytes(randomNumber);

        return new RefreshToken(Convert.ToBase64String(randomNumber), expiration); // if RefreshExpiration doesnt have value, something is wrong. crash the app
    }
}
