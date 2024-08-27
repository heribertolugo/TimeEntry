using Microsoft.EntityFrameworkCore;
using TimeClock.Data.Models;
using TimeClock.Data.Repository;

namespace TimeClock.Data;

public interface IDataRepositoryFactory
{
    void Dispose();
    TimeClockContext Context { get; }
    IDataRepository<T> GetRepository<T>() where T : class, IEntityModel;
    IDepartmentsRepository GetDepartmentsRepository();
    IDevicesRepository GetDevicesRepository();
    IEquipmentsRepository GetEquipmentsRepository();
    IEquipmentTypesRepository GetEquipmentTypesRepository();
    ILocationsRepository GetLocationsRepository();
    IPunchEntriesRepository GetPunchEntriesRepository();
    IUsersRepository GetUsersRepository();
    IEventAuditRepository GetEventAuditsRepository();
    IUnregisteredDevicesRepository GetUnregisteredDevicesRepository();
    IDepartmentsToLocationsRepository GetDepartmentsToLocationsRepository();
    IAuthorizationClaimsRepository GetAuthorizationClaimsRepository();
    IUserClaimsRepository GetUserClaimsRepository();
    IBarcodesRepository GetBarcodesRepository();
    IWorkPeriodsRepository GetWorkPeriodsRepository();
    IJobTypesRepository GetJobTypesRepository();
    IJobStepsRepository GetJobStepsRepository();
    ISentEmailsRepository GetSentEmailsRepository();
    IJobTypeStepToUserRepository GetJobTypeStepToUserRepository();
    Guid ContextId { get; }
    void SaveAll();
    Task<int> SaveAllAsync(CancellationToken cancellationToken = default);
    void BeginTransaction();
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    void CommitTransaction();
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    void RollbackTransaction();
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    bool HasPendingTransaction();
}

public sealed class DataRepositoryFactory : IDisposable, IDataRepositoryFactory
{
    #region Private Members
    private TimeClockContext _dbContext;
    private TimeClockContext? _eventAuditContext = null;
    private DepartmentsRepository? _departmentsRepository = null;
    private DevicesRepository? _devicesRepository = null;
    private EquipmentsRepository? _equipmentsRepository = null;
    private EquipmentTypesRepository? _equipmentTypesRepository = null;
    private LocationsRepository? _locationsRepository = null;
    private PunchEntriesRepository? _punchEntriesRepository = null;
    private UsersRepository? _usersRepository = null;
    private EventAuditRepository? _eventAuditRepository = null;
    private UnregisteredDevicesRepository? _unregisteredDevicesRepository = null;
    private DepartmentsToLocationsRepository? _departmentsToLocationsRepository = null;
    private AuthorizationClaimsRepository? _authorizationClaimsRepository = null;
    private UserClaimsRepository? _userClaimsRepository = null;
    private PunchEntriesCurrentStateRepository? _punchEntriesCurrentStateRepository = null;
    private BarcodesRepository? _barcodesRepository = null;
    private WorkPeriodsRepository? _workPeriodsRepository = null;
    private JobTypesRepository? _jobTypesRepository = null;
    private JobStepsRepository? _jobStepsRepository = null;
    private SentEmailsRepository? _sentEmailsRepository = null;
    private JobTypeStepToUserRepository? _jobTypeStepToUserRepository = null;

    #endregion Private Members

    public DataRepositoryFactory(string connectionString)
    {
        this._dbContext = new TimeClockContext(connectionString);
    }

    public TimeClockContext Context => this._dbContext;

