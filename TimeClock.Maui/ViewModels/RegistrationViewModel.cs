using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using TimeClock.Core.Models.ApiDtos;
using TimeClock.Core.Models.EntityDtos;
using TimeClock.Maui.Controls;
using TimeClock.Maui.Helpers;
using TimeClock.Maui.Services;

namespace TimeClock.Maui.ViewModels;
internal class RegistrationViewModel : ObservableObject
{
    private string _message = string.Empty;
    private bool _needsLocation;
    private bool _finishButtonEnabled;
    private bool _showProgress;
    private double _progress;
    private Color _messageColor = Colors.White;
    private LocationDto? _selectedLocation;
    private DepartmentDto? _selectedDepartment;

    private ITimeClockApiAccessService ApiAccessService { get; init; }
    private ILogger<RegistrationViewModel> Logger { get; init; }
    private ResultValues<string>? Keys { get; set; }
    private bool AnimateMessages { get; set; } = true;

    public RegistrationViewModel(ITimeClockApiAccessService apiService, ILogger<RegistrationViewModel> logger)
    {
        this.ApiAccessService = apiService;
        this.Logger = logger;
        this.Locations = new();
        this.Message = "Welcome to the TimeClock. Setup in process, hang on for a sec.";
        this.FinishRegistrationCommand = new AsyncRelayCommand(this.FinishRegistration);
        this.SelectedLocationChangedCommand = new AsyncRelayCommand(this.SelectedLocationChanged);
        this.Locations = new();
        this.Departments = new();
        this.ShowProgress = true;
    }

    #region Public Properties
    public string? UserName { get; set; }

    public string? Password { get; set; }

    public string? EmployeeId { get; set; }

    internal StackBase? CredentialsContainer { get; set; }

    internal Label? MessagesLabel { get; set; }

    public string Message
    {
        get => this._message;
        set => base.SetProperty(ref this._message, value);
    }

    public Color MessageColor
    {
        get => this._messageColor;
        set => base.SetProperty(ref this._messageColor, value);
    }

    public bool NeedsLocation
    {
        get => this._needsLocation;
        set => base.SetProperty(ref this._needsLocation, value);
    }

    public bool FinishButtonEnabled
    {
        get => this._finishButtonEnabled;
        set => base.SetProperty(ref this._finishButtonEnabled, value);
    }

    public bool ShowProgress
    {
        get => this._showProgress;
        set => base.SetProperty(ref this._showProgress, value);
    }

    public double Progress
    {
        get => this._progress;
        set => base.SetProperty(ref this._progress, value);
    }

    public LocationDto? SelectedLocation
    {
        get => this._selectedLocation;
        set => base.SetProperty(ref this._selectedLocation, value);
    }

    public DepartmentDto? SelectedDepartment
    {
        get => this._selectedDepartment;
        set => base.SetProperty(ref this._selectedDepartment, value);
    }

    public ObservableCollection<LocationDto> Locations { get; private set; }

    public ObservableCollection<DepartmentDto> Departments { get; private set; }
    #endregion Public Properties

    public async Task BeginRegistration()
    {
        // wait 2 seconds before starting. Allows user to see initial message and "feel" something is happening
        await Task.Delay(2000).ContinueWith(async t =>
        {
            try
            {
                ResultValues<string>? keys;
                bool success = true;
                Settings.SetDeviceId();
                this.Progress += .2;

                success = await this.EstablishConnection();
                if (!success) return;
                this.Progress += .2;
                success = await this.GetUserGeolocation();
                if (!success) return;
                this.Progress += .2;
                keys = await this.GetApiTokens();
                if (keys is not null && keys.IsSuccessfulStatusCode)
                {
                    this.Keys = keys;
                    // we've done what we can, now need user to select location
                    await this.UpdateMessage("Looking high and low for all possible locations...");
                    await this.LoadLocations();
                    this.Progress += .2;

                    await this.RequestLocationFromUser();
                    this.Progress += .2;
                    this.ShowProgress = false;
                    this.MessagesLabel?.CancelAnimations();
                    this.MessagesLabel?.AbortAnimation("messageAnimation");
                    this.AnimateMessages = false;
                }
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "Method={method}", nameof(this.BeginRegistration));
                await this.UpdateMessage("Catastrophic failure. Please contact IT.");
            }
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }

    private async Task LoadLocations()
    {
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
        foreach (LocationDto location in await this.ApiAccessService.GetLocations(Settings.DeviceGuid))
            this.Locations.Add(location);
        foreach (DepartmentDto department in await this.ApiAccessService.GetDepartments(Settings.DeviceGuid))
            this.Departments.Add(department);
        this.SelectedLocation = this.Locations.FirstOrDefault(l => l.Id == selectedLocationId);
        this.SelectedDepartment = this.Departments.FirstOrDefault(l => l.Id == selectedDepartmentId);
    }

    private async Task<bool> EstablishConnection()
    {
        string? result;
        bool validResponse;
        string message;
        bool success = false;

        await this.UpdateMessage("Checking for connection");

        try
        {
            result = await this.ApiAccessService.TestConnection(Guid.Empty, Settings.DeviceId);
            validResponse = (result == $"{Guid.Empty:N}{Settings.DeviceId}");

            if (validResponse)
            {
                message = $"Connection established! Beginning setup...";
                success = true;
            }
            else
            {
                message = $"A connection was not established or the response was incorrect. Aborting setup.";
                success = false;
            }
        }
        catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException)
        {
            this.Logger.LogError(ex, "Method={Endpoint}", nameof(this.EstablishConnection));
            message = "Error occurred while connecting. Check internet.";
            success = false;
        }

        await this.UpdateMessage(message, !success);

        return success;
    }

