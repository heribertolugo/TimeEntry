using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using TimeClock.Core;
using TimeClock.Core.Models;
using TimeClock.Core.Models.ApiDtos;
using TimeClock.Core.Models.EntityDtos;
using TimeClock.Maui.Helpers;
using TimeClock.Maui.Models;
using TimeClock.Maui.Services;
using TimeClock.Maui.ViewModels.Shared;

namespace TimeClock.Maui.ViewModels.AdminSection;

internal class ManagePunchesViewModel : ViewModelBase
{
    #region Private Members
    private ObservableCollection<EquipmentDto> _equipments;
    private ObservableCollection<UserDto> _users;
    private ObservableCollection<UserActionsForDay> _actionsForDays;
    private ObservableCollection<UserDto> _outsideUsers;
    private UserDto? _selectedUser;
    private string _userInstructions;
    private ILogger<ManagePunchesViewModel> Logger { get; init; }
    private ITimeClockApiAccessService ApiAccessService { get; init; }
    #endregion Private Members


    public ManagePunchesViewModel(ITimeClockApiAccessService apiAccessService) 
    {
        ILogger<ManagePunchesViewModel>? logger = ServiceHelper.GetLoggerService<ManagePunchesViewModel>();
        if (logger is null)
            throw new NullReferenceException($"{nameof(logger)} was not found in services");
        this.Logger = logger;
        this.ApiAccessService = apiAccessService;
        this._actionsForDays = new ObservableCollection<UserActionsForDay>();
        this._users = new ObservableCollection<UserDto>();
        this._equipments = new ObservableCollection<EquipmentDto>();
        this._outsideUsers = new ObservableCollection<UserDto>();
        this._selectedUser = null;
        this.AddUserCommand = new AsyncRelayCommand(this.AddUser);
        this._filterDate = DateTime.Now;
        this._userInstructions = string.Empty;
    }

    #region public properties
    public UserDto? SelectedUser
    {
        get => this._selectedUser;
        set
        {
            base.SetProperty(ref this._selectedUser, value);
            this.RefreshCommand.ExecuteAsync(this);
            this.UserInstructions = CommonValues.ClickTimeEntryEdit;
        }
    }
    public ObservableCollection<UserDto> Users => this._users;
    public ObservableCollection<UserDto> OutsideUsers => this._outsideUsers;
    public ObservableCollection<UserActionsForDay> ActionsForDays => this._actionsForDays;
    public string UserInstructions { get => this._userInstructions; private set => base.SetProperty(ref this._userInstructions, value); }

    #endregion public properties


    #region Public Commands
#warning SaveCommand not implemented
    public IAsyncRelayCommand SaveCommand { get; private set; }
    public IAsyncRelayCommand AddUserCommand { get; private set; }
    #endregion Public Commands


    #region Private Commands
    private Task AddUser()
    {
        Debug.WriteLine("adding user");
#warning AddUser not implemented
        return Task.CompletedTask;
    }
    #endregion Private Commands


    #region Private Methods

    private async ValueTask LoadEquipmentsCache(bool forceRefresh = false)
    {
        if (this._equipments.Count < 1 || forceRefresh)
        {
            GetEquipmentsDto data = new()
            {
                LocationId = Settings.LocationId,
                DepartmentId = Settings.DepartmentId,
            };
            var result = await this.ApiAccessService.GetEquipment(Settings.DeviceGuid, data);

            if (result.Value is null || !result.IsSuccessfulStatusCode)
            {
                throw new Exception();
            }

            IEnumerable<EquipmentDto> equipments = result.Value;
            this._equipments.Clear();

            foreach (EquipmentDto equip in equipments)
                this._equipments.Add(equip);
        }
    }

