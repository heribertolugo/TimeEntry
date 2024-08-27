using LinqKit;
using Serilog;
using System.Collections.Frozen;
using TimeClock.Data.Models;
using TimeClock.Data.Models.Jde;
using TimeClock.Data.Repository;
using TimeClock.JdeSync.Helpers;
using Location = TimeClock.Data.Models.Location;

namespace TimeClock.JdeSync.Services;
internal class JdeToTcSync : IDisposable
{
    private bool _disposedValue;

    private ILogger Logger { get; set; }
    private JdeEntityLoader JdeLoader { get; set; }
    private TcEntityLoader TcLoader {  get; set; }
    private CancellationToken CancellationToken { get; set; }
    private CancellationTokenSource CancellationTokenSource { get; set; }
    /// <summary>
    /// These values correlate to CTL.F0005 table where drsy = '06' and drrt = 'PS'. 
    /// Only values which denote the employee is still with the company are included here, so set employee active status.
    /// The only reason for a dictionary instead of an array is to include the description for readability.
    /// </summary>

    #region Mappings
    private static Dictionary<string, EquipmentType> JdeEquipmentTypeIdToTcEquipmentType { get; set; } = new();
    private static Dictionary<string, JobType> JdeJobTypeCodeToTcJobType { get; set; } = new();
    private static Dictionary<string, JobStep> JdeJobStepCodeToTcJobStep { get; set; } = new();
    private static Dictionary<string, JobType> JdeJobTypeCodeToTcJobTypeTrimd { get; set; } = new();
    private static Dictionary<string, JobStep> JdeJobStepCodeToTcJobStepTrimd { get; set; } = new();
    private static Dictionary<string, DepartmentsToLocation> DepartmentToLocationByJdeId { get; set; } = new();
    #endregion Mappings

    public JdeToTcSync(string tcConnectionString, string jdeConnectionString, ILogger logger, CancellationTokenSource cancellationTokenSource)
    {
        this.Logger = logger;
        this.CancellationTokenSource = cancellationTokenSource;
        this.CancellationToken = cancellationTokenSource.Token;
        this.JdeLoader = new(jdeConnectionString, logger, this.CancellationToken);
        this.TcLoader = new(tcConnectionString, logger, this.CancellationToken);
    }

    public void SetActiveEmployeeStatusCodes(char[] statuses)
    {
        JdeEntityLoader.ActiveEmployeeStatuses = statuses.ToFrozenSet();
    }
    public bool FilterEmployeesToActive { get => this.JdeLoader.FilterEmployeesToActive; set => this.JdeLoader.FilterEmployeesToActive = value; }
    public bool SyncUserJobTypeSteps { get => this.TcLoader.SyncUserJobTypeSteps; set => this.TcLoader.SyncUserJobTypeSteps = value; }

    public async Task Start(bool bulkSync = true)
    {
        if (bulkSync)
            await this.BulkSync();
        else
            await this.ItemizedSync();

        await this.Finalize();
    }

    private async Task BulkSync()
    {
        await Task.WhenAll(this.JdeLoader.Load(), this.TcLoader.Load());

        JdeEquipmentTypeIdToTcEquipmentType.Add(CommonValues.NonJdeId, this.TcLoader.EquipmentTypes.First(t => t.JdeId == CommonValues.NonJdeId));

        // there is room for improvement in speed here. the sync operations can be grouped into asynchronous operations.
        // however, a new TcLoader will need to be created for each task. do not call load, just populate the needed List<Entity> corresponding to the task
        // the reason for creating a new TcLoader is because a new instance of DbContext is required to run parallel database operations
        // we can safely run parallel operations if we order our relationship dependencies
        // not implemented because current process time is not bad.

        // the following 3 can be ran in parallel, each with its own instance of TcLoader.
        this.SyncEquipmentTypes(this.TcLoader, this.JdeLoader); // this TcLoader will need List<EquipmentType> set
        this.SyncDepartments(this.TcLoader, this.JdeLoader); // this TcLoader will need List<Department> set
        this.SyncLocations(this.TcLoader, this.JdeLoader); // this TcLoader will need List<Location> set

        // after the all previous complete (Task.WhenAll), these need to be ran consecutively (as they are now, in order).
        // create a new TcLoader and populate the List from above, then use TcLoader.LoadDepartmentToLocations and TcLoader.LoadEquipmentWithDepartmentLocations
        // we will need the data that is about to be accessed to be loaded before we try and sync
        this.SyncDepartmentToLocations(this.TcLoader, this.JdeLoader);
        this.SyncEquipmentWithDepartmentLocations(this.TcLoader, this.JdeLoader);

        // the following 2 can be ran in parallel using the same idea as the first 3. Each own TcLoader
        this.SyncJobTypes(this.TcLoader, this.JdeLoader); // this TcLoader will need List<JobType> set
        this.SyncJobSteps(this.TcLoader, this.JdeLoader); // this TcLoader will need List<JobStep> set

        // this needs to run after all previous finish, and have access to List of DepartmentToLocations, JobTypes and JobSteps
        this.SyncUsers(this.TcLoader, this.JdeLoader);
    }

    private async Task ItemizedSync()
    {
        await this.ProcessEquipmentTypes();
        await this.ProcessDepartments();
        await this.ProcessLocations();
        await this.TcLoader.LoadDepartmentsToLocations();
        await this.ProcessEquipmentWithDepartmentLocations();
        this.TcLoader.Equipment.Clear();
        (this.JdeLoader.Equipment as List<JdeEquipment>).Clear();
        this.TcLoader.Locations.Clear();
        this.TcLoader.Departments.Clear();
        (this.JdeLoader.Locations as List<JdeLocation>).Clear();
        this.TcLoader.EquipmentTypes.Clear();
        (this.JdeLoader.EquipmentTypes as List<JdeCustomCode>).Clear();
        this.TcLoader.EquipmentsToDepartmentLocations.Clear();
        (this.JdeLoader.EquipmentToLocations as List<JdeEquipmentLocation>).Clear();

        await this.ProcessJobTypes();
        await this.ProcessJobSteps();
        await this.ProcessUsers();
    }

