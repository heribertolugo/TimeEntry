using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System.Windows.Input;
using TimeClock.Core.Models;
using TimeClock.Core.Models.ApiDtos;
using TimeClock.Core.Models.EntityDtos;
using TimeClock.Maui.Helpers;
using TimeClock.Maui.Services;
using TimeClock.Maui.ViewModels.Shared;

namespace TimeClock.Maui.ViewModels;

internal sealed class SelectEquipmentViewModel : ViewModelBase
{
    #region Private Members/Properties
    private ITimeClockApiAccessService ApiAccessService { get; init; }
    private ILogger<SelectEquipmentViewModel> Logger { get; init; }
    private bool IsSelectionChanged { get; set; }
    private string? _screenTitle = "Select an item to use";
    private EquipmentDto? _selected;
    private EquipmentDto? _initiallySelected;
    private EquipmentDto? _previouslySelected;
    private Guid? _selectedId;
    private int _totalEquipment;
    private string _locationName;
    private string _departmentName;
    private LocationDto? _locationDto;
    private DepartmentDto? _departmentDto;
    private IEnumerable<EquipmentDto> _equipments;
    #endregion Private Members/Properties


#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public SelectEquipmentViewModel()
    {
        this.SetDefaults();
        this.SetCommands();
        this.ApiAccessService = ServiceHelper.GetApiService()!;
        this.Logger = ServiceHelper.GetLoggerService<SelectEquipmentViewModel>()!;
        this.ItemTappedCommand = new Command(this.ItemTapped);
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.


    #region Public Properties

    // TODO: create groups. first group should be recently used. second group all. last group currently being used
    // see: https://www.youtube.com/watch?v=_lVM9gpFSbw
    // or tabs might be a better option
    // or accordions: https://www.youtube.com/watch?v=71_h9wqvapQ&t=3082s
    public IEnumerable<EquipmentDto> Equipments
    {
        get => this._equipments;
        set => base.SetProperty(ref this._equipments, value);
    }
    // see here when setting selected to null: https://stackoverflow.com/a/77247043/6368401
    private bool IsOkToTakeOthersEquipment { get; set; } = false;
    private EquipmentDto? InternalPreselected { get; set; }
    public EquipmentDto? Selected
    {
        get => this._selected;
        set
        {
            this.InternalPreselected = value;
            if (this.IsEquipmentAlreadyTaken(value) && !this.IsOkToTakeOthersEquipment)
                return;
            this.IsSelectionChanged = true;
            this.PreviouslySelected = this.Selected;
            base.SetProperty(ref this._selected, value);
            this.SelectedId = value?.Id;
            this.ScreenTitle = string.IsNullOrWhiteSpace(value?.Sku) ? (this.InitiallySelected is not null ? $"{this.InitiallySelected.Sku} Unselected" : null) : $"{value.Sku} selected";
            this.IsOkToTakeOthersEquipment = false;
        }
    }
    private bool IsEquipmentAlreadyTaken(EquipmentDto? equipment)
    {
        var owningUser = equipment?.EquipmentsToUsers.FirstOrDefault()?.User;
        return owningUser != null && owningUser.Id != this.UserId;
    }
    private async Task<bool> ConfirmTakeOthersEquipment(EquipmentDto? equipment)
    {
        string? username = equipment.EquipmentsToUsers.FirstOrDefault()?.User?.FullNameOr ?? "Someone else";
        return await UiHelpers.ShowYesNoAlert($"{username} is currently using this.\nAre you sure you want to take it?", "Please confirm");
    }
    public EquipmentDto? InitiallySelected
    {
        get => this._initiallySelected;
        set => base.SetProperty(ref this._initiallySelected, value);
    }
    public EquipmentDto? PreviouslySelected
    {
        get => this._previouslySelected;
        set => base.SetProperty(ref this._previouslySelected, value);
    }
    public Guid? SelectedId
    {
        get => this._selectedId;
        set => base.SetProperty(ref this._selectedId, value);
    }
    public int TotalEquipment
    {
        get => this._totalEquipment;
        private set => base.SetProperty(ref this._totalEquipment, value);
    }
    public string LocationName
    {
        get => this._locationName;
        private set => base.SetProperty(ref this._locationName, value);
    }
    public string DepartmentName
    {
        get => this._departmentName;
        private set => base.SetProperty(ref this._departmentName, value);
    }
    public string? ScreenTitle
    {
        get => this._screenTitle;
        private set => base.SetProperty(ref this._screenTitle, value ?? "Select an item to use");
    }
    #endregion Public Proeprties


    #region Public Commands
    public IAsyncRelayCommand AcceptCommand { get; private set; }
    public IAsyncRelayCommand DelayLoadMoreCommand { get; private set; }
    public ICommand ItemTappedCommand { get; private set; }
    #endregion Public Commands


    #region Private Command Actions
    private async Task Accept()
    {
        if (base.IsBusy)
        {
            await UiHelpers.ShowOkAlert(CommonValues.AppBusy, CommonValues.Info);
            return;
        }

        if (this.Selected is null && this.InitiallySelected is null)
        {
            // should we actually have this confirmation? wouldn't it just make more sense to just close the page? TODO:
            await UiHelpers.ShowOkAlert(CommonValues.EquipNotSelect);
            return;
        }
        bool? isPunchingOut = this.Selected is null ? await this.ConfirmIsPunchingOut() : false;

        if (!isPunchingOut.HasValue)
            return;

        if (this.Selected?.Id == this.InitiallySelected?.Id)
        {
            await base.CloseCommand.ExecuteAsync(null);
        }

        Guid deviceId = Settings.DeviceGuid;
        ResultValues<EquipmentsToUserDto?>? unlinkedResult = null;
        ResultValues<EquipmentsToUserDto?> linkedResult;
        // check if current user had something previously selected. if not, check if current user took a selection from someone else
        EquipmentsToUserDto? priorLink = this.InitiallySelected?.EquipmentsToUsers.FirstOrDefault() ?? this.Selected?.EquipmentsToUsers.FirstOrDefault();

        bool isLocationRequired = PermissionsManager.IsLocationRequired();
        Location? geolocation = await PermissionsManager.GetOrRequestLocation(isRequired: isLocationRequired, getCached: !isLocationRequired);

        if (isLocationRequired && geolocation is null)
        {
            await UiHelpers.ShowOkAlert(CommonValues.GeoLocRequired);
            return;
        }

        SlimPunchEntry punchEntry = new()
        {
            DateTime = DateTime.Now,
            PunchAction = PunchActionDto.SelfEquipmentSelect,
            JobTypeId = this.UserJobTypeId,
            JobStepId = this.UserJobStepId,
            Latitude = geolocation?.Latitude,
            Longitude = geolocation?.Longitude,
            PunchType = (PunchTypeDto)base.PunchType,
            UserId = this.UserId
        };

        if (priorLink is not null)
        {
            unlinkedResult = await this.ApiAccessService.UnlinkUserToEquipment(deviceId, await Settings.GetCryptoApiKey() ?? string.Empty, 
                new(deviceId, priorLink.Id, base.UserId, priorLink.EquipmentId, 
                base.UserId, isPunchingOut.Value, punchEntry, this.UserJobTypeId, this.UserJobStepId));
        }

        if (this.Selected is not null)
        {
            if (unlinkedResult is not null)
            {
                if (!unlinkedResult.IsSuccessfulStatusCode)
                {
                    await UiHelpers.ShowOkAlert("There was an error saving deselected equipment.\nTry again or contact IT.");
                    this.Logger.LogError("Could not save equipment to user unlink. Result: {resultStatusCode}", unlinkedResult.StatusCode);
                    await base.CloseCommand.ExecuteAsync(null);
                    return;
                }
            }

            JobTypeStepToEquipmentDto? jobTypeStep = this.Selected.JobTypeStepToEquipment.FirstOrDefault(t => t.UnionCode == "");
            Guid? jobType = jobTypeStep is null ? base.UserJobTypeId : jobTypeStep.JobTypeId;
            Guid? jobStep = jobTypeStep is null ? base.UserJobStepId : jobTypeStep.JobStepId;

            linkedResult = await this.ApiAccessService.LinkUserToEquipment(deviceId, await Settings.GetCryptoApiKey() ?? string.Empty, 
                new(deviceId, base.UserId, this.Selected.Id, base.UserId, jobType, jobStep, punchEntry));

            if (!linkedResult.IsSuccessfulStatusCode)
            {
                await UiHelpers.ShowOkAlert("There was an error saving selected equipment.\nTry again or contact IT."); 
                this.Logger.LogError("Could not save equipment to user link. Result: {resultStatusCode}", linkedResult.StatusCode);
            }
        }
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

        if (this._locationDto is null || this._departmentDto is null)
            await this.LoadLocationDepartment();
        // temp variable so we can set selected item, allowing the UI to pick up the selected item on equipments loading.
        // this way we can set the color of the currently selected equipment
        IEnumerable<EquipmentDto> equipments = await this.GetEquipments();
        List<EquipmentDto> equipmentsSortedByCurrentUserFirst = new List<EquipmentDto>(
            equipments.Where(q => q.EquipmentsToUsers.FirstOrDefault()?.UserId ==  this.UserId)
            .OrderByDescending(q => q.EquipmentsToUsers.FirstOrDefault()?.UnLinkedOn ?? q.EquipmentsToUsers.FirstOrDefault()?.LinkedOn));
        equipmentsSortedByCurrentUserFirst.AddRange(equipments.Where(q => q.EquipmentsToUsers.FirstOrDefault()?.UserId != this.UserId));
        // set selected before loading equipments to UI. This allows the converter to pickup selected on first run
        this.InitiallySelected = this.Selected = equipments.FirstOrDefault(q => q.EquipmentsToUsers.Any(t => t.UserId == this.UserId));
        this.Equipments = equipmentsSortedByCurrentUserFirst;
        this.TotalEquipment = await this.GetEquipmentsCount();
        base.IsBusy = false;
        base.IsRefreshing = false;
        this.IsSelectionChanged = false; // reset since we assigned this.Selected
    }

    private Task DelayLoadMore() => Task.CompletedTask;

    /// <summary>
    /// This keeps track of the selected item so that the user can deselect it upon click. 
    /// Deselect functionality has not been implemented in this control as of yet.
    /// </summary>
    /// <param name="arg"></param>
    /// <exception cref="ArgumentNullException"></exception>
    private async void ItemTapped(object arg)
    {
        if (this.IsEquipmentAlreadyTaken(this.InternalPreselected))
        {
            this.IsOkToTakeOthersEquipment = await UiHelpers.ShowYesNoAlert($"Someone is currently using this.\nAre you sure you want to take it?", "Please confirm");
            
            if (this.IsOkToTakeOthersEquipment)
            {
                this.Selected = this.InternalPreselected;
            }

            return;
        }

        if (!this.IsSelectionChanged)
            this.Selected = null;

        this.IsSelectionChanged = false;
    }
    #endregion Private Command Actions


    #region Private Methods/Helpers
    private Task<bool?> ConfirmIsPunchingOut()
    {
        return UiHelpers.ShowYesNoCancel("Are you punching out?", cancelText: "cancel");
        //return UiHelpers.ShowYesNoAlert(string.Empty, "Are you punching out?");
    }
    private async Task<IEnumerable<EquipmentDto>> GetEquipments() 
    {
        GetEquipmentsDto data = new()
        {
            DepartmentId = Settings.DepartmentId,
            LocationId = Settings.LocationId,
            IsActive = true,
            IncludeCurrentEquipmentToUser = true,
            IncludeEquipmentToUserUser = true,
            IncludeEquipmentJobTypeSteps = true
        };
        data.Sorting.Add(new SortOption<GetEquipmentsDto.SortField>(GetEquipmentsDto.SortField.LastUsed, Core.Models.SortOrderDto.Descending));
        data.Sorting.Add(new SortOption<GetEquipmentsDto.SortField>(GetEquipmentsDto.SortField.Name));
        //data.Sorting.Add(new SortOption<GetEquipmentsDto.SortField>(GetEquipmentsDto.SortField.User));

        ResultValues<IEnumerable<EquipmentDto>?> result = await this.ApiAccessService.GetEquipment(Settings.DeviceGuid, data);

        if (result.IsSuccessfulStatusCode)
            return result.Value ?? Enumerable.Empty<EquipmentDto>();

        await UiHelpers.ShowOkAlert("There was an error getting equipment.\nTry again or contact IT.");
        this.Logger.LogError("Could not get equipment. Result: {resultStatusCode}", result.StatusCode);
        return Enumerable.Empty<EquipmentDto>();
    }
    private async Task<int> GetEquipmentsCount()
    {
        //Guid departmentId = this._departmentDto?.Id ?? Guid.Empty;
        //Guid locationId = this._locationDto?.Id ?? Guid.Empty;
        GetEquipmentsDto data = new()
        {
            DepartmentId = Settings.DepartmentId,
            LocationId = Settings.LocationId,
            IsActive = true
        };

        ResultValues<int> result = await this.ApiAccessService.GetEquipmentCount(Settings.DeviceGuid, data);

        if (!result.IsSuccessfulStatusCode)
        {
            await UiHelpers.ShowOkAlert("There was an error getting equipment count.\nTry again or contact IT.");
            this.Logger.LogError("Could not get equipment count. Result: {resultStatusCode}", result.StatusCode);
            return 0;
        }

        return result.Value;
    }
    private void SetDefaults()
    {
        this.Equipments = Enumerable.Empty<EquipmentDto>();
        this.TotalEquipment = 0;
        this.LocationName = string.Empty; 
        this.DepartmentName = string.Empty;
    }
    private void SetCommands()
    {
        this.AcceptCommand = new AsyncRelayCommand(this.Accept);
        base.RefreshCommand = new AsyncRelayCommand(this.Refresh);
        this.DelayLoadMoreCommand = new AsyncRelayCommand(this.DelayLoadMore);
    }
    private async Task LoadLocationDepartment()
    {
        Guid locationId = Settings.LocationId;
        //Guid departmentId = Settings.DepartmentId;

        this._locationDto = (await this.ApiAccessService.GetLocation(Settings.DeviceGuid, locationId)).Value;
        //** we don't need department since it would be the same as location. so don't load it, avoid unneeded db/api calls.
        //this._departmentDto = await this.ApiAccessService.GetDepartment(departmentId);

        this.LocationName = this._locationDto?.Name ?? string.Empty;
        //this.DepartmentName = this._departmentDto?.Name ?? string.Empty;
    }
    #endregion Private Methods/Helpers
}