    private async ValueTask LoadUsers(bool forceRefresh = false)
    {
        if (this._users.Count < 1 || forceRefresh)
        {
            GetUsersDto data = new()
            {
                DepartmentId = Settings.DepartmentId,
                LocationId = Settings.LocationId,
                DeviceId = Settings.DeviceGuid,
                IsActive = true,
                IncludeEquipmentToUser = false,
                IncludeClaims = true,
                IncludeJobType = true,
                IncludeJobStep = true,
                IncludeJobTypeSteps = true,
                UserActiveState = true,
                Paging = new PagingDto(1, 100)
            };

            ResultValues<IEnumerable<UserDto>?> result = await this.ApiAccessService.GetUsers(data.DeviceId, data);
            IEnumerable<UserDto> users;

            if (!result.IsSuccessfulStatusCode || result.Value is null)
            {
                this.Logger.LogWarning("Unable to load users");
                await UiHelpers.ShowOkAlert("Unable to load users. Check internet or contact IT");
                return;
            }

            users = result.Value;

            this._users.Clear();

            foreach (UserDto user in users
                .Where(u => !u.UserClaims
                // filter out other users who can edit punches. so other managers. unless it is the one who is currently logged in
                    .Any(c => c.AuthorizationClaim?.Type.Equals(Enum.GetName(AuthorizationClaimType.CanEditOthersPunches), StringComparison.InvariantCultureIgnoreCase) ?? false) || u.Id == this.UserId)
                .OrderBy(u => u.FullNameOr)) 
            {
                this._users.Add(user);
            }
        }
    }

    private async Task LoadUserActivity()
    {
        if (this.SelectedUser is null)
            return;
        DateTime beginningOfWeek = this.FilterDate.BeginningOfWeek().StartOfDay();
        DateTime endOfWeek = this.FilterDate.EndOfWeek().EndOfDay();

        var data = new GetWorkPeriodsDto()
        {
            DateRange = new DateRange(beginningOfWeek, endOfWeek),
            DeviceId = Settings.DeviceGuid,
            IncludePunchEntries = true,
            IncludeUser = true,
            RequestedById = this.UserId,
            UserId = this.SelectedUser.Id,
            IncludeEquipment = true,
            PunchActions = new HashSet<PunchActionDto>(PunchActionDtoEx.PositiveStates())
        };
        var result = await this.ApiAccessService.GetWorkPeriods(Settings.DeviceGuid, data, (await Settings.GetCryptoApiKey()) ?? string.Empty);

        if (!result.IsSuccessfulStatusCode || result.Value is null)
        {
            this.Logger.LogWarning("Unable to load user activity");
            await UiHelpers.ShowOkAlert("Unable to load user activity. Check internet or database config.");
            return;
        }

        UserDto user = this.SelectedUser;
        UserDto editor = new()
        {
            Id = this.UserId,
            FirstName = this.UserFirstName,
            LastName = this.UserLastName,
            EmployeeNumber = this.UserEmployeeNumber,
            UserName = this.UserName,
        };

        Dictionary<DateTime, List<WorkPeriodDto>> workPeriodsByDate = result.Value.GroupBy(w => w.WorkDate.Date).ToDictionary(g => g.Key, g => g.ToList());

        this.ActionsForDays.Clear();
        foreach (DateTime date in beginningOfWeek.UntilDate(endOfWeek))
        {
            List<WorkPeriodDto> workPeriods = workPeriodsByDate.TryGetValue(date.Date, out List<WorkPeriodDto>? value) ? value : [];
            UserActionsForDay userActionsInDay = new(user, editor, date, workPeriods, this._equipments);

            foreach(var item in workPeriods.SelectMany(w => w.EquipmentsToUsers.Select(q => new { w, q })))
            {
                item.w.User!.DefaultJobStep = user.DefaultJobStep;
                item.w.User!.DefaultJobType = user.DefaultJobType;
                item.w.User!.JobTypeSteps = user.JobTypeSteps;

                userActionsInDay.Items.Add(
                    new UserActionsForDayItem(item.w.User!, item.q.LinkedBy!, item.q, this._equipments, item.w, userActionsInDay, this.Logger)
                    );
                if (item.q.UnLinkedOn.HasValue)
                    userActionsInDay.Items.Add(
                        new UserActionsForDayItem(item.w.User!, item.q.UnlinkedBy!, item.q, this._equipments, item.w, userActionsInDay, this.Logger, true)
                    );
            }
            foreach(var item in workPeriods.SelectMany(w => w.PunchEntries.Select(p => new { w, p })))
            {
                item.w.User!.DefaultJobStep = user.DefaultJobStep;
                item.w.User!.DefaultJobType = user.DefaultJobType;
                item.w.User!.JobTypeSteps = user.JobTypeSteps;

                userActionsInDay.Items.Add(
                    new UserActionsForDayItem(item.w.User!, item.p.ActionBy ?? new(), item.p, item.w, userActionsInDay, this.Logger)
                    );
            }

            this._actionsForDays.Add(userActionsInDay);
        }
    }

