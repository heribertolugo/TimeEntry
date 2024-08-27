using Microsoft.EntityFrameworkCore;
using Serilog;
using TimeClock.Data;
using TimeClock.Data.Models;
using TimeClock.JdeSync.Helpers;
using Location = TimeClock.Data.Models.Location;

namespace TimeClock.JdeSync.Services;
internal sealed class TcEntityLoader : IDisposable
{
    private bool _disposedValue;
    private string ConnectionString { get; init; }
    private CancellationToken CancellationToken { get; init; }
    private ILogger Logger { get; init; }


    public TcEntityLoader(string connectionString, ILogger logger, CancellationToken cancellationToken)
    {
        this.ConnectionString = connectionString;
        this.CancellationToken = cancellationToken;
        this.Logger = logger;
        this.Context = new TimeClockContext(connectionString);
        this.Context.Database.SetCommandTimeout(TimeSpan.FromMinutes(2));
    }

    public async Task Load()
    {
        await this.CreateTcSystemUsers();
        await this.LoadTimeClockEntities();
    }

    public async Task<int> SaveChanges()
    {
        return await this.Context.SaveChangesAsync(true, this.CancellationToken);
    }

    #region Public Properties
    public TimeClockContext Context { get; private set; }
    public List<EquipmentType> EquipmentTypes { get; set; } = new();
    public List<Equipment> Equipment { get; set; } = new();
    public List<EquipmentsToDepartmentLocation> EquipmentsToDepartmentLocations { get; set; } = new();
    public List<Department> Departments { get; set; } = new();
    public List<Location> Locations { get; set; } = new();
    public List<DepartmentsToLocation> DepartmentsToLocations { get; set; } = new();
    public List<User> Users { get; set; } = new();
    public List<JobType> JobTypes { get; set; } = new();
    public List<JobStep> JobSteps { get; set; } = new();
    public IEnumerable<AuthorizationClaim> AuthorizationClaims { get; set; } = [];
    public User? SystemUser { get; private set; }
    public User? JdeSystemUser { get; private set; }

    public bool SyncUserJobTypeSteps { get; set; }
    #endregion Public Properties

    public async Task LoadEquipmentTypes()
    {
        await this.CreateTcSystemUsers();
        this.EquipmentTypes = await this.Context.EquipmentTypes.AsTracking().OrderBy(t => t.JdeId).ToListAsync();

        if (!this.EquipmentTypes.Any(q => q.Name == "Unknown"))
        {
            var newType = new EquipmentType()
            {
                IsActive = true,
                Id = Guid.NewGuid(),
                JdeId = CommonValues.NonJdeId,
                Name = "Unknown"
            };
            this.EquipmentTypes.Add(newType);
            this.Context.EquipmentTypes.Add(newType);
        }
    }
    public async Task LoadEquipment()
    {
        await this.CreateTcSystemUsers();
        this.Equipment = await this.Context.Equipments.AsTracking().Include(q => q.EquipmentType).OrderBy(q => q.JdeId).ToListAsync();
    }
    public async Task LoadEquipmentsToDepartmentLocations()
    {
        await this.CreateTcSystemUsers();
        this.EquipmentsToDepartmentLocations = await this.Context.EquipmentsToLocations.AsTracking().OrderBy(d => d.JdeId).ToListAsync();
    }
    public async Task LoadDepartments()
    {
        await this.CreateTcSystemUsers();
        this.Departments = await this.Context.Departments.AsTracking().OrderBy(d => d.JdeId).ToListAsync();
    }
    public async Task LoadLocations()
    {
        await this.CreateTcSystemUsers();
        this.Locations = await this.Context.Locations.AsTracking().OrderBy(l => l.JdeId).ToListAsync();
    }
    public async Task LoadDepartmentsToLocations()
    {
        await this.CreateTcSystemUsers();
        this.DepartmentsToLocations = await this.Context.DepartmentsToLocations.AsTracking().OrderBy(d => d.RowId).ToListAsync();
    }
    public async Task LoadUsers()
    {
        await this.CreateTcSystemUsers();
        var query = this.Context.Users.AsTracking()
            .OrderBy(d => d.JdeId)
            .Include(u => u.UserClaims)
            .Include(u => u.JobTypeSteps)
            .ThenInclude(j => j.JobType)
            .Include(u => u.JobTypeSteps)
            .ThenInclude(j => j.JobStep);

        this.Users = await query.ToListAsync();
    }
    public async Task LoadJobTypes()
    {
        await this.CreateTcSystemUsers();
        this.JobTypes = await this.Context.JobTypes.AsTracking().OrderBy(d => d.JdeId).ToListAsync();
    }
    public async Task LoadJobSteps()
    {
        await this.CreateTcSystemUsers();
        this.JobSteps = await this.Context.JobSteps.AsTracking().OrderBy(d => d.JdeId).ToListAsync();
    }