    #region Public Methods
    /// <summary>
    /// Gets an instance of the repository corresponding to the IEntityModel specified by T. 
    /// This instance is reused for each subsequent call for the lifetime of the instance of this DataRepositoryFactory.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public IDataRepository<T> GetRepository<T>() where T : class, IEntityModel
    {
        this.ThrowIfDisposed();
        var type = typeof(T);
        switch (type)
        {
            case Type _ when type == typeof(Department):
            case Type _ when type == typeof(DepartmentsToLocation):
                return (IDataRepository<T>)this.DepartmentsRepository;
            case Type _ when type == typeof(Device):
                return (IDataRepository<T>)this.DevicesRepository;
            case Type _ when type == typeof(Equipment):
            case Type _ when type == typeof(EquipmentsToDepartmentLocation):
            case Type _ when type == typeof(EquipmentsToUser):
                return (IDataRepository<T>)this.EquipmentsRepository;
            case Type _ when type == typeof(EquipmentType):
                return (IDataRepository<T>)this.EquipmentTypesRepository;
            case Type _ when type == typeof(Location):
                return (IDataRepository<T>)this.LocationsRepository;
            case Type _ when type == typeof(PunchEntry):
            case Type _ when type == typeof(PunchEntriesCurrentState):
            case Type _ when type == typeof(PunchEntriesHistory):
                return (IDataRepository<T>)this.PunchEntriesRepository;
            case Type _ when type == typeof(User):
                return (IDataRepository<T>)this.UserRepository;
            case Type _ when type == typeof(EventAudit):
                return (IDataRepository<T>)this.EventAuditRepository;
            case Type _ when type == typeof(UnregisteredDevice):
                return (IDataRepository<T>)this.UnregisteredDevicesRepository;
            case Type _ when type == typeof(DepartmentsToLocation):
                return (IDataRepository<T>)this.DepartmentsToLocationsRepository;
            case Type _ when type == typeof(AuthorizationClaim):
                return (IDataRepository<T>)this.AuthorizationClaimsRepository;
            case Type _ when type == typeof(UserClaim):
                return (IDataRepository<T>)this.UserClaimsRepository;
            case Type _ when type == typeof(Barcode):
                return (IDataRepository<T>)this.BarcodesRepository;
            case Type _ when type == typeof(WorkPeriod):
                return (IDataRepository<T>)this.WorkPeriodsRepository;
            case Type _ when type == typeof(JobType):
                return (IDataRepository<T>)this.JobTypesRepository;
            case Type _ when type == typeof(JobStep):
                return (IDataRepository<T>)this.JobStepsRepository;
            case Type _ when type == typeof(SentEmail):
                return (IDataRepository<T>)this.SentEmailsRepository;
            case Type _ when type == typeof(JobTypeStepToUser):
                return (IDataRepository<T>)this.JobTypeStepToUserRepository;
            default:
                return new DataRepository<T>(this._dbContext);
        }
    }