    private async Task<IEnumerable<UserDto>> GetOutsideUsers(bool forceRefresh = false)
    {
#warning JDE To Be Implemented
        return null;
    }
    #endregion Private Methods


    #region Overrides
    protected override async Task Refresh()
    {
        if (base.IsBusy)
        {
            await UiHelpers.ShowToast(CommonValues.AppBusy);
            return;
        }

        base.IsBusy = true;

        await this.LoadUsers();
        await this.LoadEquipmentsCache();
        await this.LoadUserActivity();

        base.IsBusy = false;
        base.IsRefreshing = false;
    }
    #endregion Overrides
}

/// <summary>
/// This class represents a collection of punches for a user within a given day.
/// </summary>
internal class UserActionsForDay : ObservableObject
{
    public UserActionsForDay(UserDto user, UserDto editBy, DateTime date, IEnumerable<WorkPeriodDto> workPeriods, ObservableCollection<EquipmentDto> equipments) 
    {
        this.WorkPeriod = new(workPeriods);
        int weekdayAbbreviation = date.DayOfWeek == System.DayOfWeek.Thursday ? 4 : 3;
        this.Date = date;
        this.DayOfWeek = Enum.GetName(date.DayOfWeek)![..weekdayAbbreviation];
        this.Items = new SortedObservableCollection<UserActionsForDayItem, UserActionsForDayItem>(new UserActionsForDayItemComparer());
        this.AddPunchCommand = new AsyncRelayCommand(this.AddEntry);
        this.User = user;
        this.Equipments = equipments;
        this.EditBy = editBy;
        this.Items.CollectionChanged += this.Items_CollectionChanged;
//#if DEBUG
//        this.IsEditable = true;
//#else
        this.IsEditable = this.Date.IsBeforeCutoff();
//#endif
    }

    public DateTime Date { get; init; }
    public string DayOfWeek {  get; init; }
    public IAsyncRelayCommand AddPunchCommand { get; private set; }
    public List<WorkPeriodDto> WorkPeriod { get; set; }
    private async Task AddEntry()
    {
        UserDto editBy = this.EditBy;
        IPopupService PopupService = ServiceHelper.GetPopoupService()!;
        UserActionsForDay that = this;
        WorkPeriodDto? defaultWorkPeriod = this.WorkPeriod.FirstOrDefault() ?? this.WorkPeriod.LastOrDefault(); //.FirstOrDefault(w => w.JobTypeId == that.User.DefaultJobTypeId)
        // the popup will call SaveEditingCommand on the UserActionsForDayItem if user saves
        await PopupService.ShowPopupAsync<EditEntryViewModel>(m => {
            //m.IsCreating = true;
            m.UserActionsForDay = that;
            m.Date = that.Date;
            m.User = that.User;
            m.Editor = editBy;
            m.Device = Settings.DeviceGuid;
            m.Equipments = that.Equipments;
            m.WorkPeriod = defaultWorkPeriod;
            m.NewEntryMode = NewEntryMode.Creating;
            //m.SelectedNewEntryType = NewEntryType.PunchEntry;
            m.SetNewItem(NewEntryType.PunchEntry);
#warning need to set default jobtype
            //m.SelectedJobType = this.User?.DefaultJobStepId;
            //m.SelectedJobStep = this.User?.DefaultJobTypeId;
        });
    }
    public SortedObservableCollection<UserActionsForDayItem, UserActionsForDayItem> Items { get; internal set; }
    public int ItemsCount => this.Items.Count;
    public ObservableCollection<EquipmentDto> Equipments { get; init; }
    public bool IsEditable { get; init; }
    public UserDto EditBy { get; set; }
    public UserDto User { get; set; } 
    public JobTypeStepToUserDto? JobTypeStep { get; set; }

    private void Items_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        base.OnPropertyChanged(nameof(this.ItemsCount));