    private async Task ProcessEquipmentTypes()
    {
        await this.JdeLoader.LoadJdeEquipmentTypes();
        await this.TcLoader.LoadEquipmentTypes();

        JdeEquipmentTypeIdToTcEquipmentType.Add(CommonValues.NonJdeId, this.TcLoader.EquipmentTypes.First(t => t.JdeId == CommonValues.NonJdeId));

        this.SyncEquipmentTypes(this.TcLoader, this.JdeLoader);
    }
    private async Task ProcessDepartments()
    {
        if (!this.JdeLoader.Locations.Any())
            await this.JdeLoader.LoadJdeLocations();
        await this.TcLoader.LoadDepartments();
        this.SyncDepartments(this.TcLoader, this.JdeLoader);
    }
    private async Task ProcessLocations()
    {
        if (!this.JdeLoader.Locations.Any())
            await this.JdeLoader.LoadJdeLocations();
        await this.TcLoader.LoadLocations();
        this.SyncLocations(this.TcLoader, this.JdeLoader);
    }
    private async Task ProcessEquipmentWithDepartmentLocations()
    {
        await this.JdeLoader.LoadJdeEquipmentLocations();
        await this.TcLoader.LoadEquipment();
        await this.TcLoader.LoadEquipmentsToDepartmentLocations();
        this.SyncDepartmentToLocations(this.TcLoader, this.JdeLoader);
        this.SyncEquipmentWithDepartmentLocations(this.TcLoader, this.JdeLoader);
    }
    private async Task ProcessJobTypes()
    {
        await this.JdeLoader.LoadJdeJobTypes();
        await this.TcLoader.LoadJobTypes();
        this.SyncJobTypes(this.TcLoader, this.JdeLoader);
    }
    private async Task ProcessJobSteps()
    {
        await this.JdeLoader.LoadJdeJobSteps();
        await this.TcLoader.LoadJobSteps();
        this.SyncJobSteps(this.TcLoader, this.JdeLoader);
    }
    private async Task ProcessUsers()
    {
        await this.JdeLoader.LoadJdeEmployees();
        await this.TcLoader.LoadUsers();
        this.SyncUsers(this.TcLoader, this.JdeLoader);
    }

    private async Task Finalize()
    {
        this.Logger.Information("saving to db");
        this.Logger.LogChangeSummaries(this.TcLoader);
        this.Logger.Warning("{changes} changes saved", await this.TcLoader.SaveChanges());
        this.TcLoader.Dispose();
        this.JdeLoader.Dispose();

        this.Logger.Information("done sync");
    }

    #region Sync JDE to TimeClock
    private void SyncEquipmentTypes(TcEntityLoader tcLoader, JdeEntityLoader jdeLoader)
    {
        List<EquipmentType> newItems = [];
        Syncer<string> syncer = new();

        syncer.OnNeedsNew += (s, e) =>
        {
            if (this.CancellationToken.IsCancellationRequested) return;
            JdeCustomCode? jde = null;
            try
            {
                JdeCustomCode jdeEntity = jde = (JdeCustomCode)e.JdeEntity!;
                string jdeId = jdeEntity.Code;
                jdeEntity.Description = jdeEntity.Description?.Trim() ?? "Unknown";
                this.Logger.LogNewItem<EquipmentType>(jdeId);
                Guid guid = Guid.NewGuid();
                var newEquipmentType = new EquipmentType()
                {
                    Id = guid,
                    IsActive = true,
                    JdeId = jdeId,
                    Name = jdeEntity.Description
                };
                tcLoader.Context.EquipmentTypes.Add(newEquipmentType);
                JdeEquipmentTypeIdToTcEquipmentType.TryAdd(jdeId.Trim(), newEquipmentType);
                newItems.Add(newEquipmentType);
            }
            catch (Exception ex)
            {
                this.Logger.LogSyncError(ex, jde, nameof(syncer.OnNeedsNew), nameof(SyncEquipmentTypes));
                if (!this.CancellationToken.IsCancellationRequested) this.CancellationTokenSource.Cancel();
            }
        };

        syncer.OnNeedsUpdate += (s, e) =>
        {
            if (this.CancellationToken.IsCancellationRequested) return;
            JdeCustomCode? jde = null;
            try
            {
                JdeCustomCode jdeEntity = jde = (JdeCustomCode)e.JdeEntity!;
                EquipmentType tcEntity = (EquipmentType)e.TimeClockEntity!;
                string jdeId = jdeEntity.Code;
                jdeEntity.Description = jdeEntity.Description?.Trim() ?? "Unknown";
                tcEntity.Name = jdeEntity.Description;
                this.Logger.LogUpdateItem<EquipmentType>(jdeId, tcEntity.Id);
                JdeEquipmentTypeIdToTcEquipmentType.TryAdd(jdeId.Trim(), tcEntity);
            }
            catch (Exception ex)
            {
                this.Logger.LogSyncError(ex, jde, nameof(syncer.OnNeedsUpdate), nameof(SyncEquipmentTypes));
                if (!this.CancellationToken.IsCancellationRequested) this.CancellationTokenSource.Cancel();
            }
        };

        syncer.OnDelete += (s, e) =>
        {
            if (this.CancellationToken.IsCancellationRequested) return;
            JdeCustomCode? jde = null;
            try
            {
                JdeCustomCode? jdeEntity = jde = (JdeCustomCode?)e.JdeEntity;
                EquipmentType tcEntity = (EquipmentType)e.TimeClockEntity!;
                string jdeId = (jdeEntity?.Code ?? tcEntity.JdeId ?? string.Empty);
                this.Logger.LogDeleteItem<EquipmentType>(jdeId, tcEntity.Id);
                tcEntity.IsActive = false;
                JdeEquipmentTypeIdToTcEquipmentType.TryAdd(jdeId.Trim(), tcEntity);
            }
            catch (Exception ex)
            {
                this.Logger.LogSyncError(ex, jde, nameof(syncer.OnDelete), nameof(SyncEquipmentTypes));
                if (!this.CancellationToken.IsCancellationRequested) this.CancellationTokenSource.Cancel();
            }
        };

        syncer.OnFinished += (s, e) =>
        {
            if (this.CancellationToken.IsCancellationRequested) return;
            try
            {
                tcLoader.EquipmentTypes.AddRange(newItems.OrderBy(t => t.JdeId));

                this.Logger.LogFinished<EquipmentType>(tcLoader);
                this.Logger.LogNewLine();
            }
            catch (Exception ex)
            {
                this.Logger.LogSyncError(ex, null, nameof(syncer.OnFinished), nameof(SyncEquipmentTypes));
                if (!this.CancellationToken.IsCancellationRequested) this.CancellationTokenSource.Cancel();
            }
        };

        try
        {
            if (this.CancellationToken.IsCancellationRequested) return;
            syncer.SyncJdeToTc(jdeLoader.EquipmentTypes, (j) => ((JdeCustomCode)j).Code, tcLoader.EquipmentTypes.Cast<IReferenceJde>());
        }
        catch (Exception ex)
        {
            this.Logger.LogSyncError(ex, null, nameof(syncer.SyncJdeToTc), nameof(SyncEquipmentTypes));
            if (!this.CancellationToken.IsCancellationRequested) this.CancellationTokenSource.Cancel();
        }
    }

