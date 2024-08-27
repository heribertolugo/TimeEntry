using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.Input;
using MetroLog.Maui;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using TimeClock.Core.Models.ApiDtos;
using TimeClock.Core.Models.EntityDtos;
using TimeClock.Maui.Helpers;
using TimeClock.Maui.Services;
using TimeClock.Maui.ViewModels.Shared;

namespace TimeClock.Maui.ViewModels;

internal sealed class ConfigurationsViewModel : ViewModelBase
{
    #region Private Members
    private static readonly Color SuccessColor = Colors.LawnGreen;
    private static readonly Color ErrorColor = Colors.DarkRed;
    private AppTheme _selectedTheme;
    private LocationDto? _selectedLocation;
    private DepartmentDto? _selectedDepartment;
    private PunchTypeDto _defaultPunchType;
    private string _deviceId;
    private Guid _deviceGuid;
    private string _apiEndpoint;
    private UiPageMeta _selectedIdleScreen;
    private int _selectedScreenTimeout;
    private string _apiToken;
    private DateTime _apiTokenExpiration;
    private ObservableCollection<LocationDto> _locations;
    private ObservableCollection<DepartmentDto> _departments;
    private ObservableCollection<PunchTypeDto> _punchTypes;
    private ObservableCollection<IUiPageMeta> _idleScreens;
    private bool _isSelectedLocationValid = true;
    private bool _isSelectedDepartmentValid = true;
    private bool _isDeviceIdValid = true;
    private bool _isApiEndPointValid = true;
    private bool _isApiTokenValid = true;
    private bool _isPublicDevice;
    private bool _isNotRegistered = true;
    private string _message;
    private Color _messageColor;
    private ILogger Logger { get; set; }
    private ITimeClockApiAccessService ApiAccessService { get; set; }
    private bool HasInternet { get; set; }
    #endregion Private Members


#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public ConfigurationsViewModel() : base()
    {
        this.ApiAccessService = ServiceHelper.GetApiService()!;
        this.Logger = ServiceHelper.GetLoggerService<ConfigurationsViewModel>()!;
        this.LoadProperties();
        this.SetCommands();
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.


    #region Public Properties
    public ObservableCollection<LocationDto> Locations { get => this._locations; private set => base.SetProperty(ref this._locations, value); }
    public ObservableCollection<DepartmentDto> Departments { get => this._departments; private set => base.SetProperty(ref this._departments, value); }
    public ObservableCollection<PunchTypeDto> PunchTypes { get => this._punchTypes; private set => this._punchTypes = value; }
    public ObservableCollection<IUiPageMeta> IdleScreens { get => this._idleScreens; private set => this._idleScreens = value; }


    public AppTheme SelectedTheme
    {
        get => this._selectedTheme;
        set
        {
            base.SetProperty(ref this._selectedTheme, value);
        }
    }

    public bool IsLightTheme { get; private set; }

    public LocationDto? SelectedLocation
    {
        get => this._selectedLocation;
        set 
        { 
            base.SetProperty(ref this._selectedLocation, value);
            this.IsSelectedLocationValid = value != null;
            if (value is not null)
                this.SelectedDepartment = this.Departments.FirstOrDefault(d => d.Name == value.Name);
        }
    }
    public bool IsSelectedLocationValid 
    { 
        get => this._isSelectedLocationValid;
        set { base.SetProperty(ref this._isSelectedLocationValid, value); }
    }

    public DepartmentDto? SelectedDepartment
    {
        get => this._selectedDepartment; 
        set 
        { 
            base.SetProperty(ref this._selectedDepartment, value);
            this.IsSelectedDepartmentValid = value != null;
        }
    }
    public bool IsSelectedDepartmentValid
    {
        get => this._isSelectedLocationValid;
        set { base.SetProperty(ref this._isSelectedDepartmentValid, value); }
    }

    public PunchTypeDto DefaultPunchType
    {
        get => this._defaultPunchType; 
        set
        {
            base.SetProperty(ref this._defaultPunchType, value);
        }
    }

    public string DeviceId
    {
        get => this._deviceId; 
        private set 
        {
            base.SetProperty(ref this._deviceId, value);
        }
    }
    public bool IsDeviceIdValid
    {
        get => this._isDeviceIdValid;
        set
        {
            base.SetProperty(ref this._isDeviceIdValid, value);
        }
    }

    public Guid DeviceGuid
    {
        get => this._deviceGuid;
        private set => base.SetProperty(ref this._deviceGuid, value);
    }

    public string ApiEndpoint
    {
        get => this._apiEndpoint; 
        set 
        { 
            base.SetProperty(ref this._apiEndpoint, value);
        }
    }

    public bool IsApiEndPointValid
    {
        get => this._isApiEndPointValid; 
        set 
        { 
            base.SetProperty(ref this._isApiEndPointValid, value); 
        }
    }

    public UiPageMeta SelectedIdleScreen
    {
        get => this._selectedIdleScreen; 
        set 
        { 
            base.SetProperty(ref this._selectedIdleScreen, value);
        }
    }

    public int SelectedScreenTimeout
    {
        get => this._selectedScreenTimeout; 
        set 
        { 
            base.SetProperty(ref this._selectedScreenTimeout, value);
        }
    }

    public string ApiToken
    {
        get => this._apiToken; 
        private set 
        { 
            base.SetProperty(ref this._apiToken, value);
        }
    }

    public bool IsApiTokenValid
    {
        get => this._isApiTokenValid; 
        set 
        { 
            base.SetProperty(ref this._isApiTokenValid, value); 
        }
    }

    public DateTime ApiTokenExpiration
    {
        get => this._apiTokenExpiration; 
        private set 
        { 
            base.SetProperty(ref this._apiTokenExpiration, value);                
        }
    }

    public string AppVersion { get; private set; }

    public string Message 
    { 
        get => this._message;
        private set
        {
            base.SetProperty(ref this._message, value);
        }
    }

    public Color MessageColor
    {
        get => this._messageColor;
        private set
        {
            base.SetProperty(ref this._messageColor, value);
        }
    }
    public bool IsPublicDevice
    {
        get => this._isPublicDevice;
        set
        {
            base.SetProperty(ref this._isPublicDevice, value);
        }
    }
    public override bool IsBusy 
    { 
        get => base.IsBusy;
        set
        {
            base.IsBusy = value;
            base.OnPropertyChanged(nameof(this.IsNotBusyOrNotRegistered));
        }
    }
    public bool IsNotRegistered
    {
        get => this._isNotRegistered;
        private set
        {
            base.SetProperty(ref this._isNotRegistered, value);
            base.OnPropertyChanged(nameof(this.IsNotBusyOrNotRegistered));
        }
    }
    public bool IsNotBusyOrNotRegistered 
    {
        get 
        {
            Debug.WriteLine($"{nameof(base.IsNotBusy)}:{base.IsNotBusy}, {nameof(this.IsNotRegistered)}: {this.IsNotRegistered}");
            return base.IsNotBusy && this.IsNotRegistered; 
        }
    }

    public bool IsConnectionSuccessful
    {
        get; private set;
    }

    #endregion Public Properties


    #region Public Commands
    public IAsyncRelayCommand TestConnectionCommand { get; private set; }
    public IAsyncRelayCommand RegisterDeviceCommand { get; private set; }
    public IAsyncRelayCommand SaveCommand { get; private set; }
    public IAsyncRelayCommand ReAuthenticateJwtCommand { get; private set; }
    public IAsyncRelayCommand CheckForUpdatesCommand { get; private set; }
    public IAsyncRelayCommand ViewLogsCommand { get; private set; }
    public IAsyncRelayCommand ThemeToggledCommand { get; private set; }
    public IAsyncRelayCommand SubmitJwtRefreshCommand { get; private set; }

    public IAsyncRelayCommand SelectLocationHelpCommand { get; private set; }
    public IAsyncRelayCommand SelectDepartmentHelpCommand { get; private set; }
    public IAsyncRelayCommand SelectPunchTypeHelpCommand { get; private set; }
    public IAsyncRelayCommand SelectIdleScreenHelpCommand { get; private set; }
    public IAsyncRelayCommand ApiEndpointHelpCommand { get; private set; }
    public IAsyncRelayCommand DeviceIdHelpCommand { get; private set; }
    public IAsyncRelayCommand DeviceGuidHelpCommand { get; private set; }
    public IAsyncRelayCommand ApiTokenHelpCommand { get; private set; }
    public IAsyncRelayCommand ApiTokenExpirationHelpCommand { get; private set; }
    public IAsyncRelayCommand IsPublicDeviceHelpCommand { get; private set; }
    #endregion Public Commands

    #region Private Commands
    protected override async Task Refresh()
    {
        this.LoadProperties();

        if (string.IsNullOrWhiteSpace(this.ApiEndpoint) || !this.IsApiEndPointValid)
            return;

        await this.LoadLocationsDepartments();
    }
    private async Task RegisterDevice()
    {
        // verify connection and validate
        bool isValid = await this.Validate();
        RegisterDeviceDto data;
        ResultValues<string?>? keys;
        ResultValues<Guid>? result = default;
        string? jwt = null;
        IPopupService PopupService = Helpers.ServiceHelper.GetPopoupService()!;
        CredentialsViewModel? credentials = null;

        if (!(await this.BeginProcessing())) return;

        if (!isValid)
        {
            await this.EndProcessing("One or more fields are invalid!");
            return;
        }

        bool isLocationRequired = PermissionsManager.IsLocationRequired();
        Location? geolocation = await PermissionsManager.GetOrRequestLocation(isRequired: isLocationRequired, getCached: !isLocationRequired);

        if (isLocationRequired && geolocation is null)
        {
            await this.EndProcessing(null);
            await UiHelpers.ShowOkAlert(CommonValues.GeoLocRequired);
            return;
        }

        Settings.Latitude = geolocation?.Latitude;
        Settings.Longitude = geolocation?.Longitude;

        // get and save keys
        this.SetMessage("Getting API tokens");

        if (string.IsNullOrWhiteSpace(jwt = await Settings.GetJwtToken()))
        {
            keys = await this.GetSetKeys();

            // GetSetKeys already alerts the user in the event of a failure
            if (keys is null || !keys.IsSuccessfulStatusCode)
            {
                await this.EndProcessing(null);
                return;
            }
        }else
        {
            keys = new ResultValues<string?>(HttpStatusCode.OK, true, await Settings.GetCryptoApiKey(), 
                new TokenPackage(jwt, (await Settings.GetJwtRefreshExpiration()).Value, (await Settings.GetJwtRefresh())!, (await Settings.GetJwtRefreshExpiration()).Value),
                null);
        }

        // force user to provide credentials to continue app registration
        while (string.IsNullOrWhiteSpace(credentials?.UserName) || string.IsNullOrWhiteSpace(credentials?.Password))
        {
            credentials = (await PopupService.ShowPopupAsync<CredentialsViewModel>(m => { m.Title = "Credentials"; m.Message = "Please provide your credentials"; m.ShowEmployeeId = true;  })) as CredentialsViewModel;
        }

        this.SetMessage("Registering Device");
        
        data = new RegisterDeviceDto()
        {
            DepartmentId = this.SelectedDepartment!.Id,
            DeviceId = this.DeviceGuid,
            DeviceName = this.DeviceId,
            LocationId = this.SelectedLocation!.Id,
            RefreshToken = keys.Token!.RefreshToken,
            Username = this.UserName = credentials!.UserName!,
            Password = credentials.Password!,
            EmployeeId = credentials.EmployeeId ?? 0,
            IsPublic = this.IsPublicDevice
        };

        if (!this.SelectedLocation.Latitude.HasValue)
            this.SelectedLocation.Latitude = Settings.Latitude;
        if (!this.SelectedLocation.Longitude.HasValue)
            this.SelectedLocation.Longitude = Settings.Longitude;

        try
        {
            // register device
            result = await this.ApiAccessService.RegisterDevice(keys.Pkey!, keys.Token.Token, data);
        }catch(Exception ex)
        {
            this.Logger.LogError(ex, "keys.Pkey={keys.Pkey} : keys.Token.Token{keys.Token.Token} : data{data}", [keys.Pkey, keys.Token.Token, data.AsJson()]);
        }

        if (result?.Token is null)
        {
            await UiHelpers.ShowOkAlert("New keys were not found.\nThis may cause issues.\nTry and refresh keys.");
        }
        else
        {
            this.SetMessage("Saving keys");
            await this.SaveRegisteredFields();
            this.ApiToken = result.Token.RefreshToken;
            this.ApiTokenExpiration = result.Token.RefreshExpiration.ToLocalTime();
        }

        if (result is not null && result.IsSuccessfulStatusCode)
        {
            Settings.IsRegistered = true;
            Settings.RegisteredBy = result.Value;
        }

        await this.EndProcessing(result?.IsSuccessfulStatusCode ?? false ? "Successfully Registered. Restart Required. App will now close. Please re-open." : "Could not register. Contact IT", string.Empty);

        if (Application.Current?.MainPage is not null && Settings.IsRegistered)
            Application.Current.CloseWindow(Application.Current.MainPage.Window);
    }

    internal async Task TestConnection()
    {
        bool success = false;

        if (!(await this.BeginProcessing())) return;

        this.SetMessage("Checking connection to server");

        if (this.DeviceGuid == default)
        {
            Settings.SetDeviceId();
            this.DeviceGuid = Settings.DeviceGuid;
            this.DeviceId = Settings.DeviceId;
            this.IsDeviceIdValid = true;
        }

        if (string.IsNullOrWhiteSpace(this.ApiEndpoint))
        {
            await this.EndProcessing("NO ENDPOINT DEFINED!");
            return;
        }

        try
        {
            success = await this.ValidateConnection();
        }
        catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException)
        {
            await this.EndProcessing("Connection Refused By Server or Cancelled!");
            return;
        }

        if (!success)
        {
            await this.EndProcessing(this.HasInternet ? "INVALID ENDPOINT!" : "NO INTERNET CONNECTION");
            return;
        }

        //Settings.ApiEndpoint = this.ApiEndpoint;

        if (this.Locations.Count < 1) await this.LoadLocationsDepartments();

        Settings.ApiEndpoint = this.ApiEndpoint;

        await this.EndProcessing("Connection Established!", string.Empty);
    }
    private async Task SubmitJwtRefresh(object? paramz)
    {
        if (!(await this.BeginProcessing())) return;

        this.SetMessage("fetching keys");

        if (paramz is not object[] values) 
        {
            await this.EndProcessing("missing username or password");
            return;
        }
        GetRefreshTokenDto tokenDto = new GetRefreshTokenDto()
        {
            Password = values[1] as string,
            UserName = values[0] as string,
            DeviceId = Settings.DeviceGuid,
            RefreshToken = await Settings.GetJwtRefresh()
        };

        if (string.IsNullOrWhiteSpace(tokenDto.UserName)
            || string.IsNullOrWhiteSpace(tokenDto.Password)
            || string.IsNullOrWhiteSpace(tokenDto.RefreshToken))
        {
            await this.EndProcessing("missing username or password");
            return;
        }

        if (string.IsNullOrWhiteSpace(this.ApiEndpoint))
        {
            await this.EndProcessing($"Endpoint not specified!");
            return;
        }

        ResultValues<string?>? keys = await this.GetSetKeys(tokenDto);

        await this.EndProcessing(null);

        if (keys?.Token is not null) 
        {
            this.ApiToken = keys.Token.RefreshToken;
            this.ApiTokenExpiration = keys.Token.RefreshExpiration.ToLocalTime();

            await Settings.SetJwtToken(keys.Token.Token);
            await Settings.SetJwtTokenExpiration(keys.Token.Expiration);
            await Settings.SetJwtRefresh(keys.Token.RefreshToken);
            await Settings.SetJwtRefreshExpiration(keys.Token.RefreshExpiration);
        }
    }
    private async Task ReAuthenticateJwt(StackBase? layoutControl)
    {
        ArgumentNullException.ThrowIfNull(layoutControl, nameof(layoutControl));

        await this.AnimateCredentialsLayout(layoutControl);
    }
    private async Task CheckForUpdates()
    {
#warning implement check for updates button
    }
    private Task ViewLogs()
    {
        var logController = new LogController();
        logController.GoToLogsPageCommand.Execute(null);
        return Task.CompletedTask;
    }
    private async Task Save()
    {
        if (!await this.Validate())
        {
            await UiHelpers.ShowOkAlert("Cannot save with missing values", "ERROR");
            return;
        }

        Settings.CurrentTheme = this.SelectedTheme;
        Settings.DefaultPunchType = this.DefaultPunchType;
        Settings.IdleScreen = this.SelectedIdleScreen;
        Settings.IdleScreenSeconds = this.SelectedScreenTimeout;

        await UiHelpers.ShowOkAlert("Saved!", "SUCCESS");
    }

