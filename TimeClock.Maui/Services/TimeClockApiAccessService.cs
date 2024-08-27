using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using TimeClock.Core;
using TimeClock.Core.Models.ApiDtos;
using TimeClock.Core.Models.EntityDtos;
using TimeClock.Core.Security;
using TimeClock.Maui.Helpers;

namespace TimeClock.Maui.Services;

internal interface ITimeClockApiAccessService
{
    string ApiPublicKey { get; set; }
    string ApiSecret { get; set; }
    string ApiToken { get; set; }
    Guid DeviceId { get; }
    string EndPoint { get; set; }
    TimeSpan TimeOut { get; set; }

    #region Administrative
    Task<string> TestConnection(Guid deviceId, string deviceName, CancellationToken cancellationToken = default);
    Task<ResultValues<string?>> GetKeys(Guid deviceId, string deviceName, CancellationToken cancellationToken = default);
    /// <summary>
    /// Registers a device and returns the ID of the user who registered the device.
    /// </summary>
    /// <param name="cryptoKey"></param>
    /// <param name="jwtToken"></param>
    /// <param name="device"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ResultValues<Guid>> RegisterDevice(string cryptoKey, string jwtToken, RegisterDeviceDto device, CancellationToken cancellationToken = default);
    Task<ResultValues<string?>> RefreshJwt(Guid deviceId, string cryptoKey, GetRefreshTokenDto data, CancellationToken cancellationToken = default);
    Task<ResultValues<UserDto?>> RequestAccess(Guid deviceId, string cryptoKey, ValidateClaimDto data, CancellationToken cancellationToken = default);

    #endregion Administrative

    #region Equipment
    Task<ResultValues<IEnumerable<EquipmentDto>?>> GetEquipment(Guid deviceId, GetEquipmentsDto data, CancellationToken cancellationToken = default);
    Task<ResultValues<int>> GetEquipmentCount(Guid deviceId, GetEquipmentsDto data, CancellationToken cancellationToken = default);

    #endregion Equipment

    #region Location
    Task<IEnumerable<LocationDto>> GetLocations(Guid deviceId, CancellationToken cancellationToken = default);
    Task<ResultValues<LocationDto?>> GetLocation(Guid deviceId, Guid locationId, CancellationToken cancellationToken = default);

    #endregion Location

    #region Department
    Task<IEnumerable<DepartmentDto>> GetDepartments(Guid deviceId);

    #endregion Department

    #region Punch Entry
    Task<ResultValues<PunchEntryDto?>> CreatePunchEntry(Guid deviceId, string cryptoKey, CreatePunchEntryDto data, CancellationToken cancellationToken = default);
    Task<ResultValues<PunchEntryDto?>> UpdatePunchEntry(Guid deviceId, string cryptoKey, CreatePunchEntryDto data, CancellationToken cancellationToken = default);
    Task<ResultValues<IEnumerable<PunchEntryDto>?>> GetPunches(Guid deviceId, GetPunchEntriesDto data, CancellationToken cancellationToken = default);

    #endregion Entry

    #region User
    Task<ResultValues<EquipmentsToUserDto?>> LinkUserToEquipment(Guid deviceId, string cryptoKey, UpdateEquipmentToUserDto data, CancellationToken cancellationToken = default);
    /// <summary>
    /// The API will use <see cref="UpdateEquipmentToUserDto.EquipmentToUserId"/> in <paramref name="data"/> to unlink. 
    /// The <see cref="UpdateEquipmentToUserDto.UserId"/> will not be considered when unlinking Equipment.
    /// </summary>
    /// <param name="deviceId"></param>
    /// <param name="cryptoKey"></param>
    /// <param name="data"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ResultValues<EquipmentsToUserDto?>> UnlinkUserToEquipment(Guid deviceId, string cryptoKey, UpdateEquipmentToUserDto data, CancellationToken cancellationToken = default);
    Task<ResultValues<UserDto?>> GetUser(Guid deviceId, Guid userId, GetUserDto data);
    Task<ResultValues<IEnumerable<UserDto>?>> GetUsers(Guid deviceId, GetUsersDto data);
    Task<ResultValues<UserDto?>> UpdateUser(Guid deviceId, UpdateUserDto data, CancellationToken cancellationToken = default);
    Task<ResultValues<IEnumerable<JobTypeStepToUserDto>?>> GetUserJobTypeSteps(Guid deviceId, string cryptoKey, GetUserJobTypeStepsDto data, CancellationToken cancellationToken = default);