    /// <summary>
    /// Syncs Equipment and EquipmentToDepartmentLocations
    /// </summary>
    private void SyncEquipmentWithDepartmentLocations(TcEntityLoader tcLoader, JdeEntityLoader jdeLoader)
    {
        List<Equipment> newItems = [];
        Syncer<int> syncer = new();
        string jdeEquipmentTypeId = string.Empty;
        JdeEquipmentLocation? jdeEquipmentLocation = null;

        syncer.OnBeforeCompare += (s, e) =>
        {
            if (this.CancellationToken.IsCancellationRequested) return;
            try
            {
                JdeEquipment jdeEntity = (JdeEquipment)e.JdeEntity!;
                jdeEquipmentLocation = jdeEntity.EquipmentLocations.FirstOrDefault();//l => l.CurrentState == 'C'
                jdeEquipmentTypeId = string.IsNullOrWhiteSpace(jdeEntity?.FK) ? CommonValues.NonJdeId : jdeEntity.FK;
            }
            catch (Exception ex)
            {
                this.Logger.LogSyncError(ex, null, nameof(syncer.OnBeforeCompare), nameof(SyncEquipmentWithDepartmentLocations));
                if (!this.CancellationToken.IsCancellationRequested) this.CancellationTokenSource.Cancel();
            }
        };

        syncer.OnNeedsNew += (s, e) =>
        {
            if (this.CancellationToken.IsCancellationRequested) return;
            JdeEquipment? jde = null;
            try
            {
                JdeEquipment jdeEntity = jde = (JdeEquipment)e.JdeEntity!;
                int jdeId = jdeEntity.Id;

                this.Logger.LogNewItem<Equipment>(jdeId);
                var newTcEquipment = new Equipment()
                {
                    IsActive = true,
                    JdeId = jdeId,
                    Description = jdeEntity?.Description3?.Trim(),
                    Sku = jdeEntity?.EquipmentNumber.Trim() ?? "Unknown",
                    Name = jdeEntity?.Description1.Trim() ?? "Unknown",
                    EquipmentType = JdeEquipmentTypeIdToTcEquipmentType[jdeEquipmentTypeId.Trim()]
                };
                tcLoader.Context.Equipments.Add(newTcEquipment);
                newItems.Add(newTcEquipment);

                if (jdeEquipmentLocation != null)
                {
                    var newItem = new EquipmentsToDepartmentLocation()
                    {
                        IsActive = true,
                        Equipment = newTcEquipment,
                        JdeId = jdeEquipmentLocation.Id,
                        // important: no need to check t.Location.JdeId atm, since locations and departments are the same
                        DepartmentsToLocation = JdeToTcSync.DepartmentToLocationByJdeId[jdeEquipmentLocation.Location.BusinessUnit],// tcLoader.DepartmentsToLocations.First(t => t.Department.JdeId == jdeEquipmentLocation.Location.BusinessUnit),
                        LinkedBy = tcLoader.SystemUser,
                        LinkedOn = DateTime.Now
                    };
                    tcLoader.Context.EquipmentsToLocations.Add(newItem);
                    tcLoader.EquipmentsToDepartmentLocations.Add(newItem);
                }
            }
            catch (Exception ex)
            {
                this.Logger.LogSyncError(ex, jde, nameof(syncer.OnNeedsNew), nameof(SyncEquipmentWithDepartmentLocations));
                if (!this.CancellationToken.IsCancellationRequested) this.CancellationTokenSource.Cancel();
            }
        };

        syncer.OnNeedsUpdate += (s, e) =>
        {
            if (this.CancellationToken.IsCancellationRequested) return;
            JdeEquipment? jde = null;
            try
            {
                JdeEquipment jdeEntity = jde = (JdeEquipment)e.JdeEntity!;
                Equipment tcEntity = (Equipment)e.TimeClockEntity!;
                int jdeId = jdeEntity.Id;
                tcEntity.Description = jdeEntity.Description3.Trim();
                tcEntity.Sku = jdeEntity.EquipmentNumber.Trim();
                tcEntity.Name = jdeEntity.Description1.Trim();
                this.Logger.LogUpdateItem<Equipment>(jdeId, tcEntity.Id);
                tcEntity.EquipmentType = JdeEquipmentTypeIdToTcEquipmentType[jdeEquipmentTypeId.Trim()];

                if (jdeEquipmentLocation != null)
                {
                    string jdeLocationId = jdeEquipmentLocation.Location.BusinessUnit;
                    // this equipment is not at the correct department location. create a new departmentLocation and deactivate and previous for this equipment
                    if (!tcLoader.EquipmentsToDepartmentLocations.Any
                    (t => t.EquipmentId == tcEntity.Id && t.IsActive == true && t.DepartmentsToLocation.Department.JdeId == jdeLocationId))
                    {
                        tcLoader.EquipmentsToDepartmentLocations.Where(t => t.EquipmentId == tcEntity.Id).ForEach(t => t.IsActive = false);
                        var newItem = new EquipmentsToDepartmentLocation()
                        {
                            IsActive = true,
                            Equipment = tcEntity,
                            JdeId = jdeEquipmentLocation.Id,
                            // important: no need to check t.Location.JdeId atm, since locations and departments are the same
                            DepartmentsToLocation = tcLoader.DepartmentsToLocations.First(t => t.Department.JdeId == jdeLocationId),
                            LinkedBy = tcLoader.SystemUser,
                            LinkedOn = DateTime.Now
                        };
                        tcLoader.Context.EquipmentsToLocations.Add(newItem);
                        tcLoader.EquipmentsToDepartmentLocations.Add(newItem);
                    }
                }
            }
            catch (Exception ex)
            {
                this.Logger.LogSyncError(ex, jde, nameof(syncer.OnNeedsUpdate), nameof(SyncEquipmentWithDepartmentLocations));
                if (!this.CancellationToken.IsCancellationRequested) this.CancellationTokenSource.Cancel();
            }
        };

        syncer.OnDelete += (s, e) =>
        {
            if (this.CancellationToken.IsCancellationRequested) return;
            JdeEquipment? jde = null;
            try
            {
                JdeEquipment? jdeEntity = (JdeEquipment?)e.JdeEntity;
                Equipment tcEntity = (Equipment)e.TimeClockEntity!;
                int jdeId = jdeEntity?.Id ?? tcEntity.JdeId ?? 0;
                this.Logger.LogDeleteItem<Equipment>(jdeId, tcEntity.Id);
                tcEntity.IsActive = false;
                tcLoader.EquipmentsToDepartmentLocations.Where(t => t.EquipmentId == tcEntity.Id).ForEach(t => t.IsActive = false);
            }
            catch (Exception ex)
            {
                this.Logger.LogSyncError(ex, jde, nameof(syncer.OnDelete), nameof(SyncEquipmentWithDepartmentLocations));
                if (!this.CancellationToken.IsCancellationRequested) this.CancellationTokenSource.Cancel();
            }
        };

        syncer.OnFinished += (s, e) =>
        {
            if (this.CancellationToken.IsCancellationRequested) return;
            tcLoader.Equipment.AddRange(newItems.OrderBy(q => q.JdeId));

            this.Logger.LogFinished<Equipment>(tcLoader);
            this.Logger.LogNewLine();
        };

        try
        {
            if (this.CancellationToken.IsCancellationRequested) return;
            syncer.SyncJdeToTc(jdeLoader.Equipment, (j) => ((JdeEquipment)j).Id, tcLoader.Equipment.Cast<IReferenceJde>());
        }
        catch (Exception ex)
        {
            this.Logger.LogSyncError(ex, null, nameof(syncer.SyncJdeToTc), nameof(SyncEquipmentWithDepartmentLocations));
            if (!this.CancellationToken.IsCancellationRequested) this.CancellationTokenSource.Cancel();
        }
    }

