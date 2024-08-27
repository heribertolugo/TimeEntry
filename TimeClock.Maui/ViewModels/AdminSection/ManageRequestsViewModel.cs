using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using TimeClock.Core.Models;
using TimeClock.Core.Models.ApiDtos;
using TimeClock.Core.Models.EntityDtos;
using TimeClock.Maui.Helpers;
using TimeClock.Maui.Services;
using TimeClock.Maui.ViewModels.Shared;

namespace TimeClock.Maui.ViewModels.AdminSection;

class ManageRequestsViewModel : ViewModelBase
{
    private ITimeClockApiAccessService ApiAccessService { get; init; }
    private ILogger<MainViewModel> Logger { get; init; }

    public ManageRequestsViewModel(ITimeClockApiAccessService apiAccessService, ILogger<MainViewModel> logger) : base()
    {
        this.ApiAccessService = apiAccessService;
        this.Logger = logger;
        this.SetPropertyDefaults();
    }

    public ObservableCollection<PunchRequestItem> PunchRequestItems { get; private set; }

    public override bool RefreshOnPropertyChange { get => false; set => base.RefreshOnPropertyChange = false; }

    #region Private Command Actions
    protected override async Task Refresh()
    {
        if (base.IsBusy)
        {
            await UiHelpers.ShowOkAlert(CommonValues.AppBusy, CommonValues.Info);
            return;
        }

        base.IsBusy = true;

        await this.LoadPunchRequests();

        base.IsBusy = false;
        base.IsRefreshing = false;
    }
    private async Task AcceptRequest(PunchRequestItem requestItem)
    {
        await this.SavePunchRequestDecision(requestItem, PunchActionDto.AdminApproved);
    }
    private async Task RejectRequest(PunchRequestItem requestItem)
    {
        await this.SavePunchRequestDecision(requestItem, PunchActionDto.AdminRejected);
    }
    private async Task SavePunchRequestDecision(PunchRequestItem requestItem, PunchActionDto punchAction)
    {
        if (base.IsBusy)
        {
            await UiHelpers.ShowOkAlert(CommonValues.AppBusy, CommonValues.Info);
            return;
        }

        base.IsBusy = true;

        bool isLocationRequired = PermissionsManager.IsLocationRequired();
        Location? geolocation = await PermissionsManager.GetOrRequestLocation(isRequired: isLocationRequired, getCached: !isLocationRequired);

        if (isLocationRequired && geolocation is null)
        {
            await UiHelpers.ShowOkAlert(CommonValues.GeoLocRequired);
            return;
        }

        CreatePunchEntryDto punchEntry = new()
        {
            Id = requestItem.CorrectedEntry.PunchEntryId,
            UserId = requestItem.User.Id,
            DeviceId = Settings.DeviceGuid,
            DateTime = requestItem.CorrectedEntry.DateTime!.Value,
            ActionById = this.UserId,
            Latitude = geolocation?.Latitude,
            Longitude = geolocation?.Longitude,
            PunchAction = punchAction,
            PunchType = PunchTypeDto.None,
            JobTypeId = requestItem.JobType?.JobTypeId,
            JobStepId = requestItem.JobType?.JobStepId,
            //Note = model.Reason,
#warning should enable note for admin in UI
            UserName = this.UserName,
            LocationDivisionCode = Settings.LocationDivision,
            UnionCode = this.Union
        };

        var apiResult = await this.ApiAccessService.UpdatePunchEntry(Settings.DeviceGuid, (await Settings.GetCryptoApiKey()) ?? string.Empty, punchEntry);

        if (!apiResult.IsSuccessfulStatusCode)
        {
            this.Logger.LogError("Could not save punch request for user {userName}. {ResponseCode}", requestItem.User.FullNameOr, apiResult.StatusCode);
            await UiHelpers.ShowOkAlert($"Could not save punch request for user {requestItem.User.FullNameOr}. {apiResult.StatusCode}");
        }
        else
        {
            //await UiHelpers.ShowOkAlert("Successfully subumitted request. Waiting for supervisor approval.", "");
            this.PunchRequestItems.Remove(requestItem);
        }

        base.IsBusy = false;
        base.IsRefreshing = false;
    }
    #endregion Private Command Actions