    private async Task<bool> GetUserGeolocation()
    {
        bool isLocationRequired = PermissionsManager.IsLocationRequired();
        Location? geolocation = await PermissionsManager.GetOrRequestLocation(isRequired: isLocationRequired, getCached: !isLocationRequired);
        
        await this.UpdateMessage("Establishing location");

        if (isLocationRequired && geolocation is null)
        {
            await this.UpdateMessage("Location is required. Please enable Geolocation and try again.", true);
            return false;
        }

        Settings.Latitude = geolocation?.Latitude;
        Settings.Longitude = geolocation?.Longitude;

        await this.UpdateMessage("Congrats! Location established.");

        return true;
    }

    private async Task<ResultValues<string>?> GetApiTokens()
    {
        ResultValues<string>? keys;
        string? jwt;

        await this.UpdateMessage("Getting activation tokens...");

        if (string.IsNullOrWhiteSpace(jwt = await Settings.GetJwtToken()))
        {
            keys = await this.GetSetKeys();

            // GetSetKeys already alerts the user in the event of a failure
            if (keys is null || !keys.IsSuccessfulStatusCode)
            {
                return null;
            }
        }

        return new ResultValues<string>(System.Net.HttpStatusCode.OK, true, await Settings.GetCryptoApiKey(),
            new TokenPackage(jwt, (await Settings.GetJwtRefreshExpiration()).Value, (await Settings.GetJwtRefresh())!, (await Settings.GetJwtRefreshExpiration()).Value),
            null);
    }

    private async Task<ResultValues<string?>?> GetSetKeys(GetRefreshTokenDto? getRefreshTokenDto = null)
    {
        ResultValues<string?>? keys = null;
        string? refreshToken = null;
        DateTime expiration = DateTime.MinValue;
        Guid deviceId = Settings.DeviceGuid;
        string deviceName = Settings.DeviceId;
        CancellationToken cancellationToken = new CancellationTokenSource(TimeSpan.FromMinutes(5)).Token;

        try
        {
            refreshToken = await Settings.GetJwtRefresh();

            if (string.IsNullOrEmpty(refreshToken))
            {
                this.ApiAccessService.TimeOut = TimeSpan.FromMinutes(5);
                keys = await this.ApiAccessService.GetKeys(deviceId, deviceName, cancellationToken);
                this.ApiAccessService.TimeOut = default;
            }
            else
            {
                ArgumentNullException.ThrowIfNull(getRefreshTokenDto);

                string? cryptoKey;
                if (string.IsNullOrWhiteSpace(cryptoKey = await Settings.GetCryptoApiKey()))
                    throw new ArgumentNullException(nameof(cryptoKey), (Exception?)null);
                keys = await this.ApiAccessService.RefreshJwt(deviceId, cryptoKey, getRefreshTokenDto);
            }
        }
        catch (Exception ex)
        {
            string keysSerialized = System.Text.Json.JsonSerializer.Serialize(keys);
            this.Logger.LogError(ex, "getRefreshTokenDto={getRefreshTokenDto} : expiration={expiration} : refreshToken{refreshToken} : keys={keys}", [getRefreshTokenDto, expiration, refreshToken, keysSerialized]);

            await this.UpdateMessage("Fatal error trying to get tokens. Contact IT", true);

            return null;
        }

        // for some reason we failed to get keys from server. we cannot continue
        if (keys is null || !keys.IsSuccessfulStatusCode || keys.Token is null)
        {
            await this.UpdateMessage("Could not get tokens. Contact IT", true);
            return null;
        }

        // save keys
        await this.UpdateMessage("Tokens are good! xD");

        if (!string.IsNullOrWhiteSpace(keys.Pkey))
            await Settings.SetCryptoApiKey(keys.Pkey);
        await Settings.SetJwtToken(keys.Token.Token);
        await Settings.SetJwtTokenExpiration(keys.Token.Expiration);
        await Settings.SetJwtRefresh(keys.Token.RefreshToken);
        await Settings.SetJwtRefreshExpiration(keys.Token.RefreshExpiration);

        return keys;
    }