    private void SyncDepartments(TcEntityLoader tcLoader, JdeEntityLoader jdeLoader)
    {
        List<Department> newItems = [];
        Syncer<string> syncer = new();

        syncer.OnNeedsNew += (s, e) =>
        {
            if (this.CancellationToken.IsCancellationRequested) return;
            JdeLocation? jde = null;
            try
            {
                JdeLocation jdeEntity = jde = (JdeLocation)e.JdeEntity!;
                string jdeId = jdeEntity.BusinessUnit;

                this.Logger.LogNewItem<Department>(jdeId);
                var newItem = new Department()
                {
                    IsActive = true,
                    JdeId = jdeId,
                    Name = $"{(jdeEntity?.Description.Trim() ?? "Unknown")} ({jdeId.Trim()})"
                };
                tcLoader.Context.Departments.Add(newItem);
                newItems.Add(newItem);
            }
            catch (Exception ex)
            {
                this.Logger.LogSyncError(ex, jde, nameof(syncer.OnNeedsNew), nameof(SyncDepartments));
                if (!this.CancellationToken.IsCancellationRequested) this.CancellationTokenSource.Cancel();
            }
        };

        syncer.OnNeedsUpdate += (s, e) =>
        {
            if (this.CancellationToken.IsCancellationRequested) return;
            JdeLocation? jde = null;
            try
            {
                JdeLocation jdeEntity = jde = (JdeLocation)e.JdeEntity!;
                Department tcEntity = (Department)e.TimeClockEntity!;
                string jdeId = jdeEntity.BusinessUnit;

                this.Logger.LogUpdateItem<Department>(jdeId, tcEntity.Id);
                tcEntity.Name = $"{(jdeEntity?.Description.Trim() ?? "Unknown")} ({jdeId.Trim()})";
                tcEntity.IsActive = true;
            }
            catch (Exception ex)
            {
                this.Logger.LogSyncError(ex, jde, nameof(syncer.OnNeedsUpdate), nameof(SyncDepartments));
                if (!this.CancellationToken.IsCancellationRequested) this.CancellationTokenSource.Cancel();
            }
        };

        syncer.OnDelete += (s, e) =>
        {
            if (this.CancellationToken.IsCancellationRequested) return;
            JdeLocation? jde = null;
            try
            {
                JdeLocation? jdeEntity = jde = (JdeLocation?)e.JdeEntity;
                Department tcEntity = (Department)e.TimeClockEntity!;
                string jdeId = jdeEntity?.BusinessUnit ?? tcEntity.JdeId ?? string.Empty;

                this.Logger.LogDeleteItem<Department>(jdeId, tcEntity.Id);
                tcEntity.IsActive = false;
            }
            catch (Exception ex)
            {
                this.Logger.LogSyncError(ex, jde, nameof(syncer.OnDelete), nameof(SyncDepartments));
                if (!this.CancellationToken.IsCancellationRequested) this.CancellationTokenSource.Cancel();
            }
        };

        syncer.OnFinished += (s, e) =>
        {
            if (this.CancellationToken.IsCancellationRequested) return;
            tcLoader.Departments.AddRange(newItems.OrderBy(d => d.JdeId));

            this.Logger.LogFinished<Department>(tcLoader);
            this.Logger.LogNewLine();
        };

        try
        {
            if (this.CancellationToken.IsCancellationRequested) return;
            syncer.SyncJdeToTc(jdeLoader.Locations, (j) => ((JdeLocation)j).BusinessUnit, tcLoader.Departments);
        }
        catch (Exception ex)
        {
            this.Logger.LogSyncError(ex, null, nameof(syncer.SyncJdeToTc), nameof(SyncDepartments));
            if (!this.CancellationToken.IsCancellationRequested) this.CancellationTokenSource.Cancel();
        }
    }