    public IDepartmentsRepository GetDepartmentsRepository()
    {
        this.ThrowIfDisposed();
        return this.DepartmentsRepository;
    }
    public IDevicesRepository GetDevicesRepository()
    {
        this.ThrowIfDisposed();
        return this.DevicesRepository;
    }
    public IEquipmentsRepository GetEquipmentsRepository()
    {
        this.ThrowIfDisposed();
        return this.EquipmentsRepository;
    }
    public IEquipmentTypesRepository GetEquipmentTypesRepository()
    {
        this.ThrowIfDisposed();
        return this.EquipmentTypesRepository;
    }
    public ILocationsRepository GetLocationsRepository()
    {
        this.ThrowIfDisposed();
        return this.LocationsRepository;
    }
    public IPunchEntriesRepository GetPunchEntriesRepository()
    {
        this.ThrowIfDisposed();
        return this.PunchEntriesRepository;
    }
    public IUsersRepository GetUsersRepository()
    {
        this.ThrowIfDisposed();
        return this.UserRepository;
    }
    public IEventAuditRepository GetEventAuditsRepository()
    {
        this.ThrowIfDisposed();
        return this.EventAuditRepository;
    }
    public IUnregisteredDevicesRepository GetUnregisteredDevicesRepository()
    {
        this.ThrowIfDisposed();
        return this.UnregisteredDevicesRepository;
    }
    public IDepartmentsToLocationsRepository GetDepartmentsToLocationsRepository()
    {
        this.ThrowIfDisposed();
        return this.DepartmentsToLocationsRepository;
    }
    public IAuthorizationClaimsRepository GetAuthorizationClaimsRepository()
    {
        this.ThrowIfDisposed();
        return this.AuthorizationClaimsRepository;
    }
    public IUserClaimsRepository GetUserClaimsRepository()
    {
        this.ThrowIfDisposed();
        return this.UserClaimsRepository;
    }
    public IBarcodesRepository GetBarcodesRepository()
    {
        this.ThrowIfDisposed();
        return this.BarcodesRepository;
    }
    public IWorkPeriodsRepository GetWorkPeriodsRepository()
    {
        this.ThrowIfDisposed();
        return this.WorkPeriodsRepository;
    }
    [Obsolete("Use PunchEntriesRepository instead")]
    /// <summary>
    /// Not used! Use PunchEntriesRepository instead
    /// </summary>
    /// <returns></returns>
    private IPunchEntriesCurrentStateRepository GetPunchEntriesCurrentStateRepository()
    {
        this.ThrowIfDisposed();
        return this.PunchEntriesCurrentStateRepository;
    }
    public IJobTypesRepository GetJobTypesRepository()
    {
        this.ThrowIfDisposed();
        return this.JobTypesRepository;
    }
    public IJobStepsRepository GetJobStepsRepository()
    {
        this.ThrowIfDisposed();
        return this.JobStepsRepository;
    }
    public ISentEmailsRepository GetSentEmailsRepository()
    {
        this.ThrowIfDisposed();
        return this.SentEmailsRepository;
    }
    public IJobTypeStepToUserRepository GetJobTypeStepToUserRepository()
    {
        this.ThrowIfDisposed();
        return this.JobTypeStepToUserRepository;
    }
    public Guid ContextId { get => this.Context.ContextId.InstanceId; }
    public void SaveAll()
    {
        this.ThrowIfDisposed();
        this.Context.SaveChanges();
    }
    public Task<int> SaveAllAsync(CancellationToken cancellationToken = default)
    {
        this.ThrowIfDisposed();
        return this.Context.SaveChangesAsync(cancellationToken);
    }
    public void BeginTransaction()
    {
        this.ThrowIfDisposed();
        this.Context.Database.BeginTransaction();
    }
    public Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        this.ThrowIfDisposed();
        return this.Context.Database.BeginTransactionAsync(cancellationToken);
    }
    public void CommitTransaction()
    {
        this.ThrowIfDisposed();
        this.Context.Database.CommitTransaction();
    }
    public Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        this.ThrowIfDisposed();
        return this.Context.Database.CommitTransactionAsync(cancellationToken);
    }
    public void RollbackTransaction()
    {
        this.ThrowIfDisposed();
        this.Context.Database.RollbackTransaction();
    }
    public Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        this.ThrowIfDisposed();
        return this.Context.Database.RollbackTransactionAsync(cancellationToken);
    }
    public bool HasPendingTransaction()
    {
        this.ThrowIfDisposed();
        return this.Context.Database.CurrentTransaction != null;
    }
    #endregion Public Methods


    #region Entity Repositories Private Properties
    private DepartmentsRepository DepartmentsRepository
    {
        get
        {
            if (this._departmentsRepository is null)
                this._departmentsRepository = new DepartmentsRepository(this._dbContext) { UseTracker = false };
            return this._departmentsRepository;
        }
        set { this._departmentsRepository = value; }
    }

    private DevicesRepository DevicesRepository
    {
        get
        {
            if (this._devicesRepository is null)
                this._devicesRepository = new DevicesRepository(this._dbContext) { UseTracker = false };
            return this._devicesRepository;
        }
        set { this._devicesRepository = value; }
    }

    private EquipmentsRepository EquipmentsRepository
    {
        get
        {
            if (this._equipmentsRepository is null)
                this._equipmentsRepository = new EquipmentsRepository(this._dbContext) { UseTracker = false };
            return this._equipmentsRepository;
        }
        set { this._equipmentsRepository = value; }
    }

    private EquipmentTypesRepository EquipmentTypesRepository
    {
        get
        {
            if (this._equipmentTypesRepository is null)
                this._equipmentTypesRepository = new EquipmentTypesRepository(this._dbContext) { UseTracker = false };
            return this._equipmentTypesRepository;
        }
        set { this._equipmentTypesRepository = value; }
    }

    private LocationsRepository LocationsRepository
    {
        get
        {
            if (this._locationsRepository is null)
                this._locationsRepository = new LocationsRepository(this._dbContext) { UseTracker = false };
            return this._locationsRepository;
        }
        set { this._locationsRepository = value; }
    }

    private PunchEntriesRepository PunchEntriesRepository
    {
        get
        {
            if (this._punchEntriesRepository is null)
                this._punchEntriesRepository = new PunchEntriesRepository(this._dbContext) { UseTracker = false };
            return this._punchEntriesRepository;
        }
        set { this._punchEntriesRepository = value; }
    }

    private UsersRepository UserRepository
    {
        get
        {
            if (this._usersRepository is null)
                this._usersRepository = new UsersRepository(this._dbContext) { UseTracker = false };
            return this._usersRepository;
        }
        set { this._usersRepository = value; }
    }
    private EventAuditRepository EventAuditRepository
    {
        get
        {
            if (this._eventAuditContext is null)
                this._eventAuditContext = new TimeClockContext(this._dbContext.Database.GetConnectionString() ?? string.Empty);
            if (this._eventAuditRepository is null)
                this._eventAuditRepository = new EventAuditRepository(this._eventAuditContext);
            return this._eventAuditRepository;
        }
        set { this._eventAuditRepository = value; }
    }
    private UnregisteredDevicesRepository UnregisteredDevicesRepository
    {
        get
        {
            if (this._unregisteredDevicesRepository is null)
                this._unregisteredDevicesRepository = new UnregisteredDevicesRepository(this._dbContext) { UseTracker = false };
            return this._unregisteredDevicesRepository;
        }
        set { this._unregisteredDevicesRepository = value; }
    }
    private DepartmentsToLocationsRepository DepartmentsToLocationsRepository
    {
        get
        {
            if (this._departmentsToLocationsRepository is null)
                this._departmentsToLocationsRepository = new DepartmentsToLocationsRepository(this._dbContext) { UseTracker = false };
            return this._departmentsToLocationsRepository;
        }
        set { this._departmentsToLocationsRepository = value; }
    }
    private AuthorizationClaimsRepository AuthorizationClaimsRepository
    {
        get
        {
            if (this._authorizationClaimsRepository is null)
                this._authorizationClaimsRepository = new AuthorizationClaimsRepository(this._dbContext) { UseTracker = false };
            return this._authorizationClaimsRepository;
        }
        set { this._authorizationClaimsRepository = value; }
    }
    private UserClaimsRepository UserClaimsRepository
    {
        get
        {
            if (this._userClaimsRepository is null)
                this._userClaimsRepository = new UserClaimsRepository(this._dbContext) { UseTracker = false };
            return this._userClaimsRepository;
        }
        set { this._userClaimsRepository = value; }
    }
    private PunchEntriesCurrentStateRepository PunchEntriesCurrentStateRepository
    {
        get
        {
            if (this._punchEntriesCurrentStateRepository is null)
                this._punchEntriesCurrentStateRepository = new PunchEntriesCurrentStateRepository(this._dbContext) { UseTracker = false };
            return this._punchEntriesCurrentStateRepository;
        }
        set { this._punchEntriesCurrentStateRepository = value; }
    }
    private BarcodesRepository BarcodesRepository
    {
        get
        {
            if (this._barcodesRepository is null)
                this._barcodesRepository = new BarcodesRepository(this._dbContext) { UseTracker = false };
            return this._barcodesRepository;
        }
        set { this._barcodesRepository = value; }
    }
    private WorkPeriodsRepository WorkPeriodsRepository
    {
        get
        {
            if (this._workPeriodsRepository is null)
                this._workPeriodsRepository = new WorkPeriodsRepository(this._dbContext) { UseTracker = false };
            return this._workPeriodsRepository;
        }
        set { this._workPeriodsRepository = value; }
    }
    private JobTypesRepository JobTypesRepository
    {
        get
        {
            if (this._jobTypesRepository is null)
                this._jobTypesRepository = new JobTypesRepository(this._dbContext) { UseTracker = false };
            return this._jobTypesRepository;
        }
        set { this._jobTypesRepository = value; }
    }
    private JobStepsRepository JobStepsRepository
    {
        get
        {
            if (this._jobStepsRepository is null)
                this._jobStepsRepository = new JobStepsRepository(this._dbContext) { UseTracker = false };
            return this._jobStepsRepository;
        }
        set { this._jobStepsRepository = value; }
    }
    private SentEmailsRepository SentEmailsRepository
    {
        get
        {
            if (this._sentEmailsRepository is null)
                this._sentEmailsRepository = new SentEmailsRepository(this._dbContext) { UseTracker = false };
            return this._sentEmailsRepository;
        }
        set { this._sentEmailsRepository = value; }
    }
    private JobTypeStepToUserRepository JobTypeStepToUserRepository
    {
        get
        {
            if (this._jobTypeStepToUserRepository is null)
                this._jobTypeStepToUserRepository = new JobTypeStepToUserRepository(this._dbContext) { UseTracker = false };
            return this._jobTypeStepToUserRepository;
        }
        set { this._jobTypeStepToUserRepository = value; }
    }
    #endregion Entity Repository Private Properties


    #region IDispose
    private void ThrowIfDisposed() => ObjectDisposedException.ThrowIf(this._disposed, typeof(DataRepositoryFactory));
    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }
    private bool _disposed = false;
    public void Dispose(bool disposing)
    {
        if (this._disposed)
            return;

        if (disposing)
        {
            if (this._dbContext != null) this._dbContext.Dispose();
        }
        this._disposed = true;
    }
    #endregion IDispose
}