    #endregion User

    #region Device
    Task<ResultValues<DeviceDto?>> GetDevice(Guid deviceId, GetDeviceDto data, string cryptoKey, CancellationToken cancellationToken = default);

    #endregion Device

    #region Work Days
    Task<ResultValues<IEnumerable<WorkPeriodDto>?>> GetWorkPeriods(Guid deviceId, GetWorkPeriodsDto data, string cryptoKey, CancellationToken cancellationToken = default);
    #endregion Work Days

    #region Job Type Steps
    Task<ResultValues<IEnumerable<JobTypeDto>?>> GetJobTypes(Guid deviceId, GenericGetDto data, string cryptoKey, CancellationToken cancellationToken = default);
    Task<ResultValues<IEnumerable<JobStepDto>?>> GetJobSteps(Guid deviceId, GenericGetDto data, string cryptoKey, CancellationToken cancellationToken = default);
    #endregion Job Type Steps
}

internal sealed class TimeClockApiAccessService : ITimeClockApiAccessService
{
    private IHttpClientFactory _httpFactory;
    private HttpClient _httpClient;
    private string _endPoint = string.Empty;
    private string ApiSubPath { get; set; } = "v1/rest/";
    private static readonly int TimeoutSeconds = 50;
    private ILogger<TimeClockApiAccessService> Logger { get; init; }

    public TimeClockApiAccessService(TimeClockApiAccessSettings settings, IHttpClientFactory httpFactory, ILogger<TimeClockApiAccessService> logger)
        : this(settings.Endpoint, settings.ApiPublicKey, string.Empty, settings.DeviceId, httpFactory, logger) { }
    public TimeClockApiAccessService(string endPoint, string apiPublicKey, string apiSecret, Guid deviceId, IHttpClientFactory httpFactory, ILogger<TimeClockApiAccessService> logger)
    {
        this._httpFactory = httpFactory;

        if (!string.IsNullOrWhiteSpace(endPoint))
        {
            this.EndPoint = endPoint;
        }
        else if (!string.IsNullOrWhiteSpace(Settings.ApiEndpoint))
        {
            this.EndPoint = Settings.ApiEndpoint;
        }
        //this.EndPoint = endPoint;
        this.ApiPublicKey = apiPublicKey;
        this.ApiSecret = apiSecret;
        this.DeviceId = deviceId;
        this.Logger = logger;
    }


    #region Public Properties
    /// <summary>
    /// Must be read after EndPoint has been assigned. Otherwise an exception will be thrown.
    /// </summary>
    public TimeSpan TimeOut
    {
        get => this._httpClient.Timeout;
        set
        {
            this._httpClient = this._httpFactory.CreateClient(this._endPoint);
            this._httpClient.Timeout = value == default ? TimeSpan.FromSeconds(TimeClockApiAccessService.TimeoutSeconds) : value;
            this._httpClient.BaseAddress = new Uri(this.EndPoint);
        }
    }
    public string EndPoint
    {
        get => this._endPoint;
        set
        {
            if (string.IsNullOrWhiteSpace(value) || !Uri.IsWellFormedUriString(value, UriKind.Absolute) || this._httpFactory is null)
                return;
            this._endPoint = value;
            this._httpClient = this._httpFactory.CreateClient(value);
            this._httpClient.Timeout = TimeSpan.FromSeconds(TimeClockApiAccessService.TimeoutSeconds);
            this._httpClient.BaseAddress = new Uri(this.EndPoint);
        }
    }
    public string ApiPublicKey { get; set; }
    public string ApiSecret { get; set; }
    public string ApiToken { get; set; } = string.Empty;