    private void SyncLocations(TcEntityLoader tcLoader, JdeEntityLoader jdeLoader)
    {
        List<Location> newItems = [];
        Syncer<string> syncer = new();

        syncer.OnNeedsNew += (s, e) =>
        {
            if (this.CancellationToken.IsCancellationRequested) return;
            JdeLocation? jde = null;
            try
            {
                JdeLocation jdeEntity = jde = (JdeLocation)e.JdeEntity!;
                string jdeId = jdeEntity.BusinessUnit;

                this.Logger.LogNewItem<Location>(jdeId);
                var newItem = new Location()
                {
                    IsActive = true,
                    JdeId = jdeId,
                    Name = $"{(jdeEntity?.Description?.Trim() ?? "Unknown")} ({jdeId.Trim()})",
                    DivisionCode = jdeEntity?.Division
                };
                tcLoader.Context.Locations.Add(newItem);
                newItems.Add(newItem);
            }
            catch (Exception ex)
            {
                this.Logger.LogSyncError(ex, jde, nameof(syncer.OnNeedsNew), nameof(SyncLocations));
                if (!this.CancellationToken.IsCancellationRequested) this.CancellationTokenSource.Cancel();
            }
        };

        syncer.OnNeedsUpdate += (s, e) =>
        {
            if (this.CancellationToken.IsCancellationRequested) return;
            JdeLocation? jde = null;
            try
            {
                JdeLocation jdeEntity = jde = (JdeLocation)e.JdeEntity!;
                Location tcEntity = (Location)e.TimeClockEntity!;
                string jdeId = jdeEntity.BusinessUnit;

                this.Logger.LogUpdateItem<Location>(jdeId, tcEntity.Id);
                tcEntity.Name = $"{(jdeEntity?.Description?.Trim())} ({jdeId.Trim()})";
                tcEntity.IsActive = true;
                tcEntity.DivisionCode = jdeEntity?.Division;
            }
            catch (Exception ex)
            {
                this.Logger.LogSyncError(ex, jde, nameof(syncer.OnNeedsUpdate), nameof(SyncLocations));
                if (!this.CancellationToken.IsCancellationRequested) this.CancellationTokenSource.Cancel();
            }
        };

        syncer.OnDelete += (s, e) =>
        {
            if (this.CancellationToken.IsCancellationRequested) return;
            JdeLocation? jde = null;
            try
            {
                JdeLocation? jdeEntity = jde = (JdeLocation?)e.JdeEntity;
                Location tcEntity = (Location)e.TimeClockEntity!;
                string jdeId = jdeEntity?.BusinessUnit ?? tcEntity.JdeId ?? string.Empty;

                this.Logger.LogDeleteItem<Location>(jdeId, tcEntity.Id);
                tcEntity.IsActive = false;
            }
            catch (Exception ex)
            {
                this.Logger.LogSyncError(ex, jde, nameof(syncer.OnDelete), nameof(SyncLocations));
                if (!this.CancellationToken.IsCancellationRequested) this.CancellationTokenSource.Cancel();
            }
        };

        syncer.OnFinished += (s, e) =>
        {
            if (this.CancellationToken.IsCancellationRequested) return;
            tcLoader.Locations.AddRange(newItems.OrderBy(d => d.JdeId));

            this.Logger.LogFinished<Location>(tcLoader);
            this.Logger.LogNewLine();
        };

        try
        {
            if (this.CancellationToken.IsCancellationRequested) return;
            syncer.SyncJdeToTc(jdeLoader.Locations, (j) => ((JdeLocation)j).BusinessUnit, tcLoader.Locations);
        }
        catch (Exception ex)
        {
            this.Logger.LogSyncError(ex, null, nameof(syncer.SyncJdeToTc), nameof(SyncLocations));
            if (!this.CancellationToken.IsCancellationRequested) this.CancellationTokenSource.Cancel();
        }
    }

    private void SyncDepartmentToLocations(TcEntityLoader tcLoader, JdeEntityLoader jdeLoader)
    {
        this.Logger.Information("{syncProcess}", nameof(SyncDepartmentToLocations));
        //TcDepartmentsToLocations = TcContext.DepartmentsToLocations.ToList();

        if (this.CancellationToken.IsCancellationRequested) return;
        try
        {
            foreach (Location location in tcLoader.Locations.Where(l => l.JdeId != CommonValues.NonJdeId))
            {
                if (this.CancellationToken.IsCancellationRequested) return;
                DepartmentsToLocation? departmentToLocation = tcLoader.DepartmentsToLocations.FirstOrDefault(t => t.Location?.JdeId == location.JdeId && t.Department?.JdeId == location.JdeId);

                this.Logger.Information("{syncProcess}: {location} {department}", nameof(SyncDepartmentToLocations), location.Name, departmentToLocation?.Department?.Name);

                if (departmentToLocation is null)
                {
                    Department department = tcLoader.Departments.First(d => d.JdeId == location.JdeId);
                    departmentToLocation = new DepartmentsToLocation() { Location = location, Department = department, IsActive = true };
                    tcLoader.Context.DepartmentsToLocations.Add(departmentToLocation);
                    tcLoader.DepartmentsToLocations.Add(departmentToLocation);
                }
                else
                {
                    departmentToLocation.IsActive = (location.IsActive ?? true) && (departmentToLocation.Department.IsActive ?? true);
                }

                JdeToTcSync.DepartmentToLocationByJdeId.TryAdd(location.JdeId!, departmentToLocation);
            }

            this.Logger.LogFinished<DepartmentsToLocation>(tcLoader);
            this.Logger.LogNewLine();
        }
        catch (Exception ex)
        {
            this.Logger.LogSyncError(ex, null, nameof(SyncDepartmentToLocations), nameof(SyncDepartmentToLocations));
            if (!this.CancellationToken.IsCancellationRequested) this.CancellationTokenSource.Cancel();
        }
    }

