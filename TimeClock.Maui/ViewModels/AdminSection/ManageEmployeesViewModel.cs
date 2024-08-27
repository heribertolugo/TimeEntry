using CommunityToolkit.Mvvm.ComponentModel;
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


internal class ManageEmployeesViewModel : ViewModelBase
{
    #region Private Members
    private UserDto? _selectedEmployee;
    private string? _employeeBarcode;
    private ObservableCollection<UserDto> _users;
    private ObservableCollection<JobStepDto> _jobSteps = new();
    private ObservableCollection<JobTypeDto> _jobTypes;
    private bool _isAddingNewJobTypeStep;
    private bool _isNotAddingNewJobTypeStep;
    private ITimeClockApiAccessService ApiAccessService { get; init; }
    private ILogger<ManageEmployeesViewModel> Logger { get; init; }
    private JobTypeDto? SelectedJobType { get; set; }
    private JobStepDto? SelectedJobStep { get; set; }
    #endregion Private Members


    public ManageEmployeesViewModel(ITimeClockApiAccessService apiAccessService, ILogger<ManageEmployeesViewModel> logger)
    {
        this.ApiAccessService = apiAccessService;
        this.Logger = logger;
        this.SetDefaults();
        this.SetCommands();
    }


    #region Public Properties
    public string? EmployeeBarcode
    {
        get { return this._employeeBarcode; }
        set { base.SetProperty(ref this._employeeBarcode, value); }
    }
    public ObservableCollection<UserDto> Users
    {
        get => this._users;
        private set => this._users = value;
    }
    public UserDto? SelectedEmployee
    {
        get { return this._selectedEmployee; }
        set 
        { 
            base.SetProperty(ref this._selectedEmployee, value);
            this.EmployeeBarcode = value?.Barcodes.FirstOrDefault()?.Value;
            this.SelectedUserJobTypeSteps.Clear();
            if (this._selectedEmployee is not null)
            {
                foreach (var jobTypeStep in this._selectedEmployee.JobTypeSteps
                    .Select(j => new BindableJobTypeStepToUser(j)))
                {
                    this.SelectedUserJobTypeSteps.Add(jobTypeStep);
                } 
            }
        }
    }
    public ObservableCollection<BindableJobTypeStepToUser> SelectedUserJobTypeSteps { get; private set; } = new();
    public ObservableCollection<JobStepDto> JobSteps
    {
        get { return this._jobSteps; }
        set { base.SetProperty(ref this._jobSteps, value); }
    }
    public ObservableCollection<JobTypeDto> JobTypes
    {
        get { return this._jobTypes; }
        set { base.SetProperty(ref this._jobTypes, value); }
    }
    public bool IsAddingNewJobTypeStep
    {
        get { return this._isAddingNewJobTypeStep; }
        set 
        { 
            base.SetProperty(ref this._isAddingNewJobTypeStep, value);
            this.IsNotAddingNewJobTypeStep = !value;
        }
    }
    public bool IsNotAddingNewJobTypeStep
    {
        get { return this._isNotAddingNewJobTypeStep; }
        set { base.SetProperty(ref this._isNotAddingNewJobTypeStep, value); }
    }
    #endregion Public Properties


    #region Public Commands
    public IAsyncRelayCommand UserSelectedCommand { get; private set; }
    public IAsyncRelayCommand ClearSelectedUserCommand { get; private set; }
    public IAsyncRelayCommand SaveCommand { get; private set; }
    public IAsyncRelayCommand SearchAn8Command { get; private set; }
    public IAsyncRelayCommand NewJobTypeSelectedCommand { get; set; }
    public IAsyncRelayCommand NewJobStepSelectedCommand { get; set; }
    public IAsyncRelayCommand AddNewJobTypeStepRequestedCommand { get; set; }
    public IAsyncRelayCommand AddNewJobTypeStepConfirmedCommand { get; set; }
    public IAsyncRelayCommand AddNewJobTypeStepCanceledCommand { get; set; }
    #endregion Public Commands