    public Guid DeviceId { get; init; }
    #endregion Public Properties

    #region Public Methods

    #region Administrative
    public async Task<ResultValues<Guid>> RegisterDevice(string cryptoKey, string jwtToken, RegisterDeviceDto device, CancellationToken cancellationToken = default)
    {
        using RsaCryptographyService crypto = new(cryptoKey, null);
        string deviceData = device.Wrap(true, crypto);

        return await this.PostApi<Guid>($"authorization/registerdevice/{device.DeviceId:N}?{nameof(deviceData)}={deviceData}", null, cancellationToken: cancellationToken);
    }
    public async Task<string> TestConnection(Guid deviceId, string deviceName, CancellationToken cancellationToken = default)
    {
        return (await this.GetApiStringResponse($"authorization/verify/{deviceId:N}/{deviceName}", false, cancellationToken: cancellationToken))?.Value ?? string.Empty;
    }
    public async Task<ResultValues<string?>> GetKeys(Guid deviceId, string deviceName, CancellationToken cancellationToken = default)
    {
        return await this.GetApiStringResponse($"authorization/{deviceId:N}/{deviceName}", false, cancellationToken);
    }
    public async Task<ResultValues<string?>> RefreshJwt(Guid deviceId, string cryptoKey, GetRefreshTokenDto data, CancellationToken cancellationToken = default)
    {
        using RsaCryptographyService crypto = new(cryptoKey, null);
        string value = data.Wrap(true,crypto);
        return await this.GetApiStringResponse($"authorization/refreshtoken/{deviceId:N}?getRefreshTokenData={value}", cancellationToken: cancellationToken);
    }
    public async Task<ResultValues<UserDto?>> RequestAccess(Guid deviceId, string cryptoKey, ValidateClaimDto data, CancellationToken cancellationToken = default)
    {
        using RsaCryptographyService crypto = new(cryptoKey, null);
        string value = data.Wrap(true, crypto);
        ResultValues<UserDto?> result = await this.GetApiObjectResponse<UserDto?>($"authorization/validateclaim/{deviceId:N}?validateClaimData={value}", cancellationToken: cancellationToken);

        if (result.StatusCode == HttpStatusCode.Unauthorized)
        {
            string? jwt = await Settings.GetJwtToken();
            if (this._httpClient.DefaultRequestHeaders.Authorization is null && jwt is not null)
                this._httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(Core.HeaderParams.TokenTypePrefix, jwt);
            string? refreshToken = await Settings.GetJwtRefresh();
            if (!string.IsNullOrWhiteSpace(refreshToken))
            {
                ResultValues<string?> refresh = await this.RefreshJwt(deviceId, cryptoKey, new GetRefreshTokenDto()
                {
                    DeviceId = deviceId,
                    Password = data.Password,
                    UserName = data.UserName,
                    RefreshToken = refreshToken
                });

                if (refresh.Token is not null)
                {
                    await Settings.SetJwtRefresh(refresh.Token.RefreshToken);
                    await Settings.SetJwtRefreshExpiration(refresh.Token.RefreshExpiration);
                    await Settings.SetJwtToken(refresh.Token.Token);
                    await Settings.SetJwtTokenExpiration(refresh.Token.Expiration);
                }
                result = await this.GetApiObjectResponse<UserDto?>($"authorization/validateclaim/{deviceId:N}?validateClaimData={value}", cancellationToken: cancellationToken);
            }
        }
        return result;
    }
    #endregion Administrative
    #region User
    public async Task<ResultValues<EquipmentsToUserDto?>> LinkUserToEquipment(Guid deviceId, string cryptoKey, UpdateEquipmentToUserDto data, CancellationToken cancellationToken = default)
    {
        using RsaCryptographyService crypto = new(cryptoKey, null);
        string value = data.Wrap(true, crypto);
        return await this.PatchApi<EquipmentsToUserDto?>($"equipment/{deviceId}?updateEquipmentToUserData={value}", null, cancellationToken: cancellationToken);
    }
    public async Task<ResultValues<EquipmentsToUserDto?>> UnlinkUserToEquipment(Guid deviceId, string cryptoKey, UpdateEquipmentToUserDto data, CancellationToken cancellationToken = default)
    {
        using RsaCryptographyService crypto = new(cryptoKey, null);
        string value = data.Wrap(true, crypto);
        return await this.PatchApi<EquipmentsToUserDto?>($"equipment/{deviceId}?updateEquipmentToUserData={value}", null,cancellationToken: cancellationToken);
    }
    public async Task<ResultValues<UserDto?>> GetUser(Guid deviceId, Guid userId, GetUserDto data)
    {
        string value = data.Wrap(true);
        ResultValues<UserDto?> result = await this.GetApiObjectResponse<UserDto?>($"users/{deviceId}/{userId}?getUserData={value}");

        return result;
    }
    public async Task<ResultValues<IEnumerable<UserDto>?>> GetUsers(Guid deviceId, GetUsersDto data)
    {
        string value = data.Wrap(true);
        ResultValues<IEnumerable<UserDto>?> result = await this.GetApiObjectResponse<IEnumerable<UserDto>?>($"users/{deviceId}?getUsersData={value}");

        return result;
    }
    public async Task<ResultValues<UserDto?>> UpdateUser(Guid deviceId, UpdateUserDto data, CancellationToken cancellationToken = default)
    {
        string value = data.Wrap(true);
        var result = await this.PatchApi<UserDto?>($"users/{deviceId}/{data.UserId}/update?updateUserData={value}", content: null, cancellationToken: cancellationToken);
        return result;
    }
    public async Task<ResultValues<IEnumerable<JobTypeStepToUserDto>?>> GetUserJobTypeSteps(Guid deviceId, string cryptoKey, GetUserJobTypeStepsDto data, CancellationToken cancellationToken = default)
    {
        string value = data.Wrap(true);
        ResultValues<IEnumerable<JobTypeStepToUserDto>?> result = 
            await this.GetApiObjectResponse<IEnumerable<JobTypeStepToUserDto>>($"users/{deviceId}/{data.UserId}/jobtypestep/?getUserJobTypeStepsData={value}", true, cancellationToken);

        return result;
    }
    #endregion User
    #region Equipment
    public async Task<ResultValues<IEnumerable<EquipmentDto>?>> GetEquipment(Guid deviceId, GetEquipmentsDto data, CancellationToken cancellationToken = default)
    {
        string value = data.Wrap(true);
        ResultValues<IEnumerable<EquipmentDto>?> result = await this.GetApiObjectResponse<IEnumerable<EquipmentDto>?>($"equipment?getEquipmentsData={value}",cancellationToken: cancellationToken);
        
        return result;
    }
    public async Task<ResultValues<int>> GetEquipmentCount(Guid deviceId, GetEquipmentsDto data, CancellationToken cancellationToken = default)
    {
        string value = data.Wrap(true);
        ResultValues<int> result = await this.GetApiObjectResponse<int>($"equipment/count?getEquipmentsData={value}", cancellationToken: cancellationToken);

        return result;
    }
    #endregion Equipment
    #region PunchEntry
    public async Task<ResultValues<PunchEntryDto?>> CreatePunchEntry(Guid deviceId, string cryptoKey, CreatePunchEntryDto data, CancellationToken cancellationToken = default)
    {
        using RsaCryptographyService crypto = new(cryptoKey, null);
        string createPunchEntryData = data.Wrap(true, crypto);

        ResultValues<PunchEntryDto?> result = await this.PostApi<PunchEntryDto?>($"punchentries/{deviceId}?{nameof(createPunchEntryData)}={createPunchEntryData}", null, cancellationToken: cancellationToken);
        if (result.StatusCode == HttpStatusCode.Unauthorized)
        {
            string? jwt = await Settings.GetJwtToken();
            if (this._httpClient.DefaultRequestHeaders.Authorization is null && jwt is not null)
                this._httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(Core.HeaderParams.TokenTypePrefix, jwt);
            string? refreshToken = await Settings.GetJwtRefresh();
            if (!string.IsNullOrWhiteSpace(refreshToken))
            {
                ResultValues<string?> refresh = await this.RefreshJwt(deviceId, cryptoKey, new GetRefreshTokenDto()
                {
                    DeviceId = deviceId,
                    Password = data.Password,
                    UserName = data.UserName,
                    RefreshToken = refreshToken
                });

                if (refresh.Token is not null)
                {
                    await Settings.SetJwtRefresh(refresh.Token.RefreshToken);
                    await Settings.SetJwtRefreshExpiration(refresh.Token.RefreshExpiration);
                    await Settings.SetJwtToken(refresh.Token.Token);
                    await Settings.SetJwtTokenExpiration(refresh.Token.Expiration);
                }

                result = await this.PostApi<PunchEntryDto?>($"punchentries/{deviceId}?{nameof(createPunchEntryData)}={createPunchEntryData}", null, cancellationToken: cancellationToken);
            }
        }
        return result;
    }
    public async Task<ResultValues<PunchEntryDto?>> UpdatePunchEntry(Guid deviceId, string cryptoKey, CreatePunchEntryDto data, CancellationToken cancellationToken = default)
    {
        using RsaCryptographyService crypto = new(cryptoKey, null);
        string createPunchEntryData = data.Wrap(true, crypto);

        ResultValues<PunchEntryDto?> result = await this.PatchApi<PunchEntryDto?>($"punchentries/{deviceId}/{data.UserId}?{nameof(createPunchEntryData)}={createPunchEntryData}", null, cancellationToken: cancellationToken);
        if (result.StatusCode == HttpStatusCode.Unauthorized)
        {
            string? jwt = await Settings.GetJwtToken();
            if (this._httpClient.DefaultRequestHeaders.Authorization is null && jwt is not null)
                this._httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(Core.HeaderParams.TokenTypePrefix, jwt);
            string? refreshToken = await Settings.GetJwtRefresh();
            if (!string.IsNullOrWhiteSpace(refreshToken))
            {
                ResultValues<string?> refresh = await this.RefreshJwt(deviceId, cryptoKey, new GetRefreshTokenDto()
                {
                    DeviceId = deviceId,
                    Password = data.Password,
                    UserName = data.UserName,
                    RefreshToken = refreshToken
                });

                if (refresh.Token is not null)
                {
                    await Settings.SetJwtRefresh(refresh.Token.RefreshToken);
                    await Settings.SetJwtRefreshExpiration(refresh.Token.RefreshExpiration);
                    await Settings.SetJwtToken(refresh.Token.Token);
                    await Settings.SetJwtTokenExpiration(refresh.Token.Expiration);
                }

                result = await this.PostApi<PunchEntryDto?>($"punchentries/{deviceId}/{data.UserId}?{nameof(createPunchEntryData)}={createPunchEntryData}", null, cancellationToken: cancellationToken);
            }
        }
        return result;
    }
    public async Task<ResultValues<IEnumerable<PunchEntryDto>?>> GetPunches(Guid deviceId, GetPunchEntriesDto data, CancellationToken cancellationToken = default)
    {
        string value = data.Wrap(true);
        ResultValues<IEnumerable<PunchEntryDto>?> result = await this.GetApiObjectResponse<IEnumerable<PunchEntryDto>?>($"punchentries/{deviceId:N}?getPunchEntriesData={value}", cancellationToken: cancellationToken);

        return result;
    }
    #endregion PunchEntry
    #region Locations
    public async Task<ResultValues<LocationDto?>> GetLocation(Guid deviceId, Guid locationId, CancellationToken cancellationToken = default)
    {
        return (await this.GetApiObjectResponse<LocationDto?>($"locations/{deviceId}/{locationId}", cancellationToken: cancellationToken));
    } 
    public async Task<IEnumerable<LocationDto>> GetLocations(Guid deviceId, CancellationToken cancellationToken = default)
    {
        return (await this.GetApiObjectResponse<IEnumerable<LocationDto>>($"locations/{deviceId}", cancellationToken: cancellationToken)).Value ?? Enumerable.Empty<LocationDto>();
    }
    #endregion Locations
    #region Departments
    public async Task<IEnumerable<DepartmentDto>> GetDepartments(Guid deviceId)
    {
        return (await this.GetApiObjectResponse<IEnumerable<DepartmentDto>>($"departments/{deviceId}")).Value ?? Enumerable.Empty<DepartmentDto>();
    }
    #endregion Departments
    #region Devices
    public Task<ResultValues<DeviceDto?>> GetDevice(Guid deviceId, GetDeviceDto data, string cryptoKey, CancellationToken cancellationToken = default)
    {
        using RsaCryptographyService crypto = new(cryptoKey, null);
        string value = data.Wrap(true, crypto);
        return this.GetApiObjectResponse<DeviceDto?>($"devices/{deviceId}?getDeviceData={value}", cancellationToken: cancellationToken);
    }
    #endregion Devices
    #region Work Days
    public Task<ResultValues<IEnumerable<WorkPeriodDto>?>> GetWorkPeriods(Guid deviceId, GetWorkPeriodsDto data, string cryptoKey, CancellationToken cancellationToken = default)
    {
        using RsaCryptographyService crypto = new(cryptoKey, null);
        string value = data.Wrap(true, crypto);
        return this.GetApiObjectResponse<IEnumerable<WorkPeriodDto>>($"workperiods/{deviceId}?getWorkPeriodsData={value}", cancellationToken: cancellationToken);
    }
    public Task<ResultValues<WorkPeriodDto?>> GetWorkPeriod(Guid deviceId, Guid workPeriodId, GetWorkPeriodDto data, string cryptoKey, CancellationToken cancellationToken = default)
    {
        using RsaCryptographyService crypto = new(cryptoKey, null);
        string value = data.Wrap(true, crypto);
        return this.GetApiObjectResponse<WorkPeriodDto>($"workperiods/{deviceId}/{workPeriodId}?getWorkPeriodData={value}", cancellationToken: cancellationToken);
    }
    #endregion Work Days
    #region Job Type Steps
    public Task<ResultValues<IEnumerable<JobTypeDto>?>> GetJobTypes(Guid deviceId, GenericGetDto data, string cryptoKey, CancellationToken cancellationToken = default)
    {
        using RsaCryptographyService crypto = new(cryptoKey, null);
        string value = data.Wrap(true, crypto);
        return this.GetApiObjectResponse<IEnumerable<JobTypeDto>>($"jobtypes/{deviceId}?getJobTypesData={value}", cancellationToken: cancellationToken);
    }
    public Task<ResultValues<IEnumerable<JobStepDto>?>> GetJobSteps(Guid deviceId, GenericGetDto data, string cryptoKey, CancellationToken cancellationToken = default)
    {
        using RsaCryptographyService crypto = new(cryptoKey, null);
        string value = data.Wrap(true, crypto);
        return this.GetApiObjectResponse<IEnumerable<JobStepDto>>($"jobsteps/{deviceId}?getJobStepsData={value}", cancellationToken: cancellationToken);
    }

