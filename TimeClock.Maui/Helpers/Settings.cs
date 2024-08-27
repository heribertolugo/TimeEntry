using TimeClock.Core.Helpers;
using TimeClock.Core.Models.EntityDtos;

namespace TimeClock.Maui.Helpers;

internal static class Settings
{
    // 0 = default, 1 = light, 2 = dark
    private static readonly AppTheme _defaultTheme = AppTheme.Dark;
    private static readonly bool _isBarcodeDefault = false;
    private static readonly Guid _defaultLocationId = Guid.Empty;
    private static readonly Guid _defaultDepartmentId = Guid.Empty;
    private static readonly Guid _deviceGuid = Guid.Empty;
    private static readonly string _deviceId = "";
    private static readonly PunchTypeDto _defaultPunchType = PunchTypeDto.Domain;
    private static readonly PunchTypeDto _defaultActivePunchType = PunchTypeDto.Domain;
    private static readonly UiPageMeta _defaultScreen = UiPageMeta.Punch;
    private static readonly int _defaultIdleScreenSeconds = 15;
    private static readonly string _cryptoApiKey = "";
    private static readonly string _jwtToken = "";
    private static readonly DateTime _jwtTokenExpiration = DateTime.MinValue;
    private static readonly string _jwtRefresh = "";
    private static readonly DateTime _jwtRefreshExpiration = DateTime.MinValue;
    private static readonly double? _latitudeDefault = null;// -90;
    private static readonly double? _longitudeDefault = null;// 0;
    private static readonly bool _isRegisteredDefault = false;
    private static readonly bool _isPublicDeviceDefault = false;
    private static readonly Guid _registeredByDefault = Guid.Empty;
    private static readonly string _locationDivision = "";
#if DEBUG
    private static readonly string _defaultApiEndpoint = "https://localhost:7142/";
#else
    private static readonly string _defaultApiEndpoint = "";
#endif

    /// <summary>
    /// Switch to toggle ActivityRedirect feature on or off.
    /// This property is not meant to be changed during runtime.
    /// Set IdleScreenSeconds during runtime instead, to a value less than 1 to disable.
    /// As application restart would be needed.
    /// This is responsible for injecting the ability to listen to certain WinProc messages.
    /// </summary>
    internal static readonly bool ActivityRedirectActive = false;