    #region Help Popups
    private async Task SelectLocationHelp() => await this.ShowHelpPopup();
    private async Task SelectDepartmentHelp() => await this.ShowHelpPopup();
    private async Task SelectPunchTypeHelp() => await this.ShowHelpPopup();
    private async Task SelectIdleScreenHelp() => await this.ShowHelpPopup();
    private async Task ApiEndpointHelp() => await this.ShowHelpPopup();
    private async Task DeviceIdHelp() => await this.ShowHelpPopup();
    private async Task DeviceGuidHelp() => await this.ShowHelpPopup();
    private async Task ApiTokenHelp() => await this.ShowHelpPopup();
    private async Task ApiTokenExpirationHelp() => await this.ShowHelpPopup();
    private async Task IsPublicDeviceHelp() => await this.ShowHelpPopup();
    #endregion Help Popups

    private Task ThemeToggled(bool state, CancellationToken? token)
    {
        this.SelectedTheme = state ? AppTheme.Light : AppTheme.Dark;
        TheTheme.SetTheme();
        return Task.CompletedTask;
    }
    #endregion Private Commands


    #region Private Methods
    private Task SaveRegisteredFields()
    {
        Settings.LocationId = this.SelectedLocation?.Id ?? default;
        Settings.DepartmentId = this.SelectedDepartment?.Id ?? default;
        Settings.DeviceGuid = this.DeviceGuid;
        Settings.DeviceId = this.DeviceId;
        Settings.ApiEndpoint = this.ApiEndpoint;
        Settings.IsPublicDevice = this.IsPublicDevice;
        Settings.LocationDivision = this.SelectedLocation?.DivisionCode;
        return Task.CompletedTask;
    }
    private async Task<string> GetCredentials()
    {
        string title = "Credentials";

        if (App.Current?.MainPage is null)
            throw new InvalidOperationException(CommonValues.AppCurInac);

        string? result = null;

        while (string.IsNullOrEmpty(result))
        {
            result = await App.Current.MainPage.DisplayPromptAsync($"{title} - User Name", "Please provide your user name to continue", accept: "NEXT");
        }

        this.UserName = result;

        result = null;

        while (string.IsNullOrEmpty(result))
        {
            result = await App.Current.MainPage.DisplayPromptAsync($"{title} - Password", "Please provide your password to continue");
        }

        return result;
    }
    /// <summary>
    /// Attempts to get the encryption key and JWT from the API if a valid one is not found saved in settings.
    /// </summary>
    /// <param name="getRefreshTokenDto"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    private async Task<ResultValues<string?>?> GetSetKeys(GetRefreshTokenDto? getRefreshTokenDto = null)
    {
        ResultValues<string?>? keys = null;
        string? refreshToken = null;
        DateTime expiration = DateTime.MinValue;
        try
        {
            // check if our refresh expiration exists. 
            // if it does not exists or if it does and will expire within the hour, get new JWT and refresh
            // otherwise return what we already have
            //expiration = (await Settings.GetJwtRefreshExpiration()) ?? DateTime.UtcNow;
            //if (expiration.AddHours(-1) >= DateTime.UtcNow)
            //{
            //    // assume we have all keys, since we had a JWT expiration saved. if we were wrong, an exception will be thrown
            //    return new ResultValues<string>(System.Net.HttpStatusCode.OK,
            //        true, (await Settings.GetCryptoApiKey()) ?? throw new ArgumentNullException("Expected PKey was null", (Exception?)null),
            //        new TokenPackage((await Settings.GetJwtToken() ?? throw new ArgumentNullException("Expected JWT was null", (Exception?)null)),
            //        (await Settings.GetJwtTokenExpiration()).Value,
            //        (await Settings.GetJwtRefresh()) ?? throw new ArgumentNullException("Expected JWT Refresh was null", (Exception?)null),
            //        (await Settings.GetJwtRefreshExpiration()).Value),
            //        null);
            //}

            refreshToken = await Settings.GetJwtRefresh();

            if (string.IsNullOrWhiteSpace(refreshToken))
                keys = await this.ApiAccessService.GetKeys(this.DeviceGuid, this.DeviceId);
            else
            {
                ArgumentNullException.ThrowIfNull(getRefreshTokenDto);

                string? cryptoKey;
                if (string.IsNullOrWhiteSpace(cryptoKey = await Settings.GetCryptoApiKey()))
                    throw new ArgumentNullException(nameof(cryptoKey), (Exception?)null);
                keys = await this.ApiAccessService.RefreshJwt(this.DeviceGuid, cryptoKey, getRefreshTokenDto);
            }
        }
        catch (Exception ex)
        {
            string keysSerialized = System.Text.Json.JsonSerializer.Serialize(keys);
            await UiHelpers.ShowOkAlert("Fatal error trying to get keys. Contact IT");
            this.Logger.LogError(ex, "getRefreshTokenDto={getRefreshTokenDto} : expiration={expiration} : refreshToken{refreshToken} : keys={keys}", [getRefreshTokenDto, expiration, refreshToken, keysSerialized]);
            this.SetMessage(string.Empty);
            return null;
        }

        // for some reason we failed to get keys from server. we cannot continue
        if (keys is null || !keys.IsSuccessfulStatusCode || keys.Token is null)
        {
            await UiHelpers.ShowOkAlert($"Could not get keys. Contact IT. {keys?.StatusCode ?? HttpStatusCode.Unused}\npossible reasons:\nunknown device\nbad username password");
            this.SetMessage(string.Empty);
            return null;
        }

        // save keys
        this.SetMessage("Saving keys");
        if (!string.IsNullOrWhiteSpace(keys.Pkey))
            await Settings.SetCryptoApiKey(keys.Pkey);
        await Settings.SetJwtToken(keys.Token.Token);
        await Settings.SetJwtTokenExpiration(keys.Token.Expiration);
        await Settings.SetJwtRefresh(keys.Token.RefreshToken);
        await Settings.SetJwtRefreshExpiration(keys.Token.RefreshExpiration);
        this.ApiToken = keys.Token.RefreshToken;
        this.ApiTokenExpiration = keys.Token.RefreshExpiration.ToLocalTime();

        return keys;
    }
    /// <summary>
    /// Clears the message label, sets app to not busy and optionally displays an alert with an optional title
    /// </summary>
    /// <param name="alertMessage"></param>
    /// <param name="header"></param>
    /// <returns></returns>
    private async Task EndProcessing(string? alertMessage, string? header = null)
    {
        if (alertMessage != null && header != null)
            await UiHelpers.ShowOkAlert(alertMessage, header);
        else if (alertMessage != null)
            await UiHelpers.ShowOkAlert(alertMessage);
        this.SetMessage(string.Empty);
        this.IsBusy = false;
    }
    private async ValueTask<bool> BeginProcessing()
    {
        if (this.IsBusy)
        {
            await UiHelpers.ShowOkAlert("Please wait!");
            return false;
        }

        this.IsBusy = true;

        return true;
    }
    private double? CredentialsLayoutHeight { get; set; } = 146; // height the control takes up naturally
    /// <summary>
    /// Expands/collapsed the container which allows user to enter username and password.
    /// </summary>
    /// <param name="layoutControl"></param>
    /// <returns></returns>
    private Task AnimateCredentialsLayout(StackBase layoutControl)
    {
        int multiplier = layoutControl.Height < 1 ? 1 : -1;
        double targetHeight = layoutControl.Height < 1 ? 0 : this.CredentialsLayoutHeight!.Value;

        layoutControl.MaximumHeightRequest = this.CredentialsLayoutHeight!.Value;

        new Animation(delta => layoutControl.HeightRequest = targetHeight + (this.CredentialsLayoutHeight.Value * delta * multiplier))
            .Commit(layoutControl, nameof(layoutControl));

        return Task.CompletedTask;
    }
    /// <summary>
    /// Validates the value for the Endpoint URL, the existence of internet connection and the connection is made to a party that can respond with the response we except.
    /// </summary>
    /// <returns></returns>
    private async Task<bool> ValidateConnection()
    {
        string result;

        if (string.IsNullOrWhiteSpace(this.ApiEndpoint))
            return this.IsApiEndPointValid = false;

        if (this.ApiAccessService.EndPoint != this.ApiEndpoint)
            this.ApiAccessService.EndPoint = this.ApiEndpoint;

        if (string.IsNullOrWhiteSpace(this.ApiAccessService.EndPoint))
            return this.IsApiEndPointValid = false;

        this.HasInternet = Connectivity.Current.NetworkAccess == NetworkAccess.Internet;

        if (!this.HasInternet)
            return this.IsApiEndPointValid = false;

        try
        {
            result = await this.ApiAccessService.TestConnection(this.DeviceGuid, this.DeviceId);
            this.IsApiEndPointValid = (result == $"{this.DeviceGuid}{this.DeviceId}");
        }
        catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException)
        {
            this.Logger.LogError(ex, "Endpoint={Endpoint} : DeviceGuid={DeviceGuid} : DeviceId={DeviceId}", this.ApiEndpoint, this.DeviceGuid, this.DeviceId);
            return this.IsApiEndPointValid = false;
        }