    private void SyncUsers(TcEntityLoader tcLoader, JdeEntityLoader jdeLoader)
    {
        List<User> newItems = [];
        Syncer<int> syncer = new();
        List<string> nameParts = [string.Empty, string.Empty];
        Action<User, JdeEmployee, IEnumerable<AuthorizationClaim>> setClaims = (user, jdeEmployee, claims) =>
        {
            if (jdeEmployee.Subordinates.Count < 1)
            {
                // default users to be able to select equipment
                if (!user.UserClaims.Any(u => u.AuthorizationClaimId == AuthorizationClaimsDefinition.CanSelectEquipment.Id))
                    (user.UserClaims ??= []).Add(new UserClaim() { AuthorizationClaimId = AuthorizationClaimsDefinition.CanSelectEquipment.Id, UserId = user.Id });
                return;
            }
            // remove any claims which are not in our approved claims list
            user.UserClaims = user.UserClaims.Where(u => claims.Any(c => c.Id == u.Id)).ToList();
            // add any new claims assigned, but do not add duplicate, as that would cause exception
            user.UserClaims = claims
                                .Where(c => c.Id != AuthorizationClaimsDefinition.CanConfigureApp.Id && !user.UserClaims.Any(u => u.Id == c.Id))// 
                                // second Where is for debugging, to update a user's claims
                                //.Where(c => AuthorizationClaimsDefinition.Definitions.Any(a => !user.UserClaims.Any(u => u.AuthorizationClaimId == a.Id)))
                                .Select(c => new UserClaim() { AuthorizationClaimId = c.Id, UserId = user.Id }).ToList();
        };
        Action<User, IEnumerable<JdeEntityLoader.UserJobTypeStep>> syncJobTypeStep = (user, data) =>
        {
            // iterate first values in user and set all to inactive
            foreach(var value in data)
            {
                string jobTypeCode = value.JobTypeCode?.Trim() ?? string.Empty;
                string jobStepCode = value.JobStepCode?.Trim() ?? string.Empty;
                //var matches = user.JobTypeSteps.Where(j => j.JobType.JdeId == value.JobTypeCode && j.JobStep.JdeId == value.JobStepCode);
                //if (matches is not null)
                //    matches.ForEach(m => m.IsActive = true);
                //else
                if (!user.JobTypeSteps.Any(j => (j.JobType?.JdeId?.Trim() ?? string.Empty) == jobTypeCode && (j.JobStep?.JdeId?.Trim() ?? string.Empty) == jobStepCode))
                {
                    JobStep? jobStep;
                    JobType? jobCode;

                    if (
                        JdeToTcSync.JdeJobStepCodeToTcJobStepTrimd.TryGetValue(jobStepCode, out jobStep) &&
                        JdeToTcSync.JdeJobTypeCodeToTcJobTypeTrimd.TryGetValue(jobTypeCode, out jobCode)
                    ) {
                        user.JobTypeSteps.Add(new JobTypeStepToUser() { JobStep = jobStep, JobType = jobCode, IsActive = true });
                    }
                }
            }
        };

        syncer.OnBeforeCompare += (s, e) =>
        {
            if (this.CancellationToken.IsCancellationRequested) return;
            JdeEmployee? jde = null;
            try
            {
                JdeEmployee jdeEntity = jde = (JdeEmployee)e.JdeEntity!;
                nameParts = new(jdeEntity.Name.Split(',', StringSplitOptions.TrimEntries));

                while (nameParts.Count < 2)
                    nameParts.Add(string.Empty);
            }
            catch (Exception ex)
            {
                this.Logger.LogSyncError(ex, jde, nameof(syncer.OnBeforeCompare), nameof(SyncUsers));
                if (!this.CancellationToken.IsCancellationRequested) this.CancellationTokenSource.Cancel();
            }
        };

        syncer.OnNeedsNew += (s, e) =>
        {
            if (this.CancellationToken.IsCancellationRequested) return;
            JdeEmployee? jde = null;
            try
            {
                JdeEmployee jdeEntity = jde = (JdeEmployee)e.JdeEntity!;
                int jdeId = jdeEntity.Id;
                string jobType = jdeEntity.JobType;
                string jobStep = jdeEntity.JobStep;

                this.Logger.LogNewItem<User>(jdeId);
                var newUser = new User()
                {
                    IsActive = JdeEntityLoader.ActiveEmployeeStatuses.Contains(jdeEntity.PayStatus),
                    JdeId = jdeId,
                    FirstName = nameParts[1].Trim(),
                    LastName = nameParts[0].Trim(),
                    EmployeeNumber = jdeId.ToString(),
                    //UserName = jdeEntity.Name.Trim(), // DO NOT set username. leave null. unless domain username is known, set to known domain username
                    UnionCode = jdeEntity.UnionCode?.Trim(),
                    DefaultJobType = JdeToTcSync.JdeJobTypeCodeToTcJobTypeTrimd.ContainsKey(jobType.Trim())
                        ? JdeToTcSync.JdeJobTypeCodeToTcJobTypeTrimd[jobType.Trim()]
                        : null,
                    DefaultJobStep = JdeToTcSync.JdeJobStepCodeToTcJobStepTrimd.ContainsKey(jobStep.Trim())
                        ? JdeToTcSync.JdeJobStepCodeToTcJobStepTrimd[jobStep.Trim()]
                        : null,
                    DepartmentsToLocation = tcLoader.DepartmentsToLocations.FirstOrDefault(t => t.Department.JdeId == jdeEntity.LocationId) ?? tcLoader.DepartmentsToLocations.First(t => t.Department.JdeId == CommonValues.NonJdeId),
                    SupervisorJdeId = jdeEntity.Supervisor?.Id,
                    PrimaryEmail = jdeEntity.Emails.FirstOrDefault()?.Email?.Trim(), //jdeEntity emails loaded were only our company.com
                    IsAdmin = jdeLoader.Employees.Any(m => ((JdeEmployee)m).SupervisorId == jdeEntity.Id),
                };
                tcLoader.Context.Users.Add(newUser);
                newItems.Add(newUser);
                // assigns management permissions to any employee which contains subordinates
                setClaims(newUser, jdeEntity, tcLoader.AuthorizationClaims);
                // adds job types and job steps
                if (!this.FilterEmployeesToActive && jdeLoader.UserJobTypeSteps.ContainsKey(jdeId))
                {
                    syncJobTypeStep(newUser, jdeLoader.UserJobTypeSteps[jdeId]);
                }
            }
            catch (Exception ex)
            {
                this.Logger.LogSyncError(ex, jde, nameof(syncer.OnNeedsNew), nameof(SyncUsers));
                if (!this.CancellationToken.IsCancellationRequested) this.CancellationTokenSource.Cancel();
            }
        };

        syncer.OnNeedsUpdate += (s, e) =>
        {
            if (this.CancellationToken.IsCancellationRequested) return;
            JdeEmployee? jde = null;
            try
            {
                JdeEmployee jdeEntity = jde = (JdeEmployee)e.JdeEntity!;
                User tcEntity = (User)e.TimeClockEntity!;
                int jdeId = jdeEntity.Id;

                this.Logger.LogUpdateItem<User>(jdeId, tcEntity.Id);
                tcEntity.FirstName = nameParts[1].Trim();
                tcEntity.LastName = nameParts[0].Trim();
                tcEntity.EmployeeNumber = jdeId.ToString();
                tcEntity.UnionCode = jdeEntity.UnionCode?.Trim();
                //tcEntity.UserName = jdeEntity.Name.Trim(); // DO NOT set username. leave null. unless domain username is known, set to known domain username
                tcEntity.DepartmentsToLocation = tcLoader.DepartmentsToLocations.First(t => t.Department.JdeId == jdeEntity.LocationId);
                tcEntity.IsActive = JdeEntityLoader.ActiveEmployeeStatuses.Contains(jdeEntity.PayStatus);
                tcEntity.DefaultJobType = JdeToTcSync.JdeJobTypeCodeToTcJobTypeTrimd.ContainsKey(jdeEntity.JobType.Trim())
                    ? JdeToTcSync.JdeJobTypeCodeToTcJobTypeTrimd[jdeEntity.JobType.Trim()]
                    : null;
                tcEntity.DefaultJobStep = JdeToTcSync.JdeJobStepCodeToTcJobStepTrimd.ContainsKey(jdeEntity.JobStep.Trim())
                    ? JdeToTcSync.JdeJobStepCodeToTcJobStepTrimd[jdeEntity.JobStep.Trim()]
                    : null;
                tcEntity.SupervisorJdeId = jdeEntity.Supervisor?.Id;
                tcEntity.PrimaryEmail = jdeEntity.Emails.FirstOrDefault()?.Email?.Trim();
                tcEntity.IsAdmin = jdeLoader.Employees.Any(m => ((JdeEmployee)m).SupervisorId == jdeEntity.Id);
                // we do not update permissions here because if they were changed, they should not get overridden
                setClaims(tcEntity, jdeEntity, tcLoader.AuthorizationClaims);
                // adds job types and job steps
                if (!this.FilterEmployeesToActive && jdeLoader.UserJobTypeSteps.ContainsKey(jdeId))
                {
                    syncJobTypeStep(tcEntity, jdeLoader.UserJobTypeSteps[jdeId]);
                }
            }
            catch (Exception ex)
            {
                this.Logger.LogSyncError(ex, jde, nameof(syncer.OnNeedsUpdate), nameof(SyncUsers));
                if (!this.CancellationToken.IsCancellationRequested) this.CancellationTokenSource.Cancel();
            }
        };

        syncer.OnDelete += (s, e) =>
        {
            if (this.CancellationToken.IsCancellationRequested) return;
            JdeEmployee? jde = null;
            try
            {
                JdeEmployee? jdeEntity = jde = (JdeEmployee?)e.JdeEntity;
                User tcEntity = (User)e.TimeClockEntity!;
                int jdeId = jdeEntity?.Id ?? tcEntity.JdeId ?? 0;

                this.Logger.LogDeleteItem<User>(jdeId, tcEntity.Id);
                tcEntity.IsActive = false;
            }
            catch (Exception ex)
            {
                this.Logger.LogSyncError(ex, jde, nameof(syncer.OnDelete), nameof(SyncUsers));
                if (!this.CancellationToken.IsCancellationRequested) this.CancellationTokenSource.Cancel();
            }
        };

        syncer.OnFinished += (s, e) =>
        {
            if (this.CancellationToken.IsCancellationRequested) return;
            
            tcLoader.Users.AddRange(newItems.OrderBy(u => u.JdeId));

            this.Logger.LogFinished<User>(tcLoader);
            this.Logger.LogNewLine();
        };

        if (this.CancellationToken.IsCancellationRequested) return;
        try
        {
            syncer.SyncJdeToTc(jdeLoader.Employees, j => ((JdeEmployee)j).Id, tcLoader.Users);
        }
        catch (Exception ex)
        {
            this.Logger.LogSyncError(ex, null, nameof(syncer.SyncJdeToTc), nameof(SyncUsers));
            if (!this.CancellationToken.IsCancellationRequested) this.CancellationTokenSource.Cancel();
        }
    }

