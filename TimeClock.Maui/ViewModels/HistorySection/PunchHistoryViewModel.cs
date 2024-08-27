using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using TimeClock.Core;
using TimeClock.Core.Models;
using TimeClock.Core.Models.ApiDtos;
using TimeClock.Core.Models.EntityDtos;
using TimeClock.Maui.Helpers;
using TimeClock.Maui.Services;
using TimeClock.Maui.ViewModels.Shared;

namespace TimeClock.Maui.ViewModels.HistorySection;

internal sealed class PunchHistoryViewModel : ViewModelBase
{
    private TimeSpan _totalForPunches;
    private ObservableCollection<PunchEntryDto> _punchEntries;
    private bool _isEditable;
    private IPopupService PopupService { get; init; }
    private ITimeClockApiAccessService ApiAccessService { get; init; }
    private ILogger<PunchHistoryViewModel> Logger { get; init; }


#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public PunchHistoryViewModel(IPopupService popupService, ILogger<PunchHistoryViewModel> logger, ITimeClockApiAccessService apiAccessService)
    {
        this.PopupService = popupService;
        this.ApiAccessService = apiAccessService;
        this.Logger = logger;
        this.SetDefaults();
        this.SetCommands();
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.


    #region Public Properties
    public ObservableCollection<PunchEntryDto> PunchEntries
    {
        get => this._punchEntries;
        set => base.SetProperty(ref this._punchEntries, value);
    }
    public TimeSpan TotalForPunches
    {
        get => this._totalForPunches;
        set => base.SetProperty(ref this._totalForPunches, value);
    }
    public bool IsEditable
    {
        get => this._isEditable;
        set => base.SetProperty(ref this._isEditable, value);
    }
    public override DateTime FilterDate 
    { 
        get => base.FilterDate;
        set
        {
            base.FilterDate = value;
            this.IsEditable = this.FilterDate.IsBeforeCutoff();
        }
    }
    #endregion Public Properties


    #region Public Commands
    public IAsyncRelayCommand DelayLoadMoreCommand { get; set; }
    public IAsyncRelayCommand AddPunchCommand { get; set; }
    public IAsyncRelayCommand EditPunchCommand { get; set; }
    #endregion Public Commands


    #region Private Command Actions
    protected override async Task Refresh()
    {
        await this.GetSetPunchEntriesAsync();
    }
    private async Task DelayLoadMore()
    {
        System.Diagnostics.Debug.WriteLine("DelayLoadMore()");
    }
    private async Task AddPunch()
    {
        object? result = await this.PopupService.ShowPopupAsync<EditPunchViewModel>(m =>
        {
            m.NewPunchTime = DateTime.Now.TimeOfDay;
            m.UserId = this.UserId;
        });
        EditPunchViewModel? model = result as EditPunchViewModel;
        CreatePunchEntryDto punchEntry;
        if (result is null)
            return;
        if (model == null)
            throw new ArgumentNullException(CommonValues.PunchEntryNull);

        bool isLocationRequired = PermissionsManager.IsLocationRequired();
        Location? geolocation = await PermissionsManager.GetOrRequestLocation(isRequired: isLocationRequired, getCached: !isLocationRequired);

        if (isLocationRequired && geolocation is null)
        {
            await UiHelpers.ShowOkAlert(CommonValues.GeoLocRequired);
            return;
        }

        punchEntry = new()
        {
            Id = Guid.NewGuid(),
            UserId = model.UserId,
            DeviceId = Settings.DeviceGuid,
            DateTime = this.FilterDate.AtTimeOfDay(model.NewPunchTime.Hours, model.NewPunchTime.Minutes),
            ActionById = this.UserId,
            Latitude = geolocation?.Latitude,
            Longitude = geolocation?.Longitude,
            PunchAction = PunchActionDto.NewRequest,
            PunchType = (PunchTypeDto)base.PunchType,
            JobTypeId = this.UserJobTypeId,
            JobStepId = this.UserJobStepId,
            Note = model.Reason,
            UserName = this.UserName,
            LocationDivisionCode = Settings.LocationDivision,
            UnionCode = this.Union, 
            WorkPeriodId = this.PunchEntries.FirstOrDefault()?.WorkPeriodId
        };

        var apiResult = await this.ApiAccessService.CreatePunchEntry(Settings.DeviceGuid, (await Settings.GetCryptoApiKey()) ?? string.Empty, punchEntry);

        if (!apiResult.IsSuccessfulStatusCode)
        {
            this.Logger.LogError("Could not create user missed punch request. {ResponseCode}", apiResult.StatusCode);
            await UiHelpers.ShowOkAlert($"Could not create request. {apiResult.StatusCode}");
        }
        else
        {
            await UiHelpers.ShowOkAlert("Successfully submitted request. Waiting for supervisor approval.", "");
        }

        // don't think we need to refresh. a request for a missed punch will not show in punch history, we can enable that as it might be useful
        //await this.Refresh();
    }
    private async Task EditPunch(Guid punchEntryId)
    {
        CreatePunchEntryDto punchEntry;
        // grab the entry from our list rather than using the one provided
        // we do this to verify what was passed is indeed valid, before we send to DB
        PunchEntryDto? punch = this.PunchEntries.FirstOrDefault(p => p.Id == punchEntryId);
        if (punch == null) throw new NullReferenceException(CommonValues.PunchNotFndInvalState);
        object? result = await this.PopupService.ShowPopupAsync<EditPunchViewModel>(m => {
            m.NewPunchTime = punch.DateTime.TimeOfDay;
            m.PunchId = punch.Id;
            m.PunchTime = punch.DateTime.TimeOfDay;
            m.UserId = punch.UserId;
            });
        EditPunchViewModel? model = result as EditPunchViewModel;
        if (result == null)
            return;
        if (model == null)
            throw new ArgumentNullException(CommonValues.PunchEntryNull);

        bool isLocationRequired = PermissionsManager.IsLocationRequired();
        Location? geolocation = await PermissionsManager.GetOrRequestLocation(isRequired: isLocationRequired, getCached: !isLocationRequired);

        if (isLocationRequired && geolocation is null)
        {
            await UiHelpers.ShowOkAlert(CommonValues.GeoLocRequired);
            return;
        }

        punchEntry = new()
        {
            Id = punch.Id,
            UserId = model.UserId,
            DeviceId = Settings.DeviceGuid,
            DateTime = this.FilterDate.AtTimeOfDay(model.NewPunchTime.Hours, model.NewPunchTime.Minutes),
            ActionById = this.UserId,
            Latitude = geolocation?.Latitude,
            Longitude = geolocation?.Longitude,
            PunchAction = PunchActionDto.EditRequest,
            PunchType = (PunchTypeDto)base.PunchType,
            JobTypeId = this.UserJobTypeId,
            JobStepId = this.UserJobStepId,
            Note = model.Reason,
            UserName = this.UserName,
            LocationDivisionCode = Settings.LocationDivision,
            UnionCode = this.Union
        };

        var apiResult = await this.ApiAccessService.UpdatePunchEntry(Settings.DeviceGuid, (await Settings.GetCryptoApiKey()) ?? string.Empty, punchEntry);

        if (!apiResult.IsSuccessfulStatusCode)
        {
            this.Logger.LogError("Could not create user missed punch request. {ResponseCode}", apiResult.StatusCode);
            await UiHelpers.ShowOkAlert($"Could not create request. {apiResult.StatusCode}");
        }
        else
        {
            await UiHelpers.ShowOkAlert("Successfully submitted request. Waiting for supervisor approval.", "");
        }

        // don't think we need to refresh. a request for a missed punch will not show in punch history, we can enable that as it might be useful
        //await this.Refresh();
    }
    #endregion Private Command Actions


    #region Private Method/Helpers
    private void SetDefaults()
    {
        this.PunchEntries = new();// Enumerable.Empty<PunchEntryDto>();
        this.TotalForPunches = new TimeSpan();
        this._filterDate = DateTime.Now;
    }
    private async Task GetSetPunchEntriesAsync()
    {
        if (base.IsBusy)
        {
            await UiHelpers.ShowToast(CommonValues.AppBusy);
            return;
        }
        base.IsBusy = true;

        try
        {
            var getPunchEntries = await this.GetPunchEntriesAsync(this.UserId, this.FilterDate, this.FilterDate);
            await this.SetPunchEntriesAsync(getPunchEntries);
        }
        catch(Exception ex)
        {
            this.Logger.LogError(ex, "GetSetPunchEntriesAsync had exception");
            await UiHelpers.ShowOkAlert("Failed to get or set punch entries. This was an exceptional case. Contact IT.");
        }
        finally
        {
            this.IsBusy = false;
            base.IsRefreshing = false;
        }
    }
    private async Task<IEnumerable<PunchEntryDto>> GetPunchEntriesAsync(Guid userId, DateTime from, DateTime to)
    {
        GetPunchEntriesDto data = new()
        {
            //DepartmentId = Settings.DepartmentId,
            //LocationId = Settings.LocationId,
            DeviceId = Settings.DeviceGuid,
            DateRange = new DateRange(from.StartOfDay(),to.EndOfDay()),
            RequestedById = this.UserId,
            IncludeDepartment = true,
            IncludeLocation = true,
            UserId = userId,
            GetIfStableState = true
            //CurrentStates = new(PunchActionDtoEx.PositiveStates())
        };
        data.Sorting.Add(new SortOption<GetPunchEntriesDto.SortField>(GetPunchEntriesDto.SortField.DateTime, SortOrderDto.Ascending));

        var punches = await this.ApiAccessService.GetPunches(Settings.DeviceGuid, data);

        if (!punches.IsSuccessfulStatusCode || punches.Value is null)
        {
            await UiHelpers.ShowOkAlert($"Could not get from server. {punches.StatusCode}");
            this.Logger.LogError("Could not get punch history from server. response: {responseCode}", punches.StatusCode);
            return Enumerable.Empty<PunchEntryDto>();
        }

        return punches.Value;
    }
    private ValueTask SetPunchEntriesAsync(IEnumerable<PunchEntryDto> punchEntries)
    {
        this.PunchEntries.Clear();
        foreach (var punch in punchEntries)
            this.PunchEntries.Add(punch);
        this.TotalForPunches = this.PunchEntries.TotalStablePunchTime();
        return ValueTask.CompletedTask;
    }
    private void SetCommands()
    {
        this.RefreshCommand = new AsyncRelayCommand(this.Refresh);
        this.DelayLoadMoreCommand = new AsyncRelayCommand(this.DelayLoadMore);
        this.AddPunchCommand = new AsyncRelayCommand(this.AddPunch);
        this.EditPunchCommand = new AsyncRelayCommand<Guid>(c => this.EditPunch(c));
    }
    #endregion Private Method/Helpers
}