        return !string.IsNullOrWhiteSpace(result) && result.Trim('"') == $"{this.DeviceGuid:N}{this.DeviceId}";
    }
    /// <summary>
    /// Loads the dropdowns for Locations and Departments and sets the selected values if a valid network connection is found.
    /// </summary>
    /// <returns></returns>
    private async Task LoadLocationsDepartments()
    {
        //if (string.IsNullOrWhiteSpace(this.ApiEndpoint) || !this.IsApiEndPointValid || string.IsNullOrWhiteSpace(this.ApiAccessService?.EndPoint))
        if (! (await this.ValidateConnection()))
            return;
        this.Locations.Clear();
        this.Departments.Clear();

        GetDeviceDto getDeviceDto = new()
        {
            DeviceId = Settings.DeviceGuid,
            ForceRefresh = true,
            IncludeDepartment = true,
            IncludeLocation = true
        };
        string? crypto = await Settings.GetCryptoApiKey();

        if (!string.IsNullOrWhiteSpace(crypto))
        {
            // we do not want to give a user the right to change location after setup, but we can change it in the DB
            // so we update the location and department if it has changed
            var result = await this.ApiAccessService.GetDevice(Settings.DeviceGuid, getDeviceDto, crypto);
            if (result.IsSuccessfulStatusCode && result.Value?.DepartmentsToLocations is not null)
            {
                Settings.LocationId = result.Value.DepartmentsToLocations.LocationId;
                Settings.DepartmentId = result.Value.DepartmentsToLocations.DepartmentId;
            }
        }

        Guid selectedLocationId = Settings.LocationId;
        Guid selectedDepartmentId = Settings.DepartmentId;
        foreach (LocationDto location in await this.ApiAccessService.GetLocations(this.DeviceGuid))
            this.Locations.Add(location);
        foreach (DepartmentDto department in await this.ApiAccessService.GetDepartments(this.DeviceGuid))
            this.Departments.Add(department);
        this.SelectedLocation = this.Locations.FirstOrDefault(l => l.Id == selectedLocationId);
        this.SelectedDepartment = this.Departments.FirstOrDefault(l => l.Id == selectedDepartmentId);
    }
    /// <summary>
    /// Sets the text for the message label and optionally sets the color based on isError parameter value.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="isError"></param>
    private void SetMessage(string message, bool isError = false)
    {
        this.Message = message;
        this.MessageColor = isError ? ConfigurationsViewModel.ErrorColor : ConfigurationsViewModel.SuccessColor;
    }
    /// <summary>
    /// Displays an alert using the calling method as a lookup key for retreiving the corresponding value from PopupMessages. 
    /// Value obtained from PopupMessages is used as the message text in the alert displayed.
    /// </summary>
    /// <param name="title"></param>
    /// <returns></returns>
    private Task ShowHelpPopup([CallerMemberName] string title = "")
    {
        if (App.Current?.MainPage is not null)
            return App.Current.MainPage.DisplayAlert(this.CleanHelpName(title), this.PopupMessages[title], "OK");
        return Task.CompletedTask;
    }
    /// <summary>
    /// Takes the string passed in and removes Help from the string, then converts it to proper case string.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private string CleanHelpName(string name)
    {
        StringBuilder output = new StringBuilder();
        bool firstPass = true;

        foreach(char kar in name.Replace("Help", ""))
        {
            if (char.IsUpper(kar) && !firstPass)
                output.Append(' ');
            output.Append(kar);
            firstPass = false;
        }

        return output.ToString();
    }
    /// <summary>
    /// Collection of messages used in help popups
    /// </summary>
    private IDictionary<string, string> PopupMessages = new Dictionary<string, string>()
    {
        {nameof(ConfigurationsViewModel.SelectLocationHelp), "The location is the physical location where the device hosting this app is located.\nA location may hold multiple departments." },
        {nameof(ConfigurationsViewModel.SelectDepartmentHelp), "The department is a organizational group inside a location. The device hosting this application should be dedicated to a department within a location.\nA department can have multiple devices." },
        {nameof(ConfigurationsViewModel.SelectPunchTypeHelp), "The default punch type determines if the punch/login screen will display inputs for username and password.\nType Barcode does not display username/password inputs.\nWhile a user can switch, when a screen times out it will return to the default punch type.\nThe idle screen is the screen that is displayed after timeout redirection occurs." },
        {nameof(ConfigurationsViewModel.ApiEndpointHelp), "The API Endpoint is the URL with which this application connects to, in order to update and/or fetch data." },
        {nameof(ConfigurationsViewModel.DeviceIdHelp), "The Device ID is a human friendly unique ID for the device this application is installed on. It is auto-generated during the registration process." },
        {nameof(ConfigurationsViewModel.DeviceGuidHelp), "The Device GUID is the defacto unique ID for the device this application is installed on." },
        {nameof(ConfigurationsViewModel.ApiTokenHelp), "The API token is a secret which is renewed regulary by the API. This values allows the application to authenticate with the API." },
        {nameof(ConfigurationsViewModel.ApiTokenExpirationHelp), "The expiration of the currently active API token." },
        {nameof(ConfigurationsViewModel.IsPublicDeviceHelp), "Whether this application is installed on a device where its primary purpose is to be used by multiple individuals. If so, it is considered a public device." },
    };
    /// <summary>
    /// Validates the value in the fields of the UI, and checks if internet is available to validate connection to API
    /// </summary>
    /// <returns></returns>
    private async Task<bool> Validate()
    {
        bool isValid = true;
        bool hasValidConnection = false;

        isValid = this.IsSelectedLocationValid = (this.SelectedLocation is not null);
        if (isValid)
            this.SelectedDepartment = this.Departments.FirstOrDefault(d => d.Name == this.SelectedLocation?.Name);
        isValid = (this.IsSelectedDepartmentValid = (this.SelectedDepartment is not null)) && isValid;

        hasValidConnection = this.IsApiEndPointValid = !string.IsNullOrWhiteSpace(this.ApiEndpoint);
        hasValidConnection = hasValidConnection && (this.IsApiEndPointValid = await this.ValidateConnection());

        isValid = (this.IsDeviceIdValid = !string.IsNullOrWhiteSpace(this.DeviceId) && this.DeviceId != default) && isValid;
        //isValid = (this.IsApiTokenValid = !string.IsNullOrWhiteSpace(this.ApiToken)) && isValid;

        return isValid && hasValidConnection;
    }
    /// <summary>
    /// Load saved or default values into class properties and members
    /// </summary>
    private void LoadProperties()
    {
        this.Locations = new ObservableCollection<LocationDto>();
        this.Departments = new ObservableCollection<DepartmentDto>();
        this.PunchTypes = new ObservableCollection<PunchTypeDto>() { PunchTypeDto.Barcode, PunchTypeDto.Domain };
        this.IdleScreens = new ObservableCollection<IUiPageMeta>();

        foreach (IUiPageMeta meta in UiPageMeta.Metas.Where(p => p.RedirectChild is not null))
            this.IdleScreens.Add(meta);

        this.AppVersion = AppInfo.Current.VersionString;
        this.SelectedTheme = Settings.CurrentTheme;
        this.DefaultPunchType = Settings.DefaultPunchType;
        this.DeviceId = Settings.DeviceId;
        this.DeviceGuid = Settings.DeviceGuid;
        this.ApiEndpoint = Settings.ApiEndpoint;
        this.SelectedIdleScreen = Settings.IdleScreen;
        this.SelectedScreenTimeout = Settings.IdleScreenSeconds;
        this.IsPublicDevice = Settings.IsPublicDevice;
        this.IsNotRegistered = !Settings.IsRegistered;
        // if we try to call the async settings any other way, the application hangs
        Task.Run(async () => await this.LoadAsyncProperties());
        // set these to true by default so that we are not presented with invalid fields on initial page load
        this.IsSelectedLocationValid = true;
        this.IsSelectedDepartmentValid = true;
        this.IsDeviceIdValid = true;
        // make sure our message label is clear and color is reset
        this.Message = string.Empty;
        this.MessageColor = ConfigurationsViewModel.SuccessColor;

        this.IsLightTheme = this.SelectedTheme == AppTheme.Light;
        base.OnPropertyChanged(nameof(this.IsLightTheme)); // this property has no INotify of its own
        base.OnPropertyChanged(nameof(this.DefaultPunchType)); // this is goofy, because picker wont recognize the initial value

        if (this.ApiAccessService is not null && this.ApiAccessService.EndPoint != this.ApiEndpoint)
            this.ApiAccessService.EndPoint = this.ApiEndpoint;
    }
    private async Task LoadAsyncProperties()
    {
        // bundle these together and call once, to keep application from hanging
        this.ApiTokenExpiration = (await Settings.GetJwtRefreshExpiration()) ?? DateTime.MinValue;
        this.ApiToken = (await Settings.GetJwtRefresh()) ?? string.Empty;
    }
    private void SetCommands()
    {
        this.RegisterDeviceCommand = new AsyncRelayCommand(this.RegisterDevice);
        this.SaveCommand = new AsyncRelayCommand(this.Save);
        this.TestConnectionCommand = new AsyncRelayCommand(this.TestConnection);
        this.ReAuthenticateJwtCommand = new AsyncRelayCommand<StackBase>(this.ReAuthenticateJwt);
        this.CheckForUpdatesCommand = new AsyncRelayCommand(this.CheckForUpdates);
        this.ViewLogsCommand = new AsyncRelayCommand(this.ViewLogs);
        this.ThemeToggledCommand = new AsyncRelayCommand<bool>((s,t) => this.ThemeToggled(s,t));
        this.SubmitJwtRefreshCommand = new AsyncRelayCommand<object>(this.SubmitJwtRefresh);

        this.SelectLocationHelpCommand = new AsyncRelayCommand(this.SelectLocationHelp);
        this.SelectDepartmentHelpCommand = new AsyncRelayCommand(this.SelectDepartmentHelp);
        this.SelectPunchTypeHelpCommand = new AsyncRelayCommand(this.SelectPunchTypeHelp);
        this.ApiEndpointHelpCommand = new AsyncRelayCommand(this.ApiEndpointHelp);
        this.DeviceIdHelpCommand = new AsyncRelayCommand(this.DeviceIdHelp);
        this.DeviceGuidHelpCommand = new AsyncRelayCommand(this.DeviceGuidHelp);
        this.ApiTokenHelpCommand = new AsyncRelayCommand(this.ApiTokenHelp);
        this.ApiTokenExpirationHelpCommand = new AsyncRelayCommand(this.ApiTokenExpirationHelp);
        this.IsPublicDeviceHelpCommand = new AsyncRelayCommand(this.IsPublicDeviceHelp);
    }
    #endregion
}