    private void SyncJobTypes(TcEntityLoader tcLoader, JdeEntityLoader jdeLoader)
    {
        List<JobType> newItems = [];
        Syncer<string> syncer = new();

        syncer.OnNeedsNew += (s, e) =>
        {
            if (this.CancellationToken.IsCancellationRequested) return;
            JdeCustomCode? jde = null;
            try
            {
                JdeCustomCode jdeEntity = jde = (JdeCustomCode)e.JdeEntity!;
                string jdeId = jdeEntity.Code;

                this.Logger.LogNewItem<JobType>(jdeId);
                var newItem = new JobType()
                {
                    IsActive = true,
                    JdeId = jdeId,
                    Description = jdeEntity.Description?.Trim() ?? "Unknown"
                };
                tcLoader.Context.JobTypes.Add(newItem);
                newItems.Add(newItem);
                JdeToTcSync.JdeJobTypeCodeToTcJobType.TryAdd(jdeId, newItem);
                JdeToTcSync.JdeJobTypeCodeToTcJobTypeTrimd.TryAdd(jdeId.Trim(), newItem);
            }
            catch (Exception ex)
            {
                this.Logger.LogSyncError(ex, jde, nameof(syncer.OnNeedsNew), nameof(SyncJobTypes));
                if (!this.CancellationToken.IsCancellationRequested) this.CancellationTokenSource.Cancel();
            }
        };

        syncer.OnNeedsUpdate += (s, e) =>
        {
            if (this.CancellationToken.IsCancellationRequested) return;
            JdeCustomCode? jde = null;
            try
            {
                JdeCustomCode jdeEntity = jde = (JdeCustomCode)e.JdeEntity!;
                JobType tcEntity = (JobType)e.TimeClockEntity!;
                string jdeId = jdeEntity.Code;

                this.Logger.LogUpdateItem<JobType>(jdeId, tcEntity.Id);
                tcEntity.Description = jdeEntity.Description?.Trim() ?? "Unknown";
                tcEntity.IsActive = true;
                JdeToTcSync.JdeJobTypeCodeToTcJobType.TryAdd(jdeId, tcEntity);
                JdeToTcSync.JdeJobTypeCodeToTcJobTypeTrimd.TryAdd(jdeId.Trim(), tcEntity);
            }
            catch (Exception ex)
            {
                this.Logger.LogSyncError(ex, jde, nameof(syncer.OnNeedsUpdate), nameof(SyncJobTypes));
                if (!this.CancellationToken.IsCancellationRequested) this.CancellationTokenSource.Cancel();
            }
        };

        syncer.OnDelete += (s, e) =>
        {
            if (this.CancellationToken.IsCancellationRequested) return;
            JdeCustomCode? jde = null;
            try
            {
                JdeCustomCode? jdeEntity = jde = (JdeCustomCode?)e.JdeEntity;
                JobType tcEntity = (JobType)e.TimeClockEntity!;
                string jdeId = jdeEntity?.Code ?? tcEntity.JdeId ?? string.Empty;

                this.Logger.LogDeleteItem<JobType>(jdeId, tcEntity.Id);
                tcEntity.IsActive = false;
            }
            catch (Exception ex)
            {
                this.Logger.LogSyncError(ex, jde, nameof(syncer.OnDelete), nameof(SyncJobTypes));
                if (!this.CancellationToken.IsCancellationRequested) this.CancellationTokenSource.Cancel();
            }
        };

        syncer.OnFinished += (s, e) =>
        {
            if (this.CancellationToken.IsCancellationRequested) return;
            tcLoader.JobTypes.AddRange(newItems.OrderBy(d => d.JdeId));

            this.Logger.LogFinished<JobType>(tcLoader);
            this.Logger.LogNewLine();
        };

        try
        {
            if (this.CancellationToken.IsCancellationRequested) return;
            syncer.SyncJdeToTc(jdeLoader.JobTypes, (j) => ((JdeCustomCode)j).Code, tcLoader.JobTypes);
        }
        catch (Exception ex)
        {
            this.Logger.LogSyncError(ex, null, nameof(syncer.SyncJdeToTc), nameof(SyncJobTypes));
            if (!this.CancellationToken.IsCancellationRequested) this.CancellationTokenSource.Cancel();
        }
    }