    #endregion Job Type Steps
    #endregion Public Methods

    #region Private Methods

    private async Task<ResultValues<string?>> GetApiStringResponse(string apiQuery, bool updateAuth = true, CancellationToken cancellationToken = default)
    {
        return await this.GetApiResult<string>(this._httpClient.GetAsync($"{this.ApiSubPath}{apiQuery}", cancellationToken), apiQuery, null, updateAuth, cancellationToken);
    }
    private async Task<ResultValues<T?>> GetApiObjectResponse<T>(string apiQuery, bool updateAuth = true, CancellationToken cancellationToken = default)
    {
        return await this.GetApiResult<T?>(this._httpClient.GetAsync($"{this.ApiSubPath}{apiQuery}", cancellationToken), apiQuery, null, updateAuth, cancellationToken);
    }
    private async Task<ResultValues<T?>> PostApi<T>(string apiQuery, HttpContent? content, bool updateAuth = true, CancellationToken cancellationToken = default)
    {
        return await this.GetApiResult<T?>(this._httpClient.PostAsync($"{this.ApiSubPath}{apiQuery}", content, cancellationToken), apiQuery, content, updateAuth, cancellationToken);
    }
    private async Task<ResultValues<T?>> PatchApi<T>(string apiQuery, HttpContent? content, bool updateAuth = true, CancellationToken cancellationToken = default)
    {
        return await this.GetApiResult<T?>(this._httpClient.PatchAsync($"{this.ApiSubPath}{apiQuery}", content, cancellationToken), apiQuery, content, updateAuth, cancellationToken);
    }