    #region Private Commands
    private async Task SearchAn8(string? query)
    {
        int an8;

        if (!int.TryParse(query, out an8))
        {
            await UiHelpers.ShowOkAlert("AN8 Is Invalid!");
            return;
        }

        UserDto? existing;
        if ((existing = this.Users.FirstOrDefault(u => u.JdeId == an8)) is not null)
        {
            this.SelectedEmployee = existing;
            return;
        }

        Guid deviceId = Settings.DeviceGuid;
        GetUserDto dto = new()
        {
            DeviceId = deviceId,
            RequestedById = this.UserId,
            JdeId = an8,
            UserActiveState = true
        };

        var response = await this.ApiAccessService.GetUser(deviceId, Guid.Empty, dto);

        if (response == null || !response.IsSuccessfulStatusCode || response.Value is null)
        {
            await UiHelpers.ShowOkAlert($"Could not find user with AN8 {an8}");
            return;
        }

        if ((existing = this.Users.FirstOrDefault(u => u.Id == response.Value.Id)) is not null)
        {
            this.SelectedEmployee = existing;
            return;
        }

        this.Users.Add(response.Value);
        this.SelectedEmployee = response.Value;

        return;
    }
    private Task UserSelected(object? user)
    {
        return Task.CompletedTask;
    }
    private Task ClearSelectedUser()
    {
        this.SelectedEmployee = null;
        return Task.CompletedTask;
    }
    private async Task<bool> Save()
    {
        if (this.SelectedEmployee is null)
        {
            await UiHelpers.ShowOkAlert("No user selected to save!");
            return false;
        } 
        
        return await this.SaveExistingUser();
    }
    private Task NewJobTypeSelected(object? value)
    {
        this.SelectedJobType = value as JobTypeDto;
        return Task.CompletedTask;
    }
    private Task NewJobStepSelected(object? value)
    {
        this.SelectedJobStep = value as JobStepDto;
        return Task.CompletedTask;
    }
    private Task AddNewJobTypeStepRequested()
    {
        if (this.SelectedEmployee is null)
        {
            return UiHelpers.ShowOkAlert("Must select an employee first!");
        }
        this.IsAddingNewJobTypeStep = true;
        return Task.CompletedTask;
    }
    private Task AddNewJobTypeStepConfirmed()
    {
        if (this.SelectedEmployee is null)
        {
            return UiHelpers.ShowOkAlert("Must select an employee first!");
        }

        if (this.SelectedEmployee.JobTypeSteps.Any( j => j.JobTypeId == this.SelectedJobType?.Id && j.JobStepId == this.SelectedJobStep?.Id))
        {
            this.IsAddingNewJobTypeStep = false;
            return UiHelpers.ShowOkAlert("Cannot add an already existing Job Type and Job Step");
        }

        JobTypeStepToUserDto jobTypeStep = new()
        {
            IsActive = true,
            JobStep = this.SelectedJobStep,
            JobStepId = this.SelectedJobStep?.Id,
            JobType = this.SelectedJobType,
            JobTypeId = this.SelectedJobType?.Id,
            User = this.SelectedEmployee,
            UserId = this.SelectedEmployee.Id
        };

        this.SelectedEmployee?.JobTypeSteps.Add(jobTypeStep);
        this.SelectedUserJobTypeSteps.Add(new(jobTypeStep));
        this.IsAddingNewJobTypeStep = false;

        return Task.CompletedTask;
    }
    private Task AddNewJobTypeStepCanceled()
    {
        this.IsAddingNewJobTypeStep = false;
        return Task.CompletedTask;
    }
    #endregion Private Commands


    #region Private Methods
    private void SetDefaults()
    {
        this._employeeBarcode = string.Empty;
        this.Users = new ObservableCollection<UserDto>();
        this.JobTypes = new ObservableCollection<JobTypeDto>();
        this.IsAddingNewJobTypeStep = false;
    }
    private void SetCommands()
    {
        this.UserSelectedCommand = new AsyncRelayCommand((u) => this.UserSelected(u));
        this.SaveCommand = new AsyncRelayCommand(this.Save);
        this.ClearSelectedUserCommand = new AsyncRelayCommand(this.ClearSelectedUser);
        this.SearchAn8Command = new AsyncRelayCommand<string>(this.SearchAn8);
        this.NewJobTypeSelectedCommand = new AsyncRelayCommand<object?>(this.NewJobTypeSelected);
        this.NewJobStepSelectedCommand = new AsyncRelayCommand<object?>(this.NewJobStepSelected);
        this.AddNewJobTypeStepRequestedCommand = new AsyncRelayCommand(this.AddNewJobTypeStepRequested);
        this.AddNewJobTypeStepConfirmedCommand = new AsyncRelayCommand(this.AddNewJobTypeStepConfirmed);
        this.AddNewJobTypeStepCanceledCommand = new AsyncRelayCommand(this.AddNewJobTypeStepCanceled);
    }