    private async Task RequestLocationFromUser()
    {
        this.NeedsLocation = true;
        await this.UpdateMessage("Please select the location for this device");
    }

    private async Task<bool> RegisterDevice(ResultValues<string> keys, string username, string password, int employeeId, Guid locationId, Guid departmentId)
    {
        ResultValues<Guid>? result = default;
        RegisterDeviceDto? data = null;

        await this.UpdateMessage("Registering device :-)\nYou are almost done!");

        try
        {
            data = new()
            {
                DepartmentId = departmentId,
                DeviceId = Settings.DeviceGuid,
                DeviceName = Settings.DeviceId,
                LocationId = locationId,
                RefreshToken = keys.Token!.RefreshToken,
                Username = username,
                Password = password,
                EmployeeId = employeeId,
                IsPublic = Settings.IsPublicDevice
            };
            // register device
            result = await this.ApiAccessService.RegisterDevice(keys.Pkey!, keys.Token.Token, data);
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "keys.Pkey={keys.Pkey} : keys.Token.Token{keys.Token.Token} : data{data}", [keys.Pkey, keys.Token?.Token, data?.AsJson()]);
            await this.UpdateMessage("Catastrophic failure. Please contact IT.", true);
            return false;
        }

        if (result?.Token is null)
        {
            await this.UpdateMessage("New keys were not found.\nThis may cause issues.\nTry and refresh keys.", true);
        }
        else
        {
            await this.UpdateMessage("Saving keys...");

            await Settings.SetJwtRefresh(result.Token.RefreshToken);
            await Settings.SetJwtRefreshExpiration(result.Token.RefreshExpiration);
            await Settings.SetJwtToken(result.Token.Token);
            await Settings.SetJwtTokenExpiration(result.Token.Expiration);
        }

        if (result is not null && result.IsSuccessfulStatusCode)
        {
            Settings.IsRegistered = true;
            Settings.RegisteredBy = result.Value;
        }

        string message = result?.IsSuccessfulStatusCode ?? false ? "Successfully Registered. Restart Required. App will now close. Please re-open." : "Could not register. Check credentials or Contact IT";
        await this.UpdateMessage(message, !(result?.IsSuccessfulStatusCode ?? false));
        if (result?.IsSuccessfulStatusCode ?? false)
            await UiHelpers.ShowOkAlert(message, "Hooray!");

        if (Application.Current?.MainPage is not null && Settings.IsRegistered)
            Application.Current.CloseWindow(Application.Current.MainPage.Window);