    private async Task<ResultValues<T?>> GetApiResult<T>(Task<HttpResponseMessage> rest, string apiQuery, object? content, bool updateAuth = true, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage result;
        T? value = default(T);

        try
        {
            result = await rest;
        }
        catch (Exception tex) when (tex is TimeoutException || tex is TaskCanceledException || tex is HttpRequestException)
        {
            this.Logger.LogError(tex, "query:{apiQuery},content{content},updateAuth:{updateAuth}", apiQuery, JsonSerializer.Serialize(content), updateAuth);
            return new(HttpStatusCode.ServiceUnavailable, false, null, null, default);
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "query:{apiQuery},content{content},updateAuth:{updateAuth}", apiQuery, JsonSerializer.Serialize(content), updateAuth);
            return new(HttpStatusCode.Unused, false, null, null, default);
        }

        (string? pkey, TokenPackage? token) data = TimeClockApiAccessService.GetRequestData(result);

        if (updateAuth && result.IsSuccessStatusCode)
        {
            if (!string.IsNullOrWhiteSpace(data.token?.Token))
                this._httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(Core.HeaderParams.TokenTypePrefix, data.token.Token);
        } 

        if (result.Content.Headers.ContentLength > 0) //result.IsSuccessStatusCode && 
        {
            try
            {
                value = await result.Content.ReadFromJsonAsync<T>(cancellationToken);
            }catch(Exception ex)
            {
                this.Logger.LogError(ex, "Could not read response as JSON. response: {ApiResponse}", await result.Content.ReadAsStringAsync());
            }
        }

