using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using TimeClock.Core.Models.ApiDtos;
using TimeClock.Core.Models.EntityDtos;
using TimeClock.Maui.Helpers;
using TimeClock.Maui.Services;
using TimeClock.Maui.ViewModels.Shared;

namespace TimeClock.Maui.ViewModels.MainSection;
[QueryProperty(nameof(SelectJobTypeStepViewModel.Barcode), nameof(SelectJobTypeStepViewModel.Barcode))]
[QueryProperty(nameof(SelectJobTypeStepViewModel.Password), nameof(SelectJobTypeStepViewModel.Password))]
internal sealed class SelectJobTypeStepViewModel : ViewModelBase
{
    #region Private Members/Properties
    private ITimeClockApiAccessService ApiAccessService { get; init; }
    private ILogger<SelectEquipmentViewModel> Logger { get; init; }
    private SelectJobTypeStepViewModelItem? _selectedItem;
    private ObservableCollection<SelectJobTypeStepViewModelItem> _items;

    #endregion Private Members/Properties

    public SelectJobTypeStepViewModel()
    {
        this.ApiAccessService = ServiceHelper.GetApiService()!;
        this.Logger = ServiceHelper.GetLoggerService<SelectEquipmentViewModel>()!;
        this.AcceptCommand = new AsyncRelayCommand(this.Accept);
        this.DelayLoadMoreCommand = new AsyncRelayCommand(this.DelayLoadMore);
        this.Items = new ObservableCollection<SelectJobTypeStepViewModelItem>();
    }

    #region Public Properties
    public SelectJobTypeStepViewModelItem? SelectedItem
    {
        get => this._selectedItem;
        set => base.SetProperty(ref this._selectedItem, value);
    }
    public ObservableCollection<SelectJobTypeStepViewModelItem> Items
    {
        get => this._items;
        set => base.SetProperty(ref this._items, value);
    }
    public string? Barcode { get; set; }
    public string? Password { get; set; }
    #endregion Public Properties


    #region Public Commands
    public IAsyncRelayCommand AcceptCommand { get; private set; }
    public IAsyncRelayCommand DelayLoadMoreCommand { get; private set; }
    #endregion Public Commands


    #region Private Commands
    private Task DelayLoadMore()
    {
        return Task.CompletedTask;
    }
    private async Task Accept()
    {
        if (base.IsBusy)
        {
            await UiHelpers.ShowToast(CommonValues.AppBusy);
            return;
        }

        base.IsBusy = true;

        if (this.SelectedItem is null)
        {
            await UiHelpers.ShowOkAlert($"Cannot save.\nNo selection made!");
            base.IsBusy = false;
            base.IsRefreshing = false;
            await base.CloseCommand.ExecuteAsync(null);
            return;
        }

        bool isLocationRequired = PermissionsManager.IsLocationRequired();
        Location? geolocation = await PermissionsManager.GetOrRequestLocation(isRequired: isLocationRequired, getCached: !isLocationRequired);

        if (isLocationRequired && geolocation is null)
        {
            await UiHelpers.ShowOkAlert(CommonValues.GeoLocRequired);
            base.IsBusy = false;
            base.IsRefreshing = false;
            return;
        }

        CreatePunchEntryDto punchEntry = new()
        {
            ActionById = this.UserId,
            PunchAction = PunchActionDto.Self,
            DateTime = DateTime.Now,
            DeviceId = Settings.DeviceGuid,
            Id = Guid.NewGuid(),
            Latitude = geolocation?.Latitude ?? Settings.Latitude,
            Longitude = geolocation?.Longitude ?? Settings.Longitude,
            Password = this.Password ?? string.Empty,
            UserName = this.Barcode ?? this.UserName,
            PunchType = (PunchTypeDto)this.PunchType,
            IncludeUser = false,
            IsJobTypeStepSet = true,
            JobStepId = this.SelectedItem.JobStepId,
            JobTypeId = this.SelectedItem.JobTypeId,
            UserId = this.UserId,
            LocationDivisionCode = Settings.LocationDivision,
            UnionCode = this.Union
        };

        ResultValues<PunchEntryDto?> result = await this.ApiAccessService.CreatePunchEntry(punchEntry.DeviceId, (await Settings.GetCryptoApiKey()) ?? string.Empty, punchEntry);

        if (!result.IsSuccessfulStatusCode)
        {
            await UiHelpers.ShowOkAlert($"Selection could not be saved.\nContact IT. {result.StatusCode}");
            this.Logger.Log(LogLevel.Error, result.StatusCode.ToString());
        }

        base.IsBusy = false;
        base.IsRefreshing = false;
        await base.CloseCommand.ExecuteAsync(null);
    }
    protected override async Task Refresh()
    {
        if (base.IsBusy)
        {
            await UiHelpers.ShowToast(CommonValues.AppBusy);
            return;
        }

        base.IsBusy = true;

        await this.LoadJobCodeSteps();

        base.IsBusy = false;
        base.IsRefreshing = false;
    }
    #endregion Private Commands

    private async Task LoadJobCodeSteps()
    {
        Guid deviceId = Settings.DeviceGuid;
        GetUserJobTypeStepsDto dto = new()
        {
            UserId = this.UserId,
            DeviceId = deviceId
        };
        var bobo = await this.ApiAccessService.GetUserJobTypeSteps(deviceId, await Settings.GetCryptoApiKey() ?? string.Empty, dto);

        foreach(var boob in bobo.Value)
        {
            KeyValuePair<string, string> iconData;
            CommonValues.JobTypeToFaIconMap.TryGetValue(boob.JobType.JdeId, out iconData);

            this.Items.Add(new SelectJobTypeStepViewModelItem()
            {
                Id = boob.Id,
                IconName = iconData.Value,
                IconNameSpace = iconData.Key,
                JobStepCode = boob.JobStep?.JdeId?.Trim(),
                JobStepDescription = boob.JobStep?.Description?.Trim(),
                JobStepId = boob.JobStep.Id,
                JobTypeCode = boob.JobType?.JdeId?.Trim(),
                JobTypeDescription = boob.JobType?.Description?.Trim(),
                JobTypeId = boob.JobType.Id
            });
        }
    }
}

internal sealed class SelectJobTypeStepViewModelItem
{
    public Guid Id { get; set; }
    public Guid JobTypeId { get; set; }
    public string JobTypeCode { get; set; }
    public string JobTypeDescription { get; set; }
    public Guid JobStepId { get; set; }
    public string JobStepCode { get; set; }
    public string JobStepDescription { get; set; }
    public string IconNameSpace { get; set; }
    public string IconName { get; set; }
}