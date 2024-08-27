using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.ComponentModel;
using TimeClock.Core.Models.EntityDtos;
using TimeClock.Maui.Helpers;

namespace TimeClock.Maui.ViewModels.Shared;

public interface IViewModel
{
    IAsyncRelayCommand CloseCommand { get; }
    DateTime FilterDate { get; set; }
    bool IsBusy { get; set; }
    bool IsNotBusy { get; }
    bool IsRefreshing { get; set; }
    IAsyncRelayCommand RefreshCommand { get; }
    string UserEmployeeNumber { get; set; }
    string UserFirstName { get; set; }
    Guid UserId { get; set; }
    string UserLastName { get; set; }
    string UserName { get; set; }
    string UserFullNameOr { get; }
    void ImportUser(IViewModel viewModel);
    ObservableCollection<string> UserClaims { get; }
    Guid? UserJobStepId { get; set; }
    Guid? UserJobTypeId { get; set; }
    int PunchType { get; set; }
    bool RefreshOnPropertyChange { get; set; }
    string Union { get; set; }
}

//[QueryProperty(nameof(ViewModelBase.UserId), nameof(UserDto.Id))]
//[QueryProperty(nameof(ViewModelBase.UserFirstName), nameof(UserDto.FirstName))]
//[QueryProperty(nameof(ViewModelBase.UserLastName), nameof(UserDto.LastName))]
//[QueryProperty(nameof(ViewModelBase.UserEmployeeNumber), nameof(UserDto.EmployeeNumber))]
//[QueryProperty(nameof(ViewModelBase.UserName), nameof(UserDto.UserName))]
public abstract partial class ViewModelBase : ObservableObject, IViewModel, IQueryAttributable
{
    protected DateTime _filterDate;
    protected bool _isBusy;
    protected bool _isNotBusy;
    protected bool _isRefreshing;
    protected Guid _userId;
    protected string _userFirstName;
    protected string _userLastName;
    protected string _userEmployeeNumber;
    protected string _userName;
    protected Guid? _userJobStepId;
    protected Guid? _userJobTypeId;
    protected int _punchType;
    protected string _union;
    private bool _isMobileDevice;
    private bool _isNotMobileDevice;
    private bool _refreshOnPropertyChange;
    private ILogger<ViewModelBase>? Logger { get; set; }
    public ViewModelBase()
    {
        this.CloseCommand = new AsyncRelayCommand(Close);
        this.RefreshCommand = new AsyncRelayCommand(Refresh);
        this.IsBusy = false;
        this.IsNotBusy = !this.IsBusy;
        this.IsRefreshing = false;
        this._userFirstName = string.Empty;
        this._userLastName = string.Empty;
        this._userEmployeeNumber = string.Empty;
        this._userName = string.Empty;
        this._refreshOnPropertyChange = true;
        this._union = string.Empty;
        this.UserClaims = new ObservableCollection<string>();
        this.Logger = ServiceHelper.GetLoggerService<ViewModelBase>();
        this.IsMobileDevice = DeviceInfo.Current.IsMobile();
        this.IsNotMobileDevice = !this.IsMobileDevice;
    }


    public virtual Guid UserId
    {
        get => this._userId;
        set => base.SetProperty(ref this._userId, value);
    }
    public string UserFirstName
    {
        get => this._userFirstName;
        set
        {
            base.SetProperty(ref this._userFirstName, value);
            base.OnPropertyChanged(nameof(this.UserFullNameOr));
        }
    }
    public string UserLastName
    {
        get => this._userLastName;
        set
        {
            base.SetProperty(ref this._userLastName, value);
            base.OnPropertyChanged(nameof(this.UserFullNameOr));
        }
    }
    public string UserEmployeeNumber
    {
        get => this._userEmployeeNumber;
        set => base.SetProperty(ref this._userEmployeeNumber, value);
    }
    public string Union
    {
        get => this._userEmployeeNumber;
        set => base.SetProperty(ref this._userEmployeeNumber, value);
    }
    public string UserName
    {
        get => this._userName;
        set
        {
            base.SetProperty(ref this._userName, value);
            base.OnPropertyChanged(nameof(this.UserFullNameOr));
        }
    }
    public string UserFullNameOr
    {
        get
        {
            string fullName = $"{this.UserFirstName} {this.UserLastName}".Trim();
            return string.IsNullOrWhiteSpace(fullName) ? this.UserName : fullName;
        }
    }
    public Guid? UserJobStepId
    {
        get => this._userJobStepId;
        set => base.SetProperty(ref this._userJobStepId, value);
    }
    public Guid? UserJobTypeId
    {
        get => this._userJobTypeId;
        set => base.SetProperty(ref this._userJobTypeId, value);
    }
    public ObservableCollection<string> UserClaims { get; private set; }
    public virtual DateTime FilterDate
    {
        get => this._filterDate;
        set => base.SetProperty(ref this._filterDate, value);
    }
    public virtual bool IsBusy
    {
        get => this._isBusy;
        set
        {
            base.SetProperty(ref this._isBusy, value);
            this.IsNotBusy = !value;
        }
    }
    public virtual bool IsNotBusy
    {
        get => this._isNotBusy;
        private set => base.SetProperty(ref this._isNotBusy, value);
    }
    public virtual bool IsRefreshing
    {
        get => this._isRefreshing;
        set => base.SetProperty(ref this._isRefreshing, value);
    }
    public virtual int PunchType
    {
        get => this._punchType;
        set => base.SetProperty(ref this._punchType, value);
    }
    public bool IsMobileDevice
    {
        get => this._isMobileDevice;
        private set => base.SetProperty(ref this._isMobileDevice, value);
    }
    public bool IsNotMobileDevice
    {
        get => this._isNotMobileDevice;
        private set => base.SetProperty(ref this._isNotMobileDevice, value);
    }
    /// <summary>
    /// Determines if ViewModel should refresh whenever <see cref="UserId"/> or <see cref="FilterDate"/> change
    /// </summary>
    public virtual bool RefreshOnPropertyChange
    {
        get => this._refreshOnPropertyChange;
        set => base.SetProperty(ref this._refreshOnPropertyChange, value);
    }