    private void SyncJobSteps(TcEntityLoader tcLoader, JdeEntityLoader jdeLoader)
    {
        List<JobStep> newItems = [];
        Syncer<string> syncer = new();

        syncer.OnNeedsNew += (s, e) =>
        {
            if (this.CancellationToken.IsCancellationRequested) return;
            JdeCustomCode? jde = null;
            try
            {
                JdeCustomCode jdeEntity = jde = (JdeCustomCode)e.JdeEntity!;
                string jdeId = jdeEntity.Code;

                this.Logger.LogNewItem<JobStep>(jdeId);
                var newItem = new JobStep()
                {
                    IsActive = true,
                    JdeId = jdeId,
                    Description = jdeEntity.Description?.Trim() ?? "Unknown"
                };
                tcLoader.Context.JobSteps.Add(newItem);
                newItems.Add(newItem);
                JdeToTcSync.JdeJobStepCodeToTcJobStep.TryAdd(jdeId, newItem);
                JdeToTcSync.JdeJobStepCodeToTcJobStepTrimd.TryAdd(jdeId.Trim(), newItem);
            }
            catch (Exception ex)
            {
                this.Logger.LogSyncError(ex, jde, nameof(syncer.OnNeedsNew), nameof(SyncJobSteps));
                if (!this.CancellationToken.IsCancellationRequested) this.CancellationTokenSource.Cancel();
            }
        };

        syncer.OnNeedsUpdate += (s, e) =>
        {
            if (this.CancellationToken.IsCancellationRequested) return;
            JdeCustomCode? jde = null;
            try
            {
                JdeCustomCode jdeEntity = jde = (JdeCustomCode)e.JdeEntity!;
                JobStep tcEntity = (JobStep)e.TimeClockEntity!;
                string jdeId = jdeEntity.Code;

                this.Logger.LogUpdateItem<JobStep>(jdeId, tcEntity.Id);
                tcEntity.Description = jdeEntity.Description?.Trim() ?? "Unknown";
                tcEntity.IsActive = true;
                JdeToTcSync.JdeJobStepCodeToTcJobStep.TryAdd(jdeId, tcEntity);
                JdeToTcSync.JdeJobStepCodeToTcJobStepTrimd.TryAdd(jdeId.Trim(), tcEntity);
            }
            catch (Exception ex)
            {
                this.Logger.LogSyncError(ex, jde, nameof(syncer.OnNeedsUpdate), nameof(SyncJobSteps));
                if (!this.CancellationToken.IsCancellationRequested) this.CancellationTokenSource.Cancel();
            }
        };

        syncer.OnDelete += (s, e) =>
        {
            if (this.CancellationToken.IsCancellationRequested) return;
            JdeCustomCode? jde = null;
            try
            {
                JdeCustomCode? jdeEntity = jde = (JdeCustomCode?)e.JdeEntity;
                JobStep tcEntity = (JobStep)e.TimeClockEntity!;
                string jdeId = jdeEntity?.Code ?? tcEntity.JdeId ?? string.Empty; ;

                this.Logger.LogDeleteItem<JobStep>(jdeId, tcEntity.Id);
                tcEntity.IsActive = false;
            }
            catch (Exception ex)
            {
                this.Logger.LogSyncError(ex, jde, nameof(syncer.OnDelete), nameof(SyncJobSteps));
                if (!this.CancellationToken.IsCancellationRequested) this.CancellationTokenSource.Cancel();
            }
        };

        syncer.OnFinished += (s, e) =>
        {
            if (this.CancellationToken.IsCancellationRequested) return;
            tcLoader.JobSteps.AddRange(newItems.OrderBy(d => d.JdeId));

            this.Logger.LogFinished<JobStep>(tcLoader);
            this.Logger.LogNewLine();
        };

        try
        {
            if (this.CancellationToken.IsCancellationRequested) return;
            syncer.SyncJdeToTc(jdeLoader.JobSteps, (j) => ((JdeCustomCode)j).Code, tcLoader.JobSteps);
        }
        catch (Exception ex)
        {
            this.Logger.LogSyncError(ex, null, nameof(syncer.SyncJdeToTc), nameof(SyncJobSteps));
            if (!this.CancellationToken.IsCancellationRequested) this.CancellationTokenSource.Cancel();
        }
    }

    #endregion Sync JDE to TimeClock



    #region IDisposable
    protected virtual void Dispose(bool disposing)
    {
        if (!this._disposedValue)
        {
            if (disposing)
            {
                try { this.JdeLoader.Dispose(); } catch (Exception) { }
                try { this.TcLoader.Dispose(); } catch (Exception) { }
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
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
