using TimeClock.Api.Security;
using TimeClock.Data;
using TimeClock.Data.Models;
using TimeClock.Data.Repository;

namespace TimeClock.Api.Helpers;

public interface IDataFactoryInitializerParams
{
    string JdeOracleConnectionString { get; set; }
    string SqlConnectionString { get; set; }
}

internal sealed class DataFactoryInitializerParams : IDataFactoryInitializerParams
{
    public DataFactoryInitializerParams(SecretsProvider secrets)
    {
        this.SqlConnectionString = secrets.GetTimeClockConnectionString() ?? string.Empty;
        this.JdeOracleConnectionString = secrets.GetJdeConnectionString() ?? string.Empty;
    }

    public string SqlConnectionString { get; set; }
    public string JdeOracleConnectionString { get; set; }
}

public sealed class DataFactoryInitializer : IDataRepositoryFactory, IDisposable
{
    private readonly IDataFactoryInitializerParams _params;
    private readonly DataRepositoryFactory _dataServiceFactory;
    private bool _disposedValue;

    public DataFactoryInitializer(IDataFactoryInitializerParams initializerParams)
    {
        this._params = initializerParams;
        this._dataServiceFactory = new DataRepositoryFactory(this._params.SqlConnectionString);
        this.Context = this._dataServiceFactory.Context;
    }

    public TimeClockContext Context { get; }
    public Guid ContextId { get => this._dataServiceFactory.ContextId; }

    public IPunchEntriesRepository GetPunchEntriesRepository()
    {
        return this._dataServiceFactory.GetPunchEntriesRepository();
    }

    public IDataRepository<T> GetRepository<T>() where T : class, IEntityModel
    {
        return this._dataServiceFactory.GetRepository<T>();
    }

    public IUsersRepository GetUsersRepository()
    {
        return this._dataServiceFactory.GetUsersRepository();
    }

    public IDepartmentsRepository GetDepartmentsRepository()
    {
        return this._dataServiceFactory.GetDepartmentsRepository();
    }

    public IDevicesRepository GetDevicesRepository()
    {
        return this._dataServiceFactory.GetDevicesRepository();
    }

    public IEquipmentsRepository GetEquipmentsRepository()
    {
        return this._dataServiceFactory.GetEquipmentsRepository();
    }

    public IEquipmentTypesRepository GetEquipmentTypesRepository()
    {
        return this._dataServiceFactory.GetEquipmentTypesRepository();
    }

    public ILocationsRepository GetLocationsRepository()
    {
        return this._dataServiceFactory.GetLocationsRepository();
    }

    public IEventAuditRepository GetEventAuditsRepository()
    {
        return this._dataServiceFactory.GetEventAuditsRepository();
    }

    public IUnregisteredDevicesRepository GetUnregisteredDevicesRepository()
    {
        return this._dataServiceFactory.GetUnregisteredDevicesRepository();
    }

    public IDepartmentsToLocationsRepository GetDepartmentsToLocationsRepository()
    {
        return this._dataServiceFactory.GetDepartmentsToLocationsRepository();
    }

    public IAuthorizationClaimsRepository GetAuthorizationClaimsRepository()
    {
        return this._dataServiceFactory.GetAuthorizationClaimsRepository();
    }

    public IUserClaimsRepository GetUserClaimsRepository()
    {
        return this._dataServiceFactory.GetUserClaimsRepository();
    }

    public IBarcodesRepository GetBarcodesRepository()
    {
        return this._dataServiceFactory.GetBarcodesRepository();
    }
    
    public IWorkPeriodsRepository GetWorkPeriodsRepository()
    {
        return this._dataServiceFactory.GetWorkPeriodsRepository();
    }

    public IJobTypesRepository GetJobTypesRepository()
    {
        return this._dataServiceFactory.GetJobTypesRepository();
    }

    public IJobStepsRepository GetJobStepsRepository()
    {
        return this._dataServiceFactory.GetJobStepsRepository();
    }

    public ISentEmailsRepository GetSentEmailsRepository()
    {
        return (ISentEmailsRepository)this._dataServiceFactory.GetRepository<SentEmail>();
    }

    public IJobTypeStepToUserRepository GetJobTypeStepToUserRepository()
    {
        return (IJobTypeStepToUserRepository)this._dataServiceFactory.GetRepository<JobTypeStepToUser>();
    }

    public void SaveAll()
    {
        this._dataServiceFactory.SaveAll();
    }
    public Task<int> SaveAllAsync(CancellationToken cancellationToken = default)
    {
        return this._dataServiceFactory.SaveAllAsync(cancellationToken);
    }
    public void BeginTransaction()
    {
        this._dataServiceFactory.BeginTransaction();
    }
    public Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return this._dataServiceFactory.BeginTransactionAsync(cancellationToken);
    }
    public void CommitTransaction()
    {
        this._dataServiceFactory.CommitTransaction();
    }
    public Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        return this._dataServiceFactory.CommitTransactionAsync(cancellationToken);
    }
    public void RollbackTransaction()
    {
        this._dataServiceFactory.RollbackTransaction();
    }
    public Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        return this._dataServiceFactory.RollbackTransactionAsync(cancellationToken);
    }
    public bool HasPendingTransaction()
    {
        return this._dataServiceFactory.HasPendingTransaction();
    }

    public void Dispose(bool disposing)
    {
        if (!this._disposedValue)
        {
            if (disposing)
            {
                this._dataServiceFactory.Dispose();
            }
            this._disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