    #region Private Methods/Helpers
    private async Task LoadPunchRequests()
    {
        this.PunchRequestItems.Clear();
        Func<PunchEntryDto, PunchRequestItem> punchEntryToRequestItem = (p) =>
        {
            List<PunchEntriesHistoryDto> histories = new(p.PunchEntriesHistories.OrderByDescending(h => h.UtcTimeStamp));
            PunchEntriesHistoryDto requestEntry = histories.First();
            histories.Remove(requestEntry);
            PunchEntriesHistoryDto? originalRequest = histories.FirstOrDefault();
            WorkPeriodJobTypeStepDto? jobType = p.WorkPeriod?.WorkPeriodJobTypeSteps.FirstOrDefault();
            return new PunchRequestItem(p.User!, originalRequest, requestEntry, jobType, new object(), p.PunchStatus, p.StablePunchStatus, this.AcceptRequest, this.RejectRequest);
        };
        GetPunchEntriesDto getPunches = new()
        {
            DepartmentId = Settings.DepartmentId,
            DeviceId = Settings.DeviceGuid,
            LocationId = Settings.LocationId,
            RequestedById = this.UserId,
            DateRange = DateRange.Default,
            CurrentStates = new([ PunchActionDto.NewRequest, PunchActionDto.EditRequest ]),
            IncludeStableState = true,
            IncludeUser = true,
            IncludeHistory = true,
            IncludeWorkPeriod = true,
            IncludeWorkPeriodJobType = true
        };
        CancellationToken token = new CancellationTokenSource(CommonValues.ApiCancellationTokenLimit).Token;
        var result = (await this.ApiAccessService.GetPunches(Settings.DeviceGuid, getPunches, token));

            if (!result.IsSuccessfulStatusCode || result.Value is null)
            {
                if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    await UiHelpers.ShowOkAlert("error: Please login");
                }
                else
                {
                    await UiHelpers.ShowOkAlert($"Could not load requests.\n{result.StatusCode}");
                }
                this.Logger.LogError("Unable to get punch requests. apiResult: {resultStatusCode}, data: {getPunches}", result.StatusCode, getPunches.Wrap());
                return;
            }

        foreach (var entry in result.Value!)
        {
            this.PunchRequestItems.Add(punchEntryToRequestItem(entry));
        }
    }
    private void SetPropertyDefaults()
    {
        this.PunchRequestItems = new ObservableCollection<PunchRequestItem>();
    }
    #endregion Private Methods/Helpers
}

internal class PunchRequestItem
{
    public PunchRequestItem(UserDto user, PunchEntriesHistoryDto? originalEntry, PunchEntriesHistoryDto correctedEntry, WorkPeriodJobTypeStepDto? jobType, object payType,
        PunchStatusDto punchStatus, PunchStatusDto? stablePunchStatus, Func<PunchRequestItem,Task> onAccept, Func<PunchRequestItem, Task> onReject)
    {
        this.User = user;
        this.OriginalEntry = originalEntry;
        this.CorrectedEntry = correctedEntry;
        this.JobType = jobType;
        this.PayType = payType;
        this.AcceptCommand = new AsyncRelayCommand(p => this.Accept(onAccept));
        this.RejectCommand = new AsyncRelayCommand(p => this.Reject(onReject));
        this.PunchStatus = punchStatus;
        this.StablePunchStatus = stablePunchStatus;
    }

    public UserDto User { get; set; }
    public PunchEntriesHistoryDto? OriginalEntry { get; set; }
    public PunchEntriesHistoryDto CorrectedEntry { get; set; }
    public WorkPeriodJobTypeStepDto? JobType { get; set; }
    public object PayType { get; set; }
    public PunchStatusDto PunchStatus { get; set; }
    public PunchStatusDto? StablePunchStatus { get; set; }

    private async Task Accept(Func<PunchRequestItem, Task> action)
    {
        await action(this);
    }
    private async Task Reject(Func<PunchRequestItem, Task> action)
    {
        await action(this);
    }

    public IAsyncRelayCommand AcceptCommand { get; set; }
    public IAsyncRelayCommand RejectCommand { get; set; }
}