        return new(result.StatusCode, result.IsSuccessStatusCode, data.pkey, data.token, value);
    }

    private static (string? pkey, TokenPackage? token) GetRequestData(HttpResponseMessage result)
    {
        TokenPackage? package = null;
        IEnumerable<string>? pkey;
        IEnumerable<string>? jwt;

        result.Headers.TryGetValues(HeaderParams.CustomCryptoKeyParameter, out pkey);
        result.Headers.TryGetValues(HeaderParams.CustomTokenParameter, out jwt);

        if (jwt is not null)
            package = TokenPackage.Unwrap(jwt.FirstOrDefault() ?? string.Empty);

        return new(pkey?.FirstOrDefault(), package);
    }
    #endregion Private Methods

}

internal record class ResultValues<T>(HttpStatusCode StatusCode, bool IsSuccessfulStatusCode, string? Pkey, TokenPackage? Token, T? Value);
//internal record struct TimeClockApiAccessSettings(string EndPoint, string ApiPublicKey, string ApiSecret, string DeviceId);
internal sealed class TimeClockApiAccessSettings 
{
    public TimeClockApiAccessSettings()
    {
        this.Endpoint = Settings.ApiEndpoint;
        this.ApiPublicKey = Task.Run(() => Settings.GetCryptoApiKey()).GetAwaiter().GetResult() ?? string.Empty;
        this.DeviceId = Settings.DeviceGuid;
    }

    public string Endpoint { get; init; }
    public string ApiPublicKey {  get; init; }
    //public string ApiSecret { get; init; }
    public Guid DeviceId { get; init; }
}