    private async Task<IEnumerable<User>> CreateTcSystemUsers()
    {
        if (this.SystemUser is not null)
        {
            return [this.SystemUser, this.JdeSystemUser!];
        }
        var systemUser = await this.Context.Users.FirstOrDefaultAsync(u => u.EmployeeNumber == "System001");
        var jdeSystemUser = await this.Context.Users.FirstOrDefaultAsync(u => u.EmployeeNumber == "System000");
        if (systemUser is not null)
        {
            this.SystemUser = systemUser;
            this.JdeSystemUser = jdeSystemUser;
            return [systemUser, jdeSystemUser!];
        }
        this.Logger.Information("Creating System Entities");
        var systemLocation = await this.Context.Locations.FirstOrDefaultAsync(d => d.Name == "System Location");
        var systemDepartment = await this.Context.Departments.FirstOrDefaultAsync(d => d.Name == "System Department");
        var systemDepartmentToLocation = await this.Context.DepartmentsToLocations.FirstOrDefaultAsync(d => d.Department.Name == "System Department" && d.Location.Name == "System Location");

        if (systemLocation is null)
        {
            systemLocation = new Location() { Name = "System Location", IsActive = false, JdeId = CommonValues.NonJdeId };
            this.Locations.Add(systemLocation);
            this.Context.Locations.Add(systemLocation);
        }
        if (systemDepartment is null)
        {
            systemDepartment = new Department() { Name = "System Department", IsActive = false, JdeId = CommonValues.NonJdeId };
            this.Departments.Add(systemDepartment);
            this.Context.Departments.Add(systemDepartment);
        }
        if (systemDepartmentToLocation is null)
        {
            systemDepartmentToLocation = new DepartmentsToLocation() { Department = systemDepartment, Location = systemLocation, IsActive = false };
            this.DepartmentsToLocations.Add(systemDepartmentToLocation);
            this.Context.DepartmentsToLocations.Add(systemDepartmentToLocation);
        }

        if (systemUser is null)
        {
            systemUser = new User()
            {
                DepartmentsToLocation = systemDepartmentToLocation,
                IsActive = false,
                LastActionOn = DateTime.Now,
                EmployeeNumber = "System001",
                FirstName = "System",
                LastName = "User",
                JdeId = -1,
                RowId = 0,
                UserName = "SystemUser"
            };
            this.Users.Add(systemUser);
            this.Context.Users.Add(systemUser);
        }

        if (jdeSystemUser is null)
        {
            jdeSystemUser = new User()
            {
                DepartmentsToLocation = systemDepartmentToLocation,
                IsActive = false,
                LastActionOn = DateTime.Now,
                EmployeeNumber = "System000",
                FirstName = "Jde",
                LastName = "Master User",
                JdeId = 0,
                RowId = 0,
                UserName = "JdeSystemUser"
            };
            this.Users.Add(jdeSystemUser);
            this.Context.Users.Add(jdeSystemUser);
        }

        this.SystemUser = systemUser;
        await this.Context.SaveChangesAsync();

        return [systemUser, jdeSystemUser];
    }

    /// <summary>
    /// Loads TimeClock entities, ordered by JdeId with Entity tracking.
    /// </summary>
    private async Task LoadTimeClockEntities()
    {
        this.Logger.Information(nameof(LoadTimeClockEntities));
        this.Logger.LogNewLine();
        this.EquipmentTypes = await this.Context.EquipmentTypes.AsTracking().OrderBy(t => t.JdeId).ToListAsync();
        this.Equipment = this.Context.Equipments.AsTracking().Include(q => q.EquipmentType).OrderBy(q => q.JdeId).AsSplitQuery().ToList();
        this.EquipmentsToDepartmentLocations = await this.Context.EquipmentsToLocations.AsTracking().OrderBy(d => d.JdeId).ToListAsync();
        this.Departments = await this.Context.Departments.AsTracking().OrderBy(d => d.JdeId).ToListAsync();
        this.Locations = await this.Context.Locations.AsTracking().OrderBy(l => l.JdeId).ToListAsync();
        this.DepartmentsToLocations = await this.Context.DepartmentsToLocations.AsTracking().OrderBy(d => d.RowId).ToListAsync();
        // need to uncomment UserClaims include when debugging if need to update UserClaims
        await this.LoadUsers();
        this.JobTypes = await this.Context.JobTypes.AsTracking().OrderBy(d => d.JdeId).ThenByDescending(d => d.IsActive).ToListAsync();
        this.JobSteps = await this.Context.JobSteps.AsTracking().OrderBy(d => d.JdeId).ThenByDescending(d => d.IsActive).ToListAsync();
        this.AuthorizationClaims = await this.Context.AuthorizationClaims.AsNoTracking().ToListAsync();
        

        if (!this.EquipmentTypes.Any(q => q.Name == "Unknown"))
        {
            var newType = new EquipmentType()
            {
                IsActive = true,
                Id = Guid.NewGuid(),
                JdeId = CommonValues.NonJdeId,
                Name = "Unknown"
            };
            this.EquipmentTypes.Add(newType);
            this.Context.EquipmentTypes.Add(newType);
        }
    }


    #region IDisposable
    private void Dispose(bool disposing)
    {
        if (!this._disposedValue)
        {
            if (disposing)
            {
                this.Context.Dispose();
            }

            this.Departments.Clear();
            this.DepartmentsToLocations.Clear();
            this.Equipment.Clear();
            this.EquipmentsToDepartmentLocations.Clear();
            this.EquipmentTypes.Clear();
            this.Locations.Clear();
            this.Users.Clear();
            this._disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    #endregion IDisposable
}