    public virtual IAsyncRelayCommand CloseCommand { get; protected set; }
    public virtual IAsyncRelayCommand RefreshCommand { get; protected set; }
    /// <summary>
    /// Imports User related fields, including <see cref="FilterDate"/>
    /// </summary>
    /// <param name="viewModel"></param>
    public virtual void ImportUser(IViewModel viewModel)
    {
        this.FilterDate = viewModel.FilterDate;
        this.UserEmployeeNumber = viewModel.UserEmployeeNumber;
        this.UserFirstName = viewModel.UserFirstName;
        this.UserId = viewModel.UserId;
        this.UserLastName = viewModel.UserLastName;
        this.UserName = viewModel.UserName;
        this.UserClaims.Clear();
        foreach (var item in viewModel.UserClaims)
            this.UserClaims.Add(item);
        this.UserJobTypeId = viewModel.UserJobTypeId;
        this.UserJobStepId = viewModel.UserJobStepId;
        this.PunchType = viewModel.PunchType;
    }

    /// <summary>
    /// Reloads data
    /// </summary>
    /// <returns></returns>
    protected abstract Task Refresh();
    /// <summary>
    /// Closes the current page by navigating back one.
    /// </summary>
    /// <returns></returns>
    protected virtual Task Close()
    {
        if (Shell.Current is null)
            return Task.CompletedTask;
        Shell.Current.Navigation.ClearStack();
        return Shell.Current.GoToAsync("..");
    }


    #region Overrides
    // fixes issue which happens when a implementor loads, as each property gets assigned, RefreshCommand gets called twice - once for each
    private int LoadedPropertyCount { get; set; } = 0;
    protected override async void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if ((e.PropertyName == nameof(this.FilterDate) || e.PropertyName == nameof(this.UserId)) && this.RefreshOnPropertyChange)
        {
            if (this.LoadedPropertyCount > 0)
                await this.RefreshCommand.ExecuteAsync(this);

            this.Logger?.LogDebug("****************** {nameof(this.LoadedPropertyCount)}: {LoadedPropertyCount} **********************", nameof(this.LoadedPropertyCount), this.LoadedPropertyCount);
            this.LoadedPropertyCount++;
        }
    }
    #endregion Overrides


    #region IQueryAttributable
    public virtual void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        object? user;
        UserDto? userDto;
        object? punchTypeObject;
        int punchType;

        query.TryGetValue(nameof(UserDto), out user);
        query.TryGetValue(nameof(this.PunchType), out punchTypeObject);

        if (punchTypeObject is not null && int.TryParse(punchTypeObject.ToString(), out punchType))
        {
            this.PunchType = punchType;
        }

        if (user is not null && (userDto = user as UserDto) is not null)
        {
            this.UserFirstName = userDto.FirstName;
            this.UserLastName = userDto.LastName;
            this.UserName = userDto.UserName ?? string.Empty;
            this.UserEmployeeNumber = userDto.EmployeeNumber;
            this.UserClaims = new ObservableCollection<string>(userDto.UserClaims.Where(c => c.AuthorizationClaim is not null).Select(c => c.AuthorizationClaim!.Type));
            this.UserId = userDto.Id;
            this.UserJobTypeId = userDto.DefaultJobTypeId;
            this.UserJobStepId = userDto.DefaultJobStepId;
            this.Union = userDto.UnionCode ?? string.Empty;
        }
    }
    #endregion IQueryAttributable
}