        switch (e.Action)
        {
            case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                //foreach(UserActionsForDayItem newItem in e.NewItems ?? new List<object>())
                //{
                //    newItem.CreatedBy = this._editBy;
                //    newItem.User = this._user;
                //}
                break;
            case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                break;
            case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                break;
            case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                break;
            case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                break;
            default:
                break;
        }
    }
}


[DebuggerDisplay("{Time}")]
internal class UserActionsForDayItem : ObservableObject
{
    #region Private Members
    private Time _time;
    private bool _isVoided;
    private string _currentActionDescription;
    private bool _isTimeChanged;
    private bool _isEquipmentChanged;
    private bool _isNewItem;
    private bool _isDirty;
    private bool _isUnlinkedEntry;
    private PunchStatusDto? _stableStatus;
    private UserActionsForDay _parent;
    private EquipmentDto? _equipment = null;
    private PunchEntryDto? _punchEntry = null;
    private EquipmentsToUserDto? _equipmentToUser = null;
    private ILogger Logger { get; init; }
    private ITimeClockApiAccessService ApiAccessService { get; init; }
    #endregion Private Members


    #region ctor
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private UserActionsForDayItem(UserActionsForDay parent, ILogger logger)
    {
        this.Logger = logger;
        this._parent = parent;
        this.PunchEntriesHistories = new ObservableCollection<PunchEntriesHistoryDto>();
        this.Equipments = new ObservableCollection<EquipmentDto>(Enumerable.Empty<EquipmentDto>());
        this._currentActionDescription = string.Empty;
        this.SetCommands();

        ITimeClockApiAccessService? apiService = ServiceHelper.GetApiService();

        if (apiService is null)
        {
            this.Logger.LogError("");
            throw new NullReferenceException($"API service not found in {nameof(UserActionsForDay)}");
        }
        this.ApiAccessService = apiService;
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public UserActionsForDayItem(UserDto user, UserDto actionBy, PunchEntryDto punchEntry, WorkPeriodDto? workPeriod, UserActionsForDay parent, ILogger logger) :this(parent, logger)
    {
        this.User = user;
        this.IsPunch = true;
        this.CreatedBy = actionBy;
        this.WorkPeriod = workPeriod;        
        this.SetAsPunchEntry(punchEntry);
        this.SetDependentDefaults();
    }
    public UserActionsForDayItem(UserDto user, UserDto actionBy, EquipmentsToUserDto equipmentToUser, ObservableCollection<EquipmentDto> equipments, WorkPeriodDto? workPeriod, UserActionsForDay parent, ILogger logger, bool isUnlinkedEntry = false) : this(parent, logger)
    {
        this.Equipments = equipments;
        this.IsPunch = false;
        this.User = user;
        this.CreatedBy = actionBy;
        this.WorkPeriod = workPeriod;
        this.IsUnlinkedEntry = isUnlinkedEntry;
        this.SetAsEquipmentEntry(equipmentToUser, equipments, isUnlinkedEntry);
        this.SetDependentDefaults();
    }
    #endregion ctor


    #region public properties
    /// <summary>
    /// Get or Set the time for the Entry item, PunchEntry DateTime or Equipment LinkedOn
    /// </summary>
    public Time Time 
    { 
        get => this._time;
        set
        {
            base.SetProperty(ref this._time, value);
            base.OnPropertyChanged(nameof(this.IsTimeChanged));
            base.OnPropertyChanged(nameof(this.IsDirty));
            base.SetProperty(ref this._effectiveTime, ((TimeSpan)value).RoundMinutes(15));
        }
    }
    private Time _effectiveTime;
    public Time EffectiveTime
    {
        get => this._effectiveTime;
        set => base.SetProperty(ref this._effectiveTime, value);
    }
    /// <summary>
    /// Represents this Entry's Time as a TimeSpan. 
    /// Alternate property for Time, needed for binding to TimePicker.
    /// </summary>
    public TimeSpan TimeSpan
    {
        get => this.Time;
        set 
        {
            this.Time = value;

            List<Time> times = this.Parent.Items.Select(i => i.Time).ToList();
            times.Add(this.Time);
            this.StableStatus = times.OrderBy(t => t)
                .Select((t, i) => i % 2 == 0 ? new { time = t, status = PunchStatusDto.In } : new { time = t, status = PunchStatusDto.Out })
                .First(v => v.time == this.Time).status;
        }
    }

    public bool IsUnlinkedEntry { get => this._isUnlinkedEntry; set => this._isUnlinkedEntry = value; }
    public bool IsVoided 
    { 
        get => this._isVoided; 
        set 
        {
            base.SetProperty(ref this._isVoided, value);
            base.OnPropertyChanged(nameof(this.IsDirty));
        }
    }
    public EquipmentDto? Equipment 
    { 
        get => this._equipment;
        set
        {
            base.SetProperty(ref this._equipment, value);
            base.OnPropertyChanged(nameof(this.IsEquipmentChanged));
            base.OnPropertyChanged(nameof(this.IsDirty));
        }
    }
    public string CurrentActionDescription { get => this._currentActionDescription; set => base.SetProperty(ref this._currentActionDescription, value); }

    internal Time OldTime { get; set; }
    internal EquipmentDto? OldEquipment { get; set; }
    public bool IsTimeChanged { get => this.Time != this.OldTime; private set => base.SetProperty(ref this._isTimeChanged, this.Time != this.OldTime); }
    public bool IsEquipmentChanged { get => this.Equipment != this.OldEquipment; private set => base.SetProperty(ref this._isEquipmentChanged, this.Equipment != this.OldEquipment); }
    internal bool IsNewItem 
    { 
        get => this._isNewItem;
        set
        {
            base.SetProperty(ref this._isNewItem, this.ItemId == default);
            base.OnPropertyChanged(nameof(this.IsNotNewItem));
        }
    }
    public bool IsNotNewItem
    {
        get => !this.IsNewItem;
        set
        {
            base.OnPropertyChanged(nameof(this.IsNotNewItem));
            base.OnPropertyChanged(nameof(this.IsNewItem));
        }
    }
    public bool IsDirty { get => this.GetIsDirty(); set => base.SetProperty(ref this._isDirty, this.GetIsDirty()); }
    public PunchStatusDto? StableStatus
    {
        get => this._stableStatus;
        set
        {
            base.SetProperty(ref this._stableStatus, value);
        }
    }

    public bool IsPunch { get; init; }
    public bool IsEquipment => !this.IsPunch;
    public DateTime Date { get; private set; }
    public Guid ItemId { get; private set; }
    public PunchTypeDto? PunchType { get; private set; }
    
    public UserActionsForDay Parent { get => this._parent; private set => this._parent = value; }
    public UserDto? User { get; set; }
    public UserDto? CreatedBy { get; set; }
    //public double? Longitude { get => this._punchEntry?.Longitude; }
    //public double? Latitude { get => this._punchEntry?.Latitude; }
    public string? Device { get; init; }
    public WorkPeriodDto? WorkPeriod { get; init; } // you cannot modify WorkPeriod for an item. You must void the item then create a new item.
    public ObservableCollection<PunchEntriesHistoryDto> PunchEntriesHistories { get; init; }
    public ObservableCollection<EquipmentDto> Equipments { get; init; }
    #endregion public properties


    #region Public Commands
    public IAsyncRelayCommand EditEntryCommand { get; private set; }
    public ICommand VoidEntryCommand { get; private set; }
    public IAsyncRelayCommand CancelEditingCommand { get; private set; } 
    public IAsyncRelayCommand SaveEditingCommand {  get; private set; }
    #endregion Public Commands


    #region Private Commands
    private async Task EditEntry()
    {
        this.Logger.LogInformation("Editing {username} entry for {time}", this.User?.FullNameOr, this.Time);
        UserActionsForDayItem that = this;
        IPopupService PopupService = ServiceHelper.GetPopoupService()!;
        this.CurrentActionDescription = string.Format(CommonValues.CurrEditEntry, "time punch");
        // the popup will call SaveEditingCommand on the UserActionsForDayItem if user saves
        await PopupService.ShowPopupAsync<EditEntryViewModel>(m =>
        {
            m.Item = that;
            m.Editor = that._parent.EditBy;
            m.User = this.User;
            m.WorkPeriod = that.WorkPeriod;
#warning need to set default jobtype
            //m.SelectedJobType = this.User?.DefaultJobStepId;
            //m.SelectedJobStep = this.User?.DefaultJobTypeId;
            m.SelectedEquipment = that.IsEquipment ? that.Equipment : null;
            m.NewEntryMode = NewEntryMode.Editing;
            //m.SelectedNewEntryType = NewEntryType.PunchEntry;
        });
    }
    private void VoidEntry()
    {
        this.IsVoided = true;
    }
    private Task CancelEditing()
    {
        if (this.IsDirty)
        {
            this.IsVoided = false;
            this.Time = this.OldTime;
            this.Equipment = this.OldEquipment;
        }
        this.Logger.LogInformation("Editing {username} entry cancelled for {time}", this.User?.FullNameOr, this.Time);
        return Task.CompletedTask;
    }
    private async Task SaveEditing()
    {
        if (this.IsPunch)
            await this.UpdateOrNewPunchEntry();
        else await this.UpdateOrNewEquipmentEntry();
    }
    #endregion Private Commands


    #region Private Methods
    private async Task UpdateOrNewEquipmentEntry()
    {
        throw new NotImplementedException();
    }
    private async Task UpdateOrNewPunchEntry()
    {
        string? key = await Settings.GetCryptoApiKey();
        ResultValues<PunchEntryDto?> result;
        UserDto currentEditor = this._parent.EditBy;
        CreatePunchEntryDto punchEntry = new()
        {
            ActionById = currentEditor?.Id ?? Guid.Empty,
#warning need to add AdminApproved state
            PunchAction = this.IsVoided ? PunchActionDto.Void : (this.IsNewItem ? PunchActionDto.AdminPunch : PunchActionDto.AdminEdit),
            DateTime = new DateTime(DateOnly.FromDateTime(this.Date), TimeOnly.FromTimeSpan(this.Time)),
            DeviceId = Settings.DeviceGuid,
            Id = this.IsNewItem ? Guid.NewGuid() : this._punchEntry?.Id ?? Guid.Empty, // this should throw instead of Guid.Empty
            JobStepId = this.Parent.JobTypeStep?.JobStep?.Id ?? this.User?.DefaultJobStepId,
            JobTypeId = this.Parent.JobTypeStep?.JobType?.Id ?? this.User?.DefaultJobTypeId,
            Latitude = Settings.Latitude,
            Longitude = Settings.Longitude,
            PunchType = PunchTypeDto.None,
            UserId = this.User?.Id, // this should throw instead of null
            UserName = this.User?.UserName ?? string.Empty,
            WorkPeriodId = this.WorkPeriod?.Id,
            LocationDivisionCode = Settings.LocationDivision,
            UnionCode = this.User?.UnionCode
        };

        if (string.IsNullOrWhiteSpace(key))
            throw new ApplicationException($"{nameof(SaveEditing)} {nameof(key)} was null or empty");
        if (currentEditor is null || currentEditor.Id == default)
            throw new ApplicationException($"{nameof(SaveEditing)} {nameof(this.CreatedBy)} was null or empty id");
        // should check if any of the above guid are empty, and throw

        if (this.IsDirty || this.IsNewItem)
        {
            List<UserActionsForDayItem> items = this._parent.Items.ToList();

            this.Logger.LogWarning("Saving entry edits for {username} {time}", this.User?.FullNameOr, this.Time);

            if (this.IsNewItem)
                result = await this.ApiAccessService.CreatePunchEntry(Settings.DeviceGuid, key, punchEntry);
            else // IsDirty
                result = await this.ApiAccessService.UpdatePunchEntry(Settings.DeviceGuid, key, punchEntry);

            if (!result.IsSuccessfulStatusCode || result.Value is null)
            {
#warning offline mode
                // this would be a good place to use local db so save while offline if code == offline
                // next api call check local for anything which needs to go out
                this.Logger.LogError("could not update/create punch entry. Method:{methodName}. Result:{apiResult}", nameof(this.UpdateOrNewPunchEntry), result.StatusCode);
                await UiHelpers.ShowOkAlert($"{(this.IsNewItem ? "creating" : "updating")} the punch entry was unsuccessful.\nCheck internet, try again or contact IT.");
                return;
            }
            else
            {
                this.ItemId = result.Value.Id;
                if (this.IsNewItem)
                {
                    items.Add(this);
                    this.IsNewItem = false;
                }
            }
            // clear the parent items and then re-add them so we can ensure correct ordering
            this._parent.Items.Clear();
            // we order by date and then time in case we have a punch from before midnight
            // the date can have incorrect time if it was edited. so then we order by time
            foreach (var item in items.Where(i => !i.IsVoided).OrderBy(i => i.Date.Date).ThenBy(i => i.Time))
            {
                item.SetToClean(); // remove dirty flag
                this.Logger.LogDebug("adding date item: {0}", item.Date.ToString("MM/dd/yyyy hh:mm tt"));
                this._parent.Items.Add(item);
            }

            this.Logger.LogWarning("Saved entry edits for {username} {time}", this.User?.FullNameOr, this.Time);
        }
    }
    /// <summary>
    /// Sets the defaults value for properties that are dependent on another property
    /// </summary>
    private void SetDependentDefaults()
    {
        this.Time = new Time(this.Date);
        this.OldTime = this.Time;
    }
    private void SetAsPunchEntry(PunchEntryDto punchEntry)
    {
        this._punchEntry = punchEntry;
        this._equipmentToUser = null;
        this.ItemId = punchEntry.Id;
        this.Date = punchEntry.DateTime;
        this.PunchType = punchEntry.PunchType;
        this.StableStatus = punchEntry.StablePunchStatus;
        this.SetPunchEntriesHistories(punchEntry.PunchEntriesHistories);
        //this.CurrentActionDescription = string.Format(CommonValues.CurrEditEntry, "time punch");
    }
    private void SetAsEquipmentEntry(EquipmentsToUserDto equipmentToUser, ObservableCollection<EquipmentDto> equipments, bool isUnlinkedEntry)
    {
        this._punchEntry = null;
        this._equipmentToUser = equipmentToUser;
        this.OldEquipment = equipments.FirstOrDefault(q => q.Id == equipmentToUser.EquipmentId);
        this.Equipment = this.OldEquipment;
        this.ItemId = equipmentToUser.Id;
        this.Date = isUnlinkedEntry ? (this._equipmentToUser.UnLinkedOn ?? DateTime.MinValue) : this._equipmentToUser.LinkedOn;
        this.PunchType = PunchTypeDto.User;
        //this.CurrentActionDescription = string.Format(CommonValues.CurrEditEntry, "equipment");

        if (this.Date == DateTime.MinValue) throw new ArgumentNullException(nameof(equipmentToUser), $"{nameof(isUnlinkedEntry)} is specified but {nameof(equipmentToUser.UnLinkedOn)} was null");
    }
    private async void SetPunchEntriesHistories(IEnumerable<PunchEntriesHistoryDto> histories)
    {
        await Task.Run(() =>
        {
            foreach (var item in histories)
                this.PunchEntriesHistories.Add(item);
        });
    }
    private void SetCommands()
    {
        this.EditEntryCommand = new AsyncRelayCommand(this.EditEntry);
        this.VoidEntryCommand = new Command(this.VoidEntry);
        this.CancelEditingCommand = new AsyncRelayCommand(this.CancelEditing);
        this.SaveEditingCommand = new AsyncRelayCommand(this.SaveEditing);
    }

    private void SetToClean()
    {
        this.OldTime = this.Time;
        this.Date = new DateTime(this.Date.Year, this.Date.Month, this.Date.Day, this.Time.Hours, this.Time.Minutes, this.Time.Seconds);
        this.OldEquipment = this.Equipment;
    }

    private bool GetIsDirty()
    {
        return this.IsTimeChanged || this.IsEquipmentChanged || this.IsNewItem || this.IsVoided;
    }
    #endregion Private Methods
}

internal class UserActionsForDayItemComparer : IComparer<UserActionsForDayItem>
{
    public int Compare(UserActionsForDayItem? x, UserActionsForDayItem? y)
    {
        var val1 = x?.Date.Date ?? DateTime.MinValue;
        var val2 = y?.Date.Date ?? DateTime.MinValue;

        var dateResult = val1.CompareTo(val2);

        if (dateResult == 0)
            return (x?.Time ?? default).CompareTo(y?.Time ?? default);
        return dateResult;
    }
}