    private async Task<bool> SaveExistingUser()
    {
#if !DEBUG
        if (!string.IsNullOrWhiteSpace(this.EmployeeBarcode) && (this.EmployeeBarcode.Trim().Length < 6 || this.EmployeeBarcode.Trim().Length > 20))
        {
            await UiHelpers.ShowOkAlert("Barcode must be between 6 and 20 characters!");
            return false;
        }
#endif
        Guid deviceId = Settings.DeviceGuid;
        UpdateUserDto dto = new()
        {
            DeviceId = deviceId,
            UserId = this.SelectedEmployee!.Id,
            ActionById = this.UserId,
            Barcode = this.EmployeeBarcode,
            Username = this.SelectedEmployee.UserName,
            JobTypeSteps = this.SelectedEmployee?.JobTypeSteps.Select(j => new JobTypeStepToUserDto()
            {
                Id = j.Id,
                IsActive = j.IsActive,
                JobStepId = j.JobStepId,
                JobTypeId = j.JobTypeId,
                UserId = j.UserId
            }).ToList() ?? []
        };

        var result = await this.ApiAccessService.UpdateUser(deviceId, dto);

        if (result is null || !result.IsSuccessfulStatusCode)
        {
            await UiHelpers.ShowOkAlert($"Could not save user. {result?.StatusCode ?? System.Net.HttpStatusCode.Unused}");
            this.Logger.LogError("Could not save user. apiResult: {apiResult}. data: {dto}", result?.StatusCode ?? System.Net.HttpStatusCode.Unused, dto.Wrap());
            return false;
        }

        await UiHelpers.ShowOkAlert("Successfully saved changes!", "");

        // a cheat to update the barcodes for this user. 
        // this is just in case selected user is changed, and then selected again, so we can display the new saved barcode
        this.SelectedEmployee.Barcodes.Clear();
        if (!string.IsNullOrWhiteSpace(this.EmployeeBarcode))
            this.SelectedEmployee.Barcodes.Add(new BarcodeDto() { Value = this.EmployeeBarcode });

        return true;
    }

