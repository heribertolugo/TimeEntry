using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using TimeClock.Core.Models.EntityDtos;
using TimeClock.Maui.Helpers;

namespace TimeClock.Maui.ViewModels.AdminSection;

internal class EditEntryViewModel : ObservableObject
{
    private UserActionsForDayItem? _item;
    private bool _isCreating;
    private NewEntryType _selectedNewType;
    private NewEntryMode _newEntryMode;
    private DateTime _date;
    private UserDto? _user;
    private UserDto? _editor;
    private Guid? _device;
    private EquipmentDto? _selectedEquipment;
    private ObservableCollection<EquipmentDto> _equipments;
    private ILogger<EditEntryViewModel> Logger {  get; init; }

    public EditEntryViewModel()
    {
        ILogger<EditEntryViewModel>? logger = ServiceHelper.GetLoggerService<EditEntryViewModel>();
        if (logger is null)
            throw new NullReferenceException($"{nameof(logger)} was not found in services");
        this.Logger = logger;
        this._equipments = new ObservableCollection<EquipmentDto>();
        this._jobTypes = new ObservableCollection<JobTypeDto>();
        this._jobSteps = new ObservableCollection<JobStepDto>();
    }

    public UserActionsForDayItem? Item { get => this._item; set => base.SetProperty(ref this._item, value); }
    /// <summary>
    /// Keeps track of the state of the UI. In creating state the user is presented with the options of which type of entry to create, PunchEntry vs Equipment. 
    /// If this is set, UserActionsForDay needs to be set - as selecting an entry option will trigger a UserActionsForDayItem to be created for the provided UserActionsForDay.
    /// </summary>
    public bool IsCreating
    {
        get => this._isCreating;
        set
        {
            base.SetProperty(ref this._isCreating, value);
            base.OnPropertyChanged(nameof(this.IsNotCreating));
        }
    }
    public bool IsNotCreating 
    { 
        get => !this._isCreating; 
        set => this.IsCreating = !value; 
    }
    /// <summary>
    /// Setting this property will trigger a new UserActionsForDayItem to be created and attached to the provided UserActionsForDay. 
    /// If no UserActionsForDay is set, an exception will be thrown.
    /// </summary>
    public NewEntryType SelectedNewEntryType
    {
        get => this._selectedNewType;
        set
        {
            base.SetProperty(ref this._selectedNewType, value);
            this.SetNewItem(value);
        }
    }
    public DateTime Date
    {
        get => this._date;
        set => base.SetProperty(ref this._date, value);
    }
    public UserDto? User
    {
        get => this._user;
        set => base.SetProperty(ref this._user, value);
    }
    public UserDto? Editor
    {
        get => this._editor;
        set => base.SetProperty(ref this._editor, value);
    }
    public Guid? Device
    {
        get => this._device;
        set => base.SetProperty(ref this._device, value);
    }
    public NewEntryMode NewEntryMode
    {
        get => this._newEntryMode;
        set => base.SetProperty(ref this._newEntryMode, value);
    }
    public ObservableCollection<EquipmentDto> Equipments
    {
        get => this._equipments;
        set => base.SetProperty(ref this._equipments, value);
    }
    public EquipmentDto? SelectedEquipment
    {
        get => this._selectedEquipment;
        set
        {
            base.SetProperty(ref this._selectedEquipment, value);
            this.Item!.Equipment = value;
        }
    }
    private UserActionsForDay? _userActionsForDay;
    public UserActionsForDay? UserActionsForDay 
    {
        get => this._userActionsForDay;
        set => this._userActionsForDay = value;
    }
    private ObservableCollection<JobTypeDto> _jobTypes;
    public ObservableCollection<JobTypeDto> JobTypes
    {
        get => this._jobTypes;
        set => base.SetProperty(ref this._jobTypes, value);
    }
    private ObservableCollection<JobStepDto> _jobSteps;
    public ObservableCollection<JobStepDto> JobSteps
    {
        get => this._jobSteps;
        set => base.SetProperty(ref this._jobSteps, value);
    }
    private JobTypeDto? _selectedJobType;
    public JobTypeDto? SelectedJobType
    {
        get => this._selectedJobType;
        set => base.SetProperty(ref this._selectedJobType, value);
    }
    private JobStepDto? _selectedJobStep;
    public JobStepDto? SelectedJobStep
    {
        get => this._selectedJobStep;
        set => base.SetProperty(ref this._selectedJobStep, value);
    }
    private WorkPeriodDto? _workPeriod;
    public WorkPeriodDto? WorkPeriod
    {
        get => this._workPeriod;
        set => base.SetProperty(ref this._workPeriod, value);
    }

    public void SetNewItem(NewEntryType newType)
    {
        switch (newType)
        {
            case NewEntryType.PunchEntry:
                if (this.User is null || this.Editor is null)
                    throw new ApplicationException("User and Editor are required");
                if (this.UserActionsForDay is null)
                    throw new ApplicationException($"{nameof(this.UserActionsForDay)} is required");

                this.Item = new UserActionsForDayItem(this.User, this.Editor,
                    new PunchEntryDto(dateTime: this.Date, userId: this.User?.Id ?? Guid.Empty, punchType: PunchTypeDto.User, action: PunchActionDto.AdminPunch,
                        actionById: this.Editor?.Id ?? Guid.Empty, deviceId: this.Device ?? Guid.Empty),
                    this.WorkPeriod, this.UserActionsForDay, this.Logger)
                { IsNewItem = true, IsDirty = true, CurrentActionDescription = this.NewEntryMode == NewEntryMode.None || this.NewEntryMode == NewEntryMode.Editing 
                    ? string.Format(CommonValues.CurrEditEntry, "time punch")
                    : string.Format(CommonValues.CurrNewEntry, "time punch")
                };
                this.IsCreating = false;
                break;
            case NewEntryType.EquipmentEntry:
                if (this.User is null || this.Editor is null)
                    throw new ApplicationException("User and Editor are required");
                if (this.UserActionsForDay is null)
                    throw new ApplicationException($"{nameof(this.UserActionsForDay)} is required");

                this.Item = new UserActionsForDayItem(this.User, this.Editor,
                new EquipmentsToUserDto(){ LinkedOn = this.Date, UserId = this.User?.Id ?? Guid.Empty, LinkedById = this.Editor?.Id ?? Guid.Empty, EquipmentId = this.SelectedEquipment?.Id ?? Guid.Empty }, 
                this.Equipments, this.WorkPeriod, this.UserActionsForDay, this.Logger)
                    { IsNewItem = true, IsDirty = true,
                    CurrentActionDescription = this.NewEntryMode == NewEntryMode.None || this.NewEntryMode == NewEntryMode.Editing
                    ? string.Format(CommonValues.CurrEditEntry, "equipment")
                    : string.Format(CommonValues.CurrNewEntry, "equipment")
                };
                this.IsCreating = false;
                break;
            case NewEntryType.None:
            default:
                break;
        }
    }
}

public enum NewEntryType
{
    None,
    PunchEntry,
    EquipmentEntry
}

public enum NewEntryMode
{
    None,
    Editing,
    Creating
}