        return result?.IsSuccessfulStatusCode ?? false;
    }

    public IAsyncRelayCommand FinishRegistrationCommand { get; set; }
    public IAsyncRelayCommand SelectedLocationChangedCommand { get; set; }
    private async Task FinishRegistration()
    {
        int employeeId;
        if (string.IsNullOrWhiteSpace(this.UserName))
        {
            await this.UpdateMessage("Your Windows user name is required..", true);
            return;
        }
        if (string.IsNullOrWhiteSpace(this.Password))
        {
            await this.UpdateMessage("Your Windows password is required..", true);
            return;
        }
        if (string.IsNullOrWhiteSpace(this.EmployeeId))
        {
            await this.UpdateMessage("Your JDE employee number is required..", true);
            return;
        }
        if (!int.TryParse(this.EmployeeId, out employeeId))
        {
            await this.UpdateMessage("Your JDE employee number seems invalid..", true);
            return;
        }
        if (this.SelectedLocation is null || this.SelectedDepartment is null)
        {
            await this.UpdateMessage("Selected location is invalid..", true);
            return;
        }
        if (this.Keys is null)
        {
            await this.UpdateMessage("I lost my keys!. Contact IT.", true);
            return;
        }

        this.FinishButtonEnabled = false;
        await this.RegisterDevice(this.Keys, this.UserName, this.Password, employeeId, this.SelectedLocation.Id, this.SelectedDepartment.Id);
        this.FinishButtonEnabled = true;
    }
    private async Task SelectedLocationChanged()
    {
        this.SelectedDepartment = this.Departments.FirstOrDefault(d => d.Name == this.SelectedLocation?.Name);
        Settings.LocationId = this.SelectedLocation?.Id ?? default;
        Settings.DepartmentId = this.SelectedDepartment?.Id ?? default;
        Settings.LocationDivision = this.SelectedLocation?.DivisionCode;
        Settings.IsPublicDevice = true;

        await this.UpdateMessage("Great location! Now we need to verify you.\nEnter your credentials, then click 'Finish Registration'");

        if (this.CredentialsContainer is not null)
        {
            this.FinishButtonEnabled = true;
            await this.AnimateCredentialsLayout(this.CredentialsContainer);
        }
        else
            await this.UpdateMessage("We ran into an issue. Couldn't locate credentials form.\nSorry. Try again or contact IT.");
    }

    private static readonly Color[] PositiveColors = [
        Color.FromRgba(255, 149, 0, 1), // orange
        Color.FromRgba(52, 199, 89, 1), // green
        Color.FromRgba(146, 200, 255, 1), // blue
        Color.FromRgba(249, 146, 255, 1), // pink
        Color.FromRgba(146, 244, 255, 1), // aqua
        Color.FromRgba(255, 232, 146, 1), // yellow
        ];
    private static readonly Func<Label, Task>[] TextAnimations = [
        ZoomFade,
        SinkSpin
        ];
    private static Random Random = new Random();
    private Task UpdateMessage(string message, bool isFailure = false)
    {
        this.MessagesLabel?.CancelAnimations();
        this.Message = message;
        Color color = isFailure ? Colors.Red : PositiveColors[Random.Next(0, PositiveColors.Length)];
        ColorTransition colorTransitionTo = new(this.MessageColor, color, 16u);
        ColorTransition colorTransitionFrom = new(color, this.MessageColor, 16u);
        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (this.MessagesLabel is not null && this.AnimateMessages)
            {
                var parent = new Animation();
                var colorChangeTo = new Animation(callback: v => this.MessagesLabel.TextColor = colorTransitionTo.GetNext(v), start: 0, end: 16u, easing: Easing.CubicInOut);
                var colorChangeFrom = new Animation(callback: v => this.MessagesLabel.TextColor = colorTransitionFrom.GetNext(v), start: 0, end: 16u, easing: Easing.CubicInOut);
                parent.Add(0,.5,colorChangeTo);
                parent.Add(.5,1,colorChangeFrom);
                parent.Add(0, .5, new Animation((v) => this.MessagesLabel.Scale = v, 1, 1.25));
                parent.Add(.5, 1, new Animation((v) => this.MessagesLabel.Scale = v, 1.25, 1));
                parent.Commit(this.MessagesLabel, length: 2000, name: "messageAnimation", easing: Easing.CubicInOut, finished: (v, c) =>
                {
                    color = isFailure ? Colors.Red : PositiveColors[Random.Next(0, PositiveColors.Length)];
                    colorTransitionTo = new(this.MessageColor, color, 16u);
                    colorTransitionFrom = new(color, this.MessageColor, 16u);
                }, repeat: () => true);
            }
            else
            {
                if (this.MessagesLabel is not null)
                    this.MessagesLabel.TextColor = isFailure ? Colors.Red : Colors.White;
                else
                    this.MessageColor = isFailure ? Colors.Red : Colors.White;
                this.Message = message;
            }
        });

        return Task.CompletedTask;
    }

    private static Task ZoomFade(Label label)
    {
        return Task.WhenAll(
            label.ScaleTo(5, easing: Easing.CubicInOut),
            label.TranslateTo(-200, -200, easing: Easing.CubicInOut),
            label.FadeTo(0, easing: Easing.CubicInOut)
        );
    }

    private static Task SinkSpin(Label label)
    {
        return Task.WhenAll(
            label.ScaleTo(0, easing: Easing.CubicInOut),
            label.RotateTo(1800, easing: Easing.CubicInOut),
            label.FadeTo(0, easing: Easing.CubicInOut)
        );
    }

    private Task JumpOut(Label label)
    {



        return Task.WhenAll(
            label.ScaleTo(0, easing: Easing.CubicInOut),
            label.RotateTo(1800, easing: Easing.CubicInOut),
            label.FadeTo(0, easing: Easing.CubicInOut)
        );
    }

    private double? CredentialsLayoutHeight { get; set; } = 190; // height the control takes up naturally
    private Task AnimateCredentialsLayout(StackBase layoutControl)
    {
        int multiplier = layoutControl.Height < 1 ? 1 : -1;
        double targetHeight = layoutControl.Height < 1 ? 0 : this.CredentialsLayoutHeight!.Value;

        layoutControl.MaximumHeightRequest = this.CredentialsLayoutHeight!.Value;

        new Animation(delta => layoutControl.HeightRequest = targetHeight + (this.CredentialsLayoutHeight.Value * delta * multiplier))
            .Commit(layoutControl, nameof(layoutControl));

        return Task.CompletedTask;
    }
}
