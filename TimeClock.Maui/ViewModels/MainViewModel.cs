using CommunityToolkit.Mvvm.Input;
using MetroLog;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Windows.Input;
using TimeClock.Core;
using TimeClock.Core.Models.ApiDtos;
using TimeClock.Core.Models.EntityDtos;
using TimeClock.Maui.Helpers;
using TimeClock.Maui.Services;
using TimeClock.Maui.ViewModels.Shared;

namespace TimeClock.Maui.ViewModels;

internal sealed class MainViewModel : ViewModelBase, IDisposable
{
    #region Private Backing Fields
    private static readonly string DefaultLocationText = "Check Internet";
    private DateTime _dateTime;
    private DateTime _clockDateTime;
    private DateTime _date;
    private string _location;
    private string _department;
    private string _password;
    private new PunchTypeDto _punchType;
    private string _targetPageDescription;
    private string _targetPagePath;
    private string _targetActionDescription;
    private bool _disposedValue;
    private bool _isShown;
    private Color _titleColor = Colors.Transparent;// Color.FromArgb("FF9500");
    private PunchStatusDto? _punchStatus;
    private HttpStatusCode _httpStatusCode;
    private bool _isMissingPunch;
    private DateOnly? _missingPunchDate;
    private ITimeClockApiAccessService ApiAccessService { get; init; }
    private ILogger<MainViewModel> Logger { get; init; }
    private CancellationTokenSource TokenSource { get; set; }
    private Timer? Timer { get; set; }
    #endregion Private Backing Fields

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public MainViewModel(ITimeClockApiAccessService apiAccessService, ILogger<MainViewModel> logger):base()
    {
        this.ApiAccessService = apiAccessService;
        this.Logger = logger;
        // must make sure to set defaults in method below, as CS8618 is disabled
        this.SetPropertyDefaults();
        this.SetCommands();
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.


    #region Public Properties
    public DateTime DateTime
    {
        get => this._dateTime;
        private set => base.SetProperty(ref this._dateTime, value);
    }
    // we use ClockDateTime for the DateTime displayed.
    // because if we modify DateTime, the base class will call refresh to reload data.
    public DateTime ClockDateTime
    {
        get => this._clockDateTime;
        private set => base.SetProperty(ref this._clockDateTime, value);
    }

    public DateTime Date
    {
        get => this._date;
        private set => base.SetProperty(ref this._date, value);
    }

    public string Location
    {
        get => Preferences.Get(nameof(this.Location), MainViewModel.DefaultLocationText);
        set
        {
            Preferences.Set(nameof(this.Location), value);
            base.SetProperty(ref this._location, value);
        }
    }
    public string Department
    {
        get => Preferences.Get(nameof(this.Department), MainViewModel.DefaultLocationText);
        set
        {
            Preferences.Set(nameof(this.Department), value);
            base.SetProperty(ref this._department, value);
        }
    }
    public string TargetPageDescription
    {
        get => this._targetPageDescription;
        set => base.SetProperty(ref this._targetPageDescription, value);
    }
    public string TargetPagePath
    {
        get => this._targetPagePath;
        set => base.SetProperty(ref this._targetPagePath, value);
    }
    public string TargetActionDescription
    {
        get => this._targetActionDescription;
        set => base.SetProperty(ref this._targetActionDescription, value);
    }
    public string Password { get => this._password; set => base.SetProperty(ref this._password, value); }
    public new PunchTypeDto PunchType
    {
        get => Settings.ActivePunchType;
        set { Settings.ActivePunchType = value; base.SetProperty(ref this._punchType, value); }
    }
    public bool IsLoginSuccess { get; private set; }
    public bool IsShown 
    { 
        get => this._isShown; 
        set
        {
            this._isShown = value;

            // the timer continues to run and execute even after this login screen is navigated away from
            // we dispose of the timer so that we do not consume resources if not needed
            // if we have idle screen redirect set, do not use this feature
            // // 
            this.SetTimer(Settings.IdleScreenSeconds < 1 && !this._isShown);
        }
    }
    public bool IsPunchHandled { get; private set; }
    public Color TitleColor
    {
        get => this._titleColor;
        set => base.SetProperty(ref this._titleColor, value);
    }
    public PunchStatusDto? CurrentPunchStatus
    {
        get => this._punchStatus;
        set => base.SetProperty(ref this._punchStatus, value);
    }
    public HttpStatusCode HttpStatusCode
    {
        get => this._httpStatusCode;
        set => base.SetProperty(ref this._httpStatusCode, value);
    }
    public bool IsMissingPunch
    {
        get => this._isMissingPunch;
        set => base.SetProperty(ref this._isMissingPunch, value);
    }
    public DateOnly? MissingPunchDate
    {
        get => this._missingPunchDate;
        set => base.SetProperty(ref this._missingPunchDate, value);
    }
    #endregion Public Properties


    #region Public Commands
    public IAsyncRelayCommand PunchCommand { get; private set; }
    public ICommand TogglePunchTypeCommand { get; private set; }
    #endregion Public Commands


    #region Public Methods

    /// <summary>
    /// Forces INotify to be raised on PunchType
    /// </summary>
    public void RefreshPunchType()
    {
        this.OnPropertyChanged(nameof(this.PunchType));
    }
    /// <summary>
    /// Clears properties which are bound to UI
    /// </summary>
    public void Clear()
    {
        this.UserName = string.Empty; this.Password = string.Empty; this.UserId = default;
    }
    #endregion Public Methods


    #region Private Command Actions
    private async Task RequestAccess()
    {
        App? app = App.Current as App;
        if (app is not null)
            app.LastActivity = DateTime.Now;

        IUiPageMeta? pageMeta = UiPageMeta.Metas.FirstOrDefault(p => p.Uri.Equals(this.TargetPagePath.Trim(CommonValues.PathSeparator), StringComparison.CurrentCultureIgnoreCase));

        if (pageMeta is null) throw new Exception($"Unknown UiPageMeta with URI {this.TargetPagePath}");

        if (pageMeta == UiPageMeta.Punch)
        {
            await this.DoPunch();
            return;
        }

        await this.AuthRedirect(pageMeta);
    }
    private async Task DoPunch()
    {
        if (this.Location.Contains("error"))
        {
            if (!(await this.LoadLocation()))
                return;
        }

        CreatePunchEntryDto punchEntry = new()
        {
            ActionById = null,
            PunchAction = PunchActionDto.Self,
            DateTime = DateTime.Now,
            DeviceId = Settings.DeviceGuid,
            Id = Guid.NewGuid(),
            Password = this.Password,
            UserName = this.UserName,
            PunchType = this.PunchType,
            IncludeUser = true,
            LocationDivisionCode = Settings.LocationDivision,
            UnionCode = this.Union
        };

        this.Clear();

        bool isLocationRequired = PermissionsManager.IsLocationRequired();
        Location? geolocation = await PermissionsManager.GetOrRequestLocation(isRequired: isLocationRequired, getCached: !isLocationRequired);

        if (isLocationRequired && geolocation is null)
        {
            await UiHelpers.ShowOkAlert(CommonValues.GeoLocRequired);
            return;
        }

        punchEntry.Latitude = geolocation?.Latitude ?? Settings.Latitude;
        punchEntry.Longitude = geolocation?.Longitude ?? Settings.Longitude;

        ResultValues<PunchEntryDto?> result = await this.ApiAccessService.CreatePunchEntry(punchEntry.DeviceId, (await Settings.GetCryptoApiKey()) ?? string.Empty, punchEntry);

        this.HttpStatusCode = result.StatusCode;
        this.DateTime = punchEntry.DateTime.RoundMinutes(15);
        this.UserFirstName = result.Value?.User?.FirstName ?? string.Empty;
        this.UserLastName = result.Value?.User?.LastName ?? string.Empty;
        //this.UserName = result.Value?.User?.UserName ?? string.Empty;
        this.CurrentPunchStatus = result.Value?.StablePunchStatus;
        this.IsMissingPunch = result.Value?.WorkPeriod?.IsPreviousMissingPunch ?? false;
        this.MissingPunchDate = result.Value?.WorkPeriod?.PreviousMissingPunchDate;

        if (result.StatusCode == HttpStatusCode.SeeOther)
        {
            ShellNavigationQueryParameters parameters = new() { { nameof(UserDto), result.Value!.User! }, { nameof(this.PunchType), (int)punchEntry.PunchType },
                { nameof(this.Password), punchEntry.Password }, { "Barcode", punchEntry.PunchType == PunchTypeDto.Barcode ? punchEntry.UserName : null } };
            
            this.IsPunchHandled = true;
            this.IsLoginSuccess = true;
            await Shell.Current.GoToAsync(UiPageMeta.JobTypeStepSelect.Route, true, parameters);
            return;
        }

        if (!(this.IsLoginSuccess = result.IsSuccessfulStatusCode))
        {
            this.DateTime = default;
            this.UserFirstName = string.Empty;
            this.UserLastName = string.Empty;
            this.UserName = string.Empty;
            this.CurrentPunchStatus = default;
            this._isMissingPunch = false;
            this.MissingPunchDate = null;
            this.Logger.Log(Microsoft.Extensions.Logging.LogLevel.Error, result.StatusCode.ToString());
        } 
        return;
    }

    private async Task AuthRedirect(IUiPageMeta pageMeta)
    {
        Shell.Current.Navigation.ClearStack();
        string? cryptoKey = await Settings.GetCryptoApiKey();
        Guid deviceId = Settings.DeviceGuid;

        if (string.IsNullOrWhiteSpace(cryptoKey)) throw new Exception("Secret key not found!");

        ValidateClaimDto data = new()
        {
            Claims = pageMeta.Claims,
            DeviceId = deviceId,
            Password = this.Password,
            UserName = this.UserName,
            PunchTypeDto = this.PunchType
        };
        
        UserDto? user = (await this.ApiAccessService.RequestAccess(deviceId, cryptoKey, data))?.Value;

        if (user is null)
        {
            this.IsLoginSuccess = false;
            return;
        }

        this.IsPunchHandled = true;
        ShellNavigationQueryParameters parameters = new(){ { nameof(UserDto), user }, { nameof(this.PunchType), (int)this.PunchType } };
        this.IsLoginSuccess = true;
        this.Clear();
        await Shell.Current.GoToAsync(this.TargetPagePath, true, parameters);
        return;
    }
    private void TogglePunchType()
    {
        //if (this.PunchType == PunchTypeDto.Barcode)
        //    this.PunchType = Settings.DefaultPunchType == PunchTypeDto.Barcode ? PunchTypeDto.Domain : Settings.DefaultPunchType;
        //else
        //    this.PunchType = PunchTypeDto.Barcode;
        this.PunchType = (this.PunchType == PunchTypeDto.Barcode) ? PunchTypeDto.Domain : PunchTypeDto.Barcode;
    }

    protected override async Task Refresh()
    {
        // ******************************************************************** \\
        // when toggling the barcode/non-barcode and then changing tabs, the currently active tab will not update.
        // this is due to the binding being set before the UI gets presented and retaining its former UI state.
        // so best bet is to refresh any bindings which are UI related.
        // This issue does not happen if the now current tab was not activated/opened before,
        // as it will not have a previous state to fall back on.
        // so we refresh/re-force bindings whenever a refresh is requested.
        // ******************************************************************** \\
        base.OnPropertyChanged(nameof(this.PunchType));
        await this.LoadLocation();
        //return Task.CompletedTask;
    }
    #endregion Private Command Actions


    #region Private Methods/Helpers
    private static string GetSalutation()
    {
        int now = DateTime.Now.TimeOfDay.Hours;

        switch (now)
        {
            case < 12:
                return "Good Morning";
            case < 17:
                return "Good Afternoon";
            default:
                return "Good Evening";
        }
    }
    private void SetTimer(bool destroy)
    {
        if (destroy)
        {
            this.Timer?.Dispose();
            return;
        }

        var app = App.Current as App;

        if (Settings.ActivityRedirectActive && app is not null)
            app.ScreenChangeSettings = new Tuple<int, IUiPageMeta>(Settings.IdleScreenSeconds, Settings.IdleScreen);
        var magicSync = 60 - DateTime.Now.Second;

        this.Timer = new Timer(new TimerCallback((that) =>
        {
            // DO NOT use "me". Quickly changing screens can/will cause NavigationFailedException
            //MainViewModel me = (MainViewModel)that!;
            this.ClockDateTime = DateTime.Now;
            if (this.Date != this.ClockDateTime.Date)
                this.Date = this.ClockDateTime.Date;
            if (Settings.ActivityRedirectActive && app is not null)
                app.NonActivityRedirect();
        }), this, TimeSpan.FromSeconds(magicSync), TimeSpan.FromSeconds(1));
    }

    private void SetPropertyDefaults()
    {
        this.TokenSource = new CancellationTokenSource(CommonValues.ApiCancellationTokenLimit);
        this._dateTime = DateTime.Now;
        this.ClockDateTime = DateTime.Now;
        this.Date = DateTime.Now.Date;
        this._userName = string.Empty;
        this._password = string.Empty;
        this._targetPageDescription = string.Empty;
        this._targetPagePath = string.Empty;
        this._targetActionDescription = string.Empty;
        this._punchType = Settings.ActivePunchType;
    }

    private void SetCommands()
    {
        this.PunchCommand = new AsyncRelayCommand(this.RequestAccess);
        this.TogglePunchTypeCommand = new Command(this.TogglePunchType);
    }

    private async Task<bool> LoadLocation()
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(this.Location) && !this.Location.Contains("error:") && !this.Location.Contains(DefaultLocationText))
                return true;
            
            var result = (await this.ApiAccessService.GetLocation(Settings.DeviceGuid, Settings.LocationId, this.TokenSource.Token));

            if (!result.IsSuccessfulStatusCode)
            {
                if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    this.Location = "error: Please login";
                }
                else if (result.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
                {
                    this.Location = DefaultLocationText;
                }
                else
                {
                    this.Location = $"error: {Enum.GetName(result.StatusCode)}";
                }

                return false;
            }

            this.Location = result.Value?.Name ?? "error: Unknown";
            this.Department = this.Location;

            return this.Location is not null && !this.Location.Contains("error:");
        }catch(Exception ex)
        {
            this.Logger.LogError(ex, "trying to load location with {LocationId} from {DeviceId}", Settings.LocationId, Settings.DeviceGuid);
        }
        return false;
    }
    #endregion Private Methods/Helpers


    #region IDisposable
    private void Dispose(bool disposing)
    {
        if (!this._disposedValue)
        {
            if (disposing)
                this.Timer?.Dispose();

            this._disposedValue = true;
        }
    }
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    #endregion IDisposable
}