    private async Task LoadUsers(bool forceRefresh = false)
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
                UserActiveState = true,
                IncludeBarCode = true,
                IncludeJobTypeSteps = false,
                Paging = new PagingDto(1, 100)
            };

            ResultValues<IEnumerable<UserDto>?> result = await this.ApiAccessService.GetUsers(data.DeviceId, data);
            IEnumerable<UserDto> users;

            if (!result.IsSuccessfulStatusCode || result.Value is null)
            {
                this.Logger.LogError("Unable to load users ApiResult: {statusCode}", result.StatusCode);
                await UiHelpers.ShowOkAlert("Unable to load users. Check internet or contact IT");
                return;
            }

            users = result.Value;

            this._users.Clear();

            foreach (UserDto user in users
                .Where(u => !u.UserClaims
                    // filter out other users who can edit punches (like other managers). unless it is the one who is currently logged in
                    .Any(c => c.AuthorizationClaim?.Type.Equals(Enum.GetName(AuthorizationClaimType.CanEditOthersPunches), StringComparison.InvariantCultureIgnoreCase) ?? false) || u.Id == this.UserId)
                .OrderBy(u => u.FullNameOr))
            {
                this._users.Add(user);
            }
        }
    }
    private async Task LoadJobTypes(bool forceRefresh = false)
    {
        return;
        if (this.JobTypes.Count > 0 && !forceRefresh)
            return;

        GenericGetDto data = new()
        {
            DeviceId = Settings.DeviceGuid,
            RequestedById = this.UserId,
            Timestamp = DateTime.Now
        };

        ResultValues<IEnumerable<JobTypeDto>?> result = await this.ApiAccessService.GetJobTypes(data.DeviceId, data, (await Settings.GetCryptoApiKey()) ?? string.Empty);
        IEnumerable<JobTypeDto> jobTypes;

        if (!result.IsSuccessfulStatusCode || result.Value is null)
        {
            this.Logger.LogError("Unable to load job types. ApiResult: {statusCode}", result.StatusCode);
            await UiHelpers.ShowOkAlert("Unable to job types. Check internet or contact IT");
            return;
        }

        jobTypes = result.Value;

        this.JobTypes.Clear();

        foreach (var type in jobTypes)
        {
            this.JobTypes.Add(type);
        }
    }
    private async Task LoadJobSteps(bool forceRefresh = false)
    {
        return;
        if (this.JobSteps.Count > 0 && !forceRefresh)
            return;

        GenericGetDto data = new()
        {
            DeviceId = Settings.DeviceGuid,
            RequestedById = this.UserId,
            Timestamp = DateTime.Now
        };

        ResultValues<IEnumerable<JobStepDto>?> result = await this.ApiAccessService.GetJobSteps(data.DeviceId, data, (await Settings.GetCryptoApiKey()) ?? string.Empty);
        IEnumerable<JobStepDto> jobSteps;

        if (!result.IsSuccessfulStatusCode || result.Value is null)
        {
            this.Logger.LogError("Unable to load job steps. ApiResult: {statusCode}", result.StatusCode);
            await UiHelpers.ShowOkAlert("Unable to job steps. Check internet or contact IT");
            return;
        }

        jobSteps = result.Value;

        this.JobSteps.Clear();

        foreach (var step in jobSteps)
        {
            this.JobSteps.Add(step);
        }
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

        await Task.WhenAll(
            this.LoadUsers(),
            this.LoadJobTypes(),
            this.LoadJobSteps()
            );

        base.IsBusy = false;
        base.IsRefreshing = false;
    }
    #endregion Overrides
}

public class BindableJobTypeStepToUser : ObservableObject
{
    private JobTypeStepToUserDto? _from;
    public BindableJobTypeStepToUser() { }
    public BindableJobTypeStepToUser(JobTypeStepToUserDto from)
    {
        this.Id = from.Id;
        this.IsActive = from.IsActive;
        this.JobTypeId = from.JobTypeId;
        this.JobStepId = from.JobStepId;
        this.JobStep = from.JobStep;
        this.JobType = from.JobType;
        this.UserId = from.UserId;
        this._from = from;
    }
    private Guid _id;
    public Guid Id
    {
        get => this._id;
        set 
        { 
            base.SetProperty(ref this._id, value);
            if (this._from is not null)
                this._from.Id = value;
        }
    }
    private bool _isActive;
    public bool IsActive
    {
        get => this._isActive;
        set
        {
            base.SetProperty(ref this._isActive, value);
            if (this._from is not null)
                this._from.IsActive = value;
        }
    }
    private Guid? _jobTypeId;
    public Guid? JobTypeId
    {
        get => this._jobTypeId;
        set
        {
            base.SetProperty(ref this._jobTypeId, value);
            if (this._from is not null)
                this._from.JobTypeId = value;
        }
    }
    private Guid? _jobStepId;
    public Guid? JobStepId
    {
        get => this._jobStepId;
        set
        {
            base.SetProperty(ref this._jobStepId, value);
            if (this._from is not null)
                this._from.JobStepId = value;
        }
    }
    private Guid _userId;
    public Guid UserId
    {
        get => this._userId;
        set
        {
            base.SetProperty(ref this._userId, value);
            if (this._from is not null)
                this._from.UserId = value;
        }
    }
    private JobTypeDto? _jobType;
    public JobTypeDto? JobType
    {
        get => this._jobType;
        set
        {
            base.SetProperty(ref this._jobType, value);
            if (this._from is not null)
                this._from.JobType = value;
        }
    }
    private JobStepDto? _jobStep;
    public JobStepDto? JobStep
    {
        get => this._jobStep;
        set
        {
            base.SetProperty(ref this._jobStep, value);
            if (this._from is not null)
                this._from.JobStep = value;
        }
    }
}