    public static AppTheme CurrentTheme
    {
        get => (AppTheme)Preferences.Get(nameof(CurrentTheme), (int)Settings._defaultTheme);
        set => Preferences.Set(nameof(CurrentTheme), (int)value);
    }
    public static bool IsBarcode
    {
        get => Preferences.Get(nameof(IsBarcode), Settings._isBarcodeDefault);
        set => Preferences.Set(nameof(IsBarcode), value);
    }
    public static Guid LocationId
    {
        get => new Guid(Preferences.Get(nameof(LocationId), Settings._defaultLocationId.ToString()));
        set => Preferences.Set(nameof(LocationId), value.ToString());
    }
    public static Guid DepartmentId
    {
        get => new Guid(Preferences.Get(nameof(DepartmentId), Settings._defaultDepartmentId.ToString()));
        set => Preferences.Set(nameof(DepartmentId), value.ToString());
    }
    public static PunchTypeDto DefaultPunchType
    {
        get => (PunchTypeDto)Preferences.Get(nameof(DefaultPunchType), (int)Settings._defaultPunchType);
        set => Preferences.Set(nameof(DefaultPunchType), (int)value);
    }
    public static PunchTypeDto ActivePunchType
    {
        get => (PunchTypeDto)Preferences.Get(nameof(ActivePunchType), (int)Settings._defaultActivePunchType);
        set => Preferences.Set(nameof(ActivePunchType), (int)value);
    }
    public static string DeviceId
    {
        get => Preferences.Get(nameof(DeviceId), Settings._deviceId);
        set => Preferences.Set(nameof(DeviceId), value);
    }
    public static Guid DeviceGuid
    {
        get => new Guid(Preferences.Get(nameof(DeviceGuid), Settings._deviceGuid.ToString()));
        set => Preferences.Set(nameof(DeviceGuid), value.ToString());
    }
    public static string ApiEndpoint
    {
        get => Preferences.Get(nameof(ApiEndpoint), Settings._defaultApiEndpoint);
        set => Preferences.Set(nameof(ApiEndpoint), value);
    }
    public static UiPageMeta IdleScreen
    {
        get => (UiPageMeta)Preferences.Get(nameof(IdleScreen), (int)Settings._defaultScreen);
        set => Preferences.Set(nameof(IdleScreen), (int)value);
    }
    public static int IdleScreenSeconds
    {
        get => Preferences.Get(nameof(IdleScreenSeconds), Settings._defaultIdleScreenSeconds);
        set => Preferences.Set(nameof(IdleScreenSeconds), value);
    }
    public static double? Latitude
    {
        get => (Preferences.Get(nameof(Latitude), Settings._latitudeDefault?.ToString()))?.ToDoubleOrNull();
        set => Preferences.Set(nameof(Latitude), value?.ToString());
    }
    public static double? Longitude
    {
        get => (Preferences.Get(nameof(Longitude), Settings._longitudeDefault?.ToString()))?.ToDoubleOrNull();
        set => Preferences.Set(nameof(Longitude), value?.ToString());
    }
    public static bool IsRegistered
    {
        get => Preferences.Get(nameof(IsRegistered), Settings._isRegisteredDefault);
        set => Preferences.Set(nameof(IsRegistered), value);
    }
    public static bool IsPublicDevice
    {
        get => Preferences.Get(nameof(IsPublicDevice), Settings._isPublicDeviceDefault);
        set => Preferences.Set(nameof(IsPublicDevice), value);
    }
    public static Guid RegisteredBy
    {
        get => new Guid(Preferences.Get(nameof(RegisteredBy), Settings._registeredByDefault.ToString()));
        set => Preferences.Set(nameof(RegisteredBy), value.ToString());
    }
    public static string? LocationDivision
    {
        get => Preferences.Get(nameof(LocationDivision), Settings._locationDivision);
        set => Preferences.Set(nameof(LocationDivision), value);
    }

#warning SecureStorage needs permissions set on Android and iOs
    #region Secure Settings
    public static Task<string?> GetCryptoApiKey()
    {
        return SecureStorage.Default.GetAsync(nameof(Settings._cryptoApiKey));
    }
    public static Task SetCryptoApiKey(string value)
    {
        return SecureStorage.Default.SetAsync(nameof(Settings._cryptoApiKey), value);
    }
    public static Task<string?> GetJwtToken()
    {
        return SecureStorage.Default.GetAsync(nameof(Settings._jwtToken));
    }
    public static Task SetJwtToken(string value)
    {
        return SecureStorage.Default.SetAsync(nameof(Settings._jwtToken), value);
    }
    public static async Task<DateTime?> GetJwtTokenExpiration()
    {
        string? expiration = await SecureStorage.Default.GetAsync(nameof(Settings._jwtTokenExpiration));
        return expiration is null ? null : DateTime.Parse(expiration);
    }
    public static Task SetJwtTokenExpiration(DateTime value)
    {
        return SecureStorage.Default.SetAsync(nameof(Settings._jwtTokenExpiration), value.ToString());
    }
    public static Task<string?> GetJwtRefresh()
    { 
        return SecureStorage.Default.GetAsync(nameof(Settings._jwtRefresh));
    }
    public static Task SetJwtRefresh(string value)
    {
        return SecureStorage.Default.SetAsync(nameof(Settings._jwtRefresh), value);
    }
    public static async Task<DateTime?> GetJwtRefreshExpiration()
    {
        string? expiration = await SecureStorage.Default.GetAsync(nameof(Settings._jwtRefreshExpiration));
        return expiration is null ? null : DateTime.Parse(expiration);
    }
    public static Task SetJwtRefreshExpiration(DateTime value)
    {
        return SecureStorage.Default.SetAsync(nameof(Settings._jwtRefreshExpiration), value.ToString());
    }
    #endregion Secure Settings


    public static void Reset()
    {
        Preferences.Clear();
        SecureStorage.Default.RemoveAll();
    }

    public static void SetDeviceId()
    {
        Guid guid = Guid.NewGuid();
        Settings.DeviceGuid = guid;

        Settings.DeviceId = DeviceGuidHelper.GuidToFriendly(guid);
    }

}
