using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Text;
using TimeClock.Data.Helpers;
using TimeClock.Data.Models;

namespace TimeClock.Data.Repository;

public interface IEquipmentsRepository : IDataRepository<Equipment>
{
    IQueryable<Equipment> GetActive(bool isActive = true, IPaging? paging = null, ISorting<Equipment>? sorting = null, bool? tracking = null);
    Task<IEnumerable<Equipment>> GetActiveAsync(bool isActive = true, IPaging? paging = null, ISorting<Equipment>? sorting = null, bool? tracking = null, CancellationToken token = default, params Expression<Func<Equipment, object>>[] includes);
    IQueryable<Equipment> GetByEquipmentType(Guid equipmentTypeId, IPaging? paging = null, ISorting<Equipment>? sorting = null, bool? tracking = null);
    Task<IEnumerable<Equipment>> GetByEquipmentTypeAsync(Guid equipmentTypeId, IPaging? paging = null, ISorting<Equipment>? sorting = null, bool? tracking = null, CancellationToken token = default, params Expression<Func<Equipment, object>>[] includes);
    IQueryable<Equipment?> GetByName(string name, IPaging? paging = null, ISorting<Equipment>? sorting = null, bool? tracking = null);
    Task<IEnumerable<Equipment?>> GetByNameAsync(string name, IPaging? paging = null, ISorting<Equipment>? sorting = null, bool? tracking = null, CancellationToken token = default, params Expression<Func<Equipment, object>>[] includes);
    Equipment? GetBySku(string sku, bool? tracking = null, params Expression<Func<Equipment, object>>[] includes);
    Task<Equipment?> GetBySkuAsync(string sku, bool? tracking = null, CancellationToken token = default, params Expression<Func<Equipment, object>>[] includes);
    void AddToDepartmentLocation(Equipment equipment, Guid departmentId, Guid locationId, Guid userId, bool? save = null);
    Task AddToDepartmentLocationAsync(Equipment equipment, Guid departmentId, Guid locationId, Guid userId, bool? save = null, CancellationToken token = default);
    EquipmentsToUser LinkToUser(Guid equipmentId, Guid userId, Guid actionById, Guid? jobTypeId, Guid? jobStepId, int workPeriodBuffer,
        DateTime? linkedOn = null, DateTime? linkedOnEffective = null, Guid? workPeriodId = null, PunchEntriesHistory? punchEntriesHistory = null, bool? save = null);
    Task<EquipmentsToUser> LinkToUserAsync(Guid equipmentId, Guid userId, Guid actionById, Guid? jobTypeId, Guid? jobStepId, int workPeriodBuffer,
        DateTime? linkedOn = null, DateTime? linkedOnEffective = null, Guid? workPeriodId = null, PunchEntriesHistory? punchEntriesHistory = null, bool? save = null, CancellationToken token = default);
    EquipmentsToUser? UnlinkToUser(Guid? equipmentsToUserId, Guid equipmentId, Guid userId, Guid actionById,
        DateTime? unlinkedOn = null, DateTime? unlinkedOnEffective = null, bool? save = null);
    Task<EquipmentsToUser?> UnlinkToUserAsync(Guid? equipmentsToUserId, Guid equipmentId, Guid userId, Guid actionById,
        DateTime? unlinkedOn = null, DateTime? unlinkedOnEffective = null, Guid? jobStepId = null, Guid? jobTypeId = null, 
        bool? save = null, CancellationToken token = default);
    IEnumerable<EquipmentsToUser> UnlinkToUserByWorkPeriod(Guid workPeriodId, Guid actionById,
        DateTime? unlinkedOn = null, DateTime? unlinkedOnEffective = null, bool? save = null);
    Task<IEnumerable<EquipmentsToUser>> UnlinkToUserByWorkPeriodAsync(Guid workPeriodId, Guid actionById,
        DateTime? unlinkedOn = null, DateTime? unlinkedOnEffective = null, Guid? jobStepId = null, Guid? jobTypeId = null,
        bool? save = null, CancellationToken token = default);
    void DisableInDepartmentLocation(Guid equipmentId, Guid departmentId, Guid locationId, bool? save = null);
    Task DisableInDepartmentLocationAsync(Guid equipmentId, Guid departmentId, Guid locationId, bool? save = null, CancellationToken token = default);
}

public class EquipmentsRepository : DataRepository<Equipment>, IEquipmentsRepository
{
    internal EquipmentsRepository(TimeClockContext context)
        : base(context) { }

    public Equipment? GetBySku(string sku, bool? tracking = null, params Expression<Func<Equipment, object>>[] includes)
    {
        return base.GetTracker(tracking).IncludeMultiple(includes).FirstOrDefault(x => x.Sku == sku);
    }
    public IQueryable<Equipment?> GetByName(string name, IPaging? paging = null, ISorting<Equipment>? sorting = null, bool? tracking = null)
    {
        IQueryable<Equipment> results = base.SortedDbSet(sorting, tracking).Where(x => x.Name == name);

        if (paging != null)
            return results.PageResults(paging);
        return results;
    }
    public IQueryable<Equipment> GetByEquipmentType(Guid equipmentTypeId, IPaging? paging = null, ISorting<Equipment>? sorting = null, bool? tracking = null)
    {
        IQueryable<Equipment> results =  base.SortedDbSet(sorting, tracking).Where(x => x.EquipmentTypeId == equipmentTypeId);

        if (paging != null)
            return results.PageResults(paging);
        return results;
    }
    public IQueryable<Equipment> GetActive(bool isActive = true, IPaging? paging = null, ISorting<Equipment>? sorting = null, bool? tracking = null)
    {
        IQueryable<Equipment> results = base.SortedDbSet(sorting, tracking).Where(x => x.IsActive == isActive);

        if (paging != null)
            return results.PageResults(paging);
        return results;
    }
    /// <summary>
    /// Adds a new Equipment or updates existing Equipment to an existing or new Department and Location relationship.
    /// Note: the Department and Location must already exist.
    /// </summary>
    /// <param name="equipment"></param>
    /// <param name="departmentId"></param>
    /// <param name="locationId"></param>
    /// <param name="userId">ID of the User who is responsible for adding this Equipment to the Location</param>
    /// <param name="save"></param>
    public void AddToDepartmentLocation(Equipment equipment, Guid departmentId, Guid locationId, Guid userId, bool? save = null)
    {
        DepartmentsToLocation? departmentLocation = base.Context.DepartmentsToLocations.FirstOrDefault(d => d.DepartmentId == departmentId && d.LocationId == locationId);
        EquipmentsToDepartmentLocation? equipmentLocation = null;
        bool isEquipmentLocationNew = false;
        SaveType saveType = base.GetSaveType(save);

        if (departmentLocation is null)
            throw new ArgumentException("The specified Department or Location does not exist");
        if (!base.Context.Equipments.Any(q => q.Id == equipment.Id))
            base.Context.Equipments.Add(equipment);
        equipmentLocation = base.Context.EquipmentsToLocations.FirstOrDefault(q => q.EquipmentId == equipment.Id);

        if (equipmentLocation is null) 
        {
            isEquipmentLocationNew = true;
            equipmentLocation = new EquipmentsToDepartmentLocation() { Equipment = equipment, DepartmentsToLocation = departmentLocation, LinkedById = userId, LinkedOn = DateTime.Now };
            base.Context.EquipmentsToLocations.Add(equipmentLocation);
        }
        else
        {
            equipmentLocation.LinkedById = userId;
            equipmentLocation.IsActive = true;
            equipmentLocation.DepartmentsToLocation = departmentLocation;
            equipmentLocation.LinkedOn = DateTime.Now;
        }

        switch (saveType)
        {
            case SaveType.SaveToDb:
                if (isEquipmentLocationNew)
                    base.Save();
                else
                {
                    base.Context.EquipmentsToLocations.Where(q => q.EquipmentId == equipment.Id).ExecuteUpdate(setter =>
                        setter.SetProperty(q => q.LinkedById, userId).SetProperty(q => q.IsActive, true)
                        .SetProperty(q => q.DepartmentsToLocation, departmentLocation).SetProperty(q => q.LinkedOn, DateTime.Now));
                }
                break;
            case SaveType.SaveToDbSet:
                base.Save();
                break;
            case SaveType.TrackToDbSet:
                if (!isEquipmentLocationNew)
                    base.Context.EquipmentsToLocations.Update(equipmentLocation);
                break;
        }
    }
    public void DisableInDepartmentLocation(Guid equipmentId, Guid departmentId, Guid locationId, bool? save = null)
    {
        DepartmentsToLocation? departmentLocation = base.Context.DepartmentsToLocations.FirstOrDefault(d => d.DepartmentId == departmentId && d.LocationId == locationId);
        EquipmentsToDepartmentLocation? equipmentLocation = null;
        SaveType saveType = base.GetSaveType(save);

        if (departmentLocation is null)
            throw new ArgumentException("The specified Department or Location does not exist");
        equipmentLocation = base.Context.EquipmentsToLocations.FirstOrDefault(q => q.EquipmentId == equipmentId);
        if (equipmentLocation is null)
            throw new ArgumentException("The specified equipment is not at the Department and Location");

        equipmentLocation.IsActive = false;

        switch (saveType)
        {
            case SaveType.SaveToDb:
                base.Context.EquipmentsToLocations.Where(q => q.EquipmentId == equipmentId)
                .ExecuteUpdate(setters => setters.SetProperty(q => q.IsActive, false));
                break;
            case SaveType.SaveToDbSet:
                base.Save();
                break;
            case SaveType.TrackToDbSet:
                base.Context.EquipmentsToLocations.Update(equipmentLocation);
                break;
        }
    }
    /// <summary>
    /// Incomplete implementation, will throw.
    /// </summary>
    /// <param name="equipmentId"></param>
    /// <param name="userId"></param>
    /// <param name="actionById"></param>
    /// <param name="jobTypeId"></param>
    /// <param name="jobStepId"></param>
    /// <param name="workPeriodBuffer"></param>
    /// <param name="linkedOn"></param>
    /// <param name="linkedOnEffective"></param>
    /// <param name="_workPeriodId"></param>
    /// <param name="punchEntriesHistory"></param>
    /// <param name="save"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public EquipmentsToUser LinkToUser(Guid equipmentId, Guid userId, Guid actionById, Guid? jobTypeId, Guid? jobStepId, int workPeriodBuffer,
        DateTime? linkedOn = null, DateTime? linkedOnEffective = null, Guid? _workPeriodId = null, PunchEntriesHistory? punchEntriesHistory = null, bool? save = null)
    {
        throw new NotImplementedException();
        SaveType saveType = base.GetSaveType(save);
        Guid newWorkPeriodId = Guid.NewGuid();
        DateTime now = DateTime.Now;
        EquipmentsToUser link = new()
        {
            EquipmentId = equipmentId,
            UserId = userId,
            LinkedOn = now,
            LinkedById = actionById,
            WorkPeriodId = newWorkPeriodId
        };

        // see async version for notes
        WorkPeriodsRepository workPeriodRepo = new(base.Context);
        DateTime minDate = now.AddHours(workPeriodBuffer * -1);
        DateTime maxDate = now.AddHours(workPeriodBuffer);
        WorkPeriod? workPeriod = null;

        // need to get work period for this link if it exists. but how? we can use punch entries
        // only a work period which has not been submitted and is for calculating total hours worked
        workPeriod = base.Context.WorkPeriods.Where(w =>
            w.UserId == userId &&
            w.PunchEntries.Any(p => p.CurrentState.PunchEntriesHistory.DateTime >= minDate && p.CurrentState.PunchEntriesHistory.DateTime <= maxDate) &&
            w.Purpose == WorkPeriodPurpose.PunchEntriesSum &&
            w.WorkPeriodStatusHistories.OrderByDescending(h => h.RowId).First().Status == WorkPeriodStatus.Pending)
            .OrderByDescending(w => w.RowId)
            .FirstOrDefault();

        if (workPeriod is null)
        {
            workPeriod = new()
            {
                Id = newWorkPeriodId,
                HoursWorked = 0,
                PayPeriodEnd = DateOnly.FromDateTime(now.Date.NextDayOfWeek(DayOfWeek.Sunday).AddMicroseconds(-1)),
                Purpose = WorkPeriodPurpose.PunchEntriesSum,
                UserId = userId,
                WorkDate = now.Date.ToDateOnly(),
            };

            workPeriod.WorkPeriodStatusHistories.Add(new WorkPeriodStatusHistory()
            {
                DateTime = now,
                Status = WorkPeriodStatus.Pending,
                WorkPeriodId = workPeriod.Id
            });

            // since we are creating a new work period, we know we do not have any punches. so we need to create one
            if (punchEntriesHistory is not null)
            {
                PunchEntry punchEntry = new() { Id = Guid.NewGuid(), UserId = userId, WorkPeriodId = workPeriod.Id };
                punchEntriesHistory.PunchEntryId = punchEntry.Id;
                punchEntry.PunchEntriesHistories.Add(punchEntriesHistory);
                PunchEntriesCurrentState currentState = new() { PunchEntry = punchEntry, PunchEntriesHistory = punchEntriesHistory };
                punchEntry.CurrentState = currentState;
                workPeriod.PunchEntries.Add(punchEntry);
                base.Context.PunchEntries.Add(punchEntry);
            }
        }

        link.WorkPeriod = workPeriod;

        base.Context.EquipmentsToUsers.Add(link);

        //******* NOTE: about JobType/Step to WorkPeriod Equipment
        // not using workPeriodJobType at least for the time being, due to having to disable and/or replace 
        // the JobType and step when altering PunchEntry state (void, edit, etc)
        //if (!base.Context.WorkPeriodJobTypes.Any(j => j.JobTypeId == jobTypeId && j.JobStepId == jobStepId && j.WorkPeriodId == workPeriod.Id))
        //{
        //    WorkPeriodJobType workPeriodJobType = new WorkPeriodJobType()
        //    {
        //        ActiveSince = now,
        //        JobTypeId = jobTypeId,
        //        JobStepId = jobStepId,
        //        WorkPeriodId = workPeriod.Id,
        //        WorkPeriod = workPeriod
        //    };
        //    base.Context.WorkPeriodJobTypes.Add(workPeriodJobType);
        //    workPeriod.WorkPeriodJobTypes.Add(workPeriodJobType);
        //}

        if (saveType != SaveType.None)
            base.Save();

        return link;
    }
    /// <summary>
    /// Incomplete implementation, will throw.
    /// </summary>
    /// <param name="linkId"></param>
    /// <param name="equipmentId"></param>
    /// <param name="userId"></param>
    /// <param name="actionById"></param>
    /// <param name="unlinkedOn"></param>
    /// <param name="unlinkedOnEffective"></param>
    /// <param name="save"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public EquipmentsToUser? UnlinkToUser(Guid? linkId, Guid equipmentId, Guid userId, Guid actionById,
        DateTime? unlinkedOn = null, DateTime? unlinkedOnEffective = null, bool? save = null)
    {
        throw new NotImplementedException();
        SaveType saveType = base.GetSaveType(save);
        IQueryable<EquipmentsToUser> query = linkId.HasValue
            ? base.Context.EquipmentsToUsers.Where(t => t.Id == linkId.Value)
            : base.Context.EquipmentsToUsers
                .Where(q => q.UserId == userId && q.EquipmentId == equipmentId && q.UnLinkedOn == null)
                .OrderByDescending(q => q.LinkedOn);
        EquipmentsToUser? equipmentToUser = null;
        if (saveType == SaveType.SaveToDb)
        {
            query.ExecuteUpdate(setters => setters.SetProperty(q => q.UnLinkedOn, DateTime.Now).SetProperty(q => q.UnlinkedById, actionById));
        }
        else
        {
            equipmentToUser = query.FirstOrDefault();
            if (equipmentToUser is null)
                throw new ArgumentException("No Equipment link to User found");
            equipmentToUser.UnLinkedOn = DateTime.Now;
            equipmentToUser.UnlinkedById = actionById;

            if (saveType == SaveType.SaveToDbSet)
                base.Save();
            else if (saveType == SaveType.TrackToDbSet)
                base.Context.EquipmentsToUsers.Update(equipmentToUser);
        }

        return equipmentToUser;
    }
    /// <summary>
    /// Incomplete implementation, will throw.
    /// </summary>
    /// <param name="workPeriodId"></param>
    /// <param name="actionById"></param>
    /// <param name="unlinkedOn"></param>
    /// <param name="unlinkedOnEffective"></param>
    /// <param name="save"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public IEnumerable<EquipmentsToUser> UnlinkToUserByWorkPeriod(Guid workPeriodId, Guid actionById,
        DateTime? unlinkedOn = null, DateTime? unlinkedOnEffective = null, bool? save = null) 
    {
        throw new NotImplementedException();
        SaveType saveType = base.GetSaveType(save);
        IQueryable<EquipmentsToUser> query = base.Context.EquipmentsToUsers.Where(t => t.WorkPeriodId == workPeriodId && t.UnLinkedOn == null);

        IEnumerable<EquipmentsToUser> equipmentsToUser = Enumerable.Empty<EquipmentsToUser>();
        if (saveType == SaveType.SaveToDb)
        {
            query.ExecuteUpdate(setters => setters.SetProperty(q => q.UnLinkedOn, DateTime.Now).SetProperty(q => q.UnlinkedById, actionById));
        }
        else
        {
            equipmentsToUser = query.ToList();
            if (equipmentsToUser is null)
                throw new ArgumentException("No Equipment link to User found");
            foreach(var item in equipmentsToUser)
            {
                item.UnLinkedOn = DateTime.Now;
                item.UnlinkedById = actionById;
                
                if (saveType == SaveType.TrackToDbSet)
                    base.Context.EquipmentsToUsers.Update(item);
            }

            if (saveType == SaveType.SaveToDbSet)
                base.Save();
        }

        return equipmentsToUser;
    }

    public Task<Equipment?> GetBySkuAsync(string sku, bool? tracking = null, CancellationToken token = default, params Expression<Func<Equipment, object>>[] includes)
    {
        return base.GetTracker(tracking).IncludeMultiple(includes).FirstOrDefaultAsync(x => x.Sku == sku, token);
    }
    public async Task<IEnumerable<Equipment?>> GetByNameAsync(string name, IPaging? paging = null, ISorting<Equipment>? sorting = null, bool? tracking = null, CancellationToken token = default, params Expression<Func<Equipment, object>>[] includes)
    {
        IQueryable<Equipment> results = base.SortedDbSet(sorting, tracking).IncludeMultiple(includes).Where(x => x.Name == name);

        if (paging != null)
            return await results.PageResults(paging).ToListAsync(token);
        return await results.ToListAsync(token);
    }
    public async Task<IEnumerable<Equipment>> GetByEquipmentTypeAsync(Guid equipmentTypeId, IPaging? paging = null, ISorting<Equipment>? sorting = null, bool? tracking = null, CancellationToken token = default, params Expression<Func<Equipment, object>>[] includes)
    {
        IQueryable<Equipment> results = base.SortedDbSet(sorting, tracking).IncludeMultiple(includes).Where(x => x.EquipmentTypeId == equipmentTypeId);

        if (paging != null)
            return await results.PageResults(paging).ToListAsync(token);
        return await results.ToListAsync(token);
    }
    public async Task<IEnumerable<Equipment>> GetActiveAsync(bool isActive = true, IPaging? paging = null, ISorting<Equipment>? sorting = null, bool? tracking = null, CancellationToken token = default, params Expression<Func<Equipment, object>>[] includes)
    {
        var results =base.SortedDbSet(sorting, tracking).IncludeMultiple(includes).Where(x => x.IsActive == isActive);

        if (paging != null)
            return await results.PageResults(paging).ToListAsync(token);
        return await results.ToListAsync(token);
    }
    public async Task AddToDepartmentLocationAsync(Equipment equipment, Guid departmentId, Guid locationId, Guid userId, bool? save = null, CancellationToken token = default)
    {
        DepartmentsToLocation? departmentLocation = await base.Context.DepartmentsToLocations.FirstOrDefaultAsync(d => d.DepartmentId == departmentId && d.LocationId == locationId);
        EquipmentsToDepartmentLocation? equipmentLocation = null;
        bool isEquipmentLocationNew = false;
        SaveType saveType = base.GetSaveType(save);

        if (departmentLocation is null)
            throw new ArgumentException("The specified Department or Location does not exist");
        if (! await base.Context.Equipments.AnyAsync(q => q.Id == equipment.Id))
            await base.Context.Equipments.AddAsync(equipment);
        equipmentLocation = await base.Context.EquipmentsToLocations.FirstOrDefaultAsync(q => q.EquipmentId == equipment.Id);

        if (equipmentLocation is null)
        {
            isEquipmentLocationNew = true;
            equipmentLocation = new EquipmentsToDepartmentLocation() { Equipment = equipment, DepartmentsToLocation = departmentLocation, LinkedById = userId, LinkedOn = DateTime.Now };
            await base.Context.EquipmentsToLocations.AddAsync(equipmentLocation);
        }
        else
        {
            equipmentLocation.LinkedById = userId;
            equipmentLocation.IsActive = true;
            equipmentLocation.DepartmentsToLocation = departmentLocation;
            equipmentLocation.LinkedOn = DateTime.Now;
        }

        switch (saveType)
        {
            case SaveType.SaveToDb:
                if (isEquipmentLocationNew)
                    await base.SaveAsync();
                else
                {
                    await base.Context.EquipmentsToLocations.Where(q => q.EquipmentId == equipment.Id).ExecuteUpdateAsync(setter =>
                        setter.SetProperty(q => q.LinkedById, userId).SetProperty(q => q.IsActive, true)
                        .SetProperty(q => q.DepartmentsToLocation, departmentLocation).SetProperty(q => q.LinkedOn, DateTime.Now));
                }
                break;
            case SaveType.SaveToDbSet:
                await base.SaveAsync();
                break;
            case SaveType.TrackToDbSet:
                if (!isEquipmentLocationNew)
                    base.Context.EquipmentsToLocations.Update(equipmentLocation);
                break;
        }
    }
    public async Task DisableInDepartmentLocationAsync(Guid equipmentId, Guid departmentId, Guid locationId, bool? save = null, CancellationToken token = default)
    {
        DepartmentsToLocation? departmentLocation = await base.Context.DepartmentsToLocations.FirstOrDefaultAsync(d => d.DepartmentId == departmentId && d.LocationId == locationId);
        EquipmentsToDepartmentLocation? equipmentLocation = null;
        SaveType saveType = base.GetSaveType(save);

        if (departmentLocation is null)
            throw new ArgumentException("The specified Department or Location does not exist");
        equipmentLocation = await base.Context.EquipmentsToLocations.FirstOrDefaultAsync(q => q.EquipmentId == equipmentId);
        if (equipmentLocation is null)
            throw new ArgumentException("The specified equipment is not at the Department and Location");

        equipmentLocation.IsActive = false;

        switch (saveType)
        {
            case SaveType.SaveToDb:
                await base.Context.EquipmentsToLocations.Where(q => q.EquipmentId == equipmentId)
                .ExecuteUpdateAsync(setters => setters.SetProperty(q => q.IsActive, false));
                break;
            case SaveType.SaveToDbSet:
                await base.SaveAsync();
                break;
            case SaveType.TrackToDbSet:
                base.Context.EquipmentsToLocations.Update(equipmentLocation);
                break;
        }
    }
    /// <summary>
    /// Creates a new link from user to equipment. Will also create a new work period for the new link.
    /// </summary>
    /// <param name="equipmentId"></param>
    /// <param name="userId"></param>
    /// <param name="actionById"></param>
    /// <param name="jobTypeId"></param>
    /// <param name="jobStepId"></param>
    /// <param name="workPeriodBuffer"></param>
    /// <param name="punchEntriesHistory">If provided, will create a punch with a current state if a new work period is created.</param>
    /// <param name="save"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<EquipmentsToUser> LinkToUserAsync(Guid equipmentId, Guid userId, Guid actionById, Guid? jobTypeId, Guid? jobStepId, int workPeriodBuffer, 
        DateTime? linkedOn = null, DateTime? linkedOnEffective = null, Guid? workPeriodId = null, PunchEntriesHistory? punchEntriesHistory = null, bool? save = null, CancellationToken token = default)
    {
        SaveType saveType = base.GetSaveType(save);
        Guid newWorkPeriodId = Guid.NewGuid();
        DateTime now = DateTime.Now;
        DateTime effectiveNow = linkedOnEffective ?? now;

        EquipmentsToUser link = new()
        {
            EquipmentId = equipmentId,
            UserId = userId,
            LinkedOn = linkedOn ?? now,
            LinkedOnEffective = effectiveNow,
            LinkedById = actionById,
            WorkPeriodId = newWorkPeriodId,
            JobTypeId = jobTypeId,
            JobStepId = jobStepId
        };

        // need to determine if we need new work period
        // the way to determine if we need work period is to look for a work period who has a punch before current date time using the threshold
        // creating links to equipment should mimic the functionality of creating a punch for a user
        // in that, a work period needs to be assigned at some point, as well as a jobType connection to work period
        // the burden should fall onto the repository and not the user (the caller to the repository), since it is required by punch entry to maintain state correctness
        // in the same way, linking equipment must also maintain a state correctness which corresponds to the existing punch entries, or the one which needs to be created
        // while business logic (in my opinion) should generally not fall into a database/data abstraction layer - in this case we need to handle the basic logic
        // which helps the data maintain a usable state of correctness. this is the only business logic which should be handled by the repository layer.
        // if we only created a work period, how would the caller know? he would have to query the database again, which we are already doing in here to decide if we create a work period
        // so we allow the user to provide a punch, so the caller doesn't have to guess or re-query db in order to decide if a new punch in necessary
        WorkPeriodsRepository workPeriodRepo = new(base.Context);
        DateTime minDate = now.AddHours(workPeriodBuffer * -1);
        DateTime maxDate = now.AddHours(workPeriodBuffer);
        WorkPeriod? workPeriod = null;
        bool needsPunchEntry;

        if (workPeriodId.HasValue)
        {
            workPeriod = await base.Context.WorkPeriods
                .Include(w => w.PunchEntries).ThenInclude(p => p.CurrentState.StablePunchEntriesHistory)
                .FirstOrDefaultAsync(w => w.Id == workPeriodId.Value);
            if (workPeriod is null)
                throw new ArgumentException($"Provided WorkPeriod ID was invalid: {workPeriodId}", nameof(workPeriodId));
        }
        else{
            // need to get work period for this link if it exists. but how? we can use punch entries
            // only a work period which has not been submitted and is for purpose of calculating total hours worked
            workPeriod = await base.Context.WorkPeriods.Where(w =>
                w.UserId == userId &&
                w.PunchEntries.Any(p => p.CurrentState.PunchEntriesHistory.DateTime >= minDate && p.CurrentState.PunchEntriesHistory.DateTime <= maxDate) &&
                w.Purpose == WorkPeriodPurpose.PunchEntriesSum &&
                w.WorkPeriodStatusHistories.OrderByDescending(h => h.RowId).First().Status == WorkPeriodStatus.Pending)
                .OrderByDescending(w => w.RowId)
                .Include(w => w.PunchEntries).ThenInclude(p => p.CurrentState.StablePunchEntriesHistory)
                .FirstOrDefaultAsync();
        }

        if (workPeriod is null)
        {
            needsPunchEntry = true;
            workPeriod = new()
            {
                Id = newWorkPeriodId,
                HoursWorked = 0,
                PayPeriodEnd = DateOnly.FromDateTime(now.Date.NextDayOfWeek(DayOfWeek.Sunday).AddMicroseconds(-1)),
                Purpose = WorkPeriodPurpose.PunchEntriesSum,
                UserId = userId,
                WorkDate = now.Date.ToDateOnly(),
            };

            workPeriod.WorkPeriodStatusHistories.Add(new WorkPeriodStatusHistory()
            {
                DateTime = now,
                Status = WorkPeriodStatus.Pending,
                WorkPeriodId = workPeriod.Id
            });
        }
        else
        {
            needsPunchEntry = workPeriod.PunchEntries.Where(p => p.CurrentState.StablePunchEntriesHistory != null)
                .OrderByDescending(p => p.CurrentState.StablePunchEntriesHistory!.EffectiveDateTime).FirstOrDefault()?.CurrentState.StableStatus == PunchStatus.Out;
        }

        // since we are creating a new work period, we know we do not have any punches. so we need to create one
        if (punchEntriesHistory is not null && needsPunchEntry)
        {
            PunchEntry punchEntry = new() { Id = Guid.NewGuid(), UserId = userId, WorkPeriodId = workPeriod.Id, DeviceId = punchEntriesHistory.DeviceId };
            punchEntriesHistory.PunchEntryId = punchEntry.Id;
            punchEntry.PunchEntriesHistories.Add(punchEntriesHistory);
            PunchEntriesCurrentState currentState = new() { PunchEntry = punchEntry, PunchEntriesHistory = punchEntriesHistory, StablePunchEntriesHistory = punchEntriesHistory };
            punchEntry.CurrentState = currentState;
            workPeriod.PunchEntries.Add(punchEntry);
            await base.Context.PunchEntries.AddAsync(punchEntry);
        }

        link.WorkPeriod = workPeriod;

        await base.Context.EquipmentsToUsers.AddAsync(link);

        WorkPeriodJobTypeStep workPeriodJobType = new()
        {
            ActivatedOn = effectiveNow,
            JobTypeId = jobTypeId,
            JobStepId = jobStepId,
            WorkPeriodId = workPeriod.Id,
            WorkPeriod = workPeriod,
            EquipmentsToUser = link
        };

        await base.Context.WorkPeriodJobTypeSteps.AddAsync(workPeriodJobType);
        workPeriod.WorkPeriodJobTypeSteps.Add(workPeriodJobType);

        if (saveType != SaveType.None)
            await base.SaveAsync();

        return link;
    }
    /// <summary>
    /// </summary>
    /// <param name="equipmentsToUserId">If <paramref name="equipmentsToUserId"/> is null, then <paramref name="equipmentId"/> and <paramref name="userId"/> are used to get the most recent record and update it.</param>
    /// <param name="equipmentId"></param>
    /// <param name="userId"></param>
    /// <param name="save"></param>
    /// <param name="token"></param>
    /// <returns>returns null if saving directly to database, otherwise updated entity is returned</returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<EquipmentsToUser?> UnlinkToUserAsync(Guid? equipmentsToUserId, Guid equipmentId, Guid userId, Guid actionById,
        DateTime? unlinkedOn = null, DateTime? unlinkedOnEffective = null, Guid? jobStepId = null, Guid? jobTypeId = null, bool? save = null, CancellationToken token = default)
    {
        SaveType saveType = base.GetSaveType(save);
        IQueryable<EquipmentsToUser> query = equipmentsToUserId.HasValue 
            ? base.Context.EquipmentsToUsers.Where(t => t.Id == equipmentsToUserId.Value) 
            : base.Context.EquipmentsToUsers
                .Where(q => q.UserId == userId && q.EquipmentId == equipmentId && q.UnLinkedOn == null)
                .OrderByDescending(q => q.LinkedOn);
        EquipmentsToUser? equipmentToUser = null;
        DateTime unlinkedOnValue = unlinkedOn ?? DateTime.Now;
        DateTime unlinkedOnEffectiveValue = unlinkedOnEffective ?? unlinkedOnValue;

        if (saveType == SaveType.SaveToDb)
        {
            await query.ExecuteUpdateAsync(setters => setters
                .SetProperty(q => q.UnLinkedOn, unlinkedOnValue)
                .SetProperty(q => q.UnlinkedById, actionById)
                .SetProperty(q => q.UnLinkedOnEffective, unlinkedOnEffectiveValue), 
                cancellationToken: token);
            equipmentToUser = await query.Include(t => t.WorkPeriod).FirstOrDefaultAsync();
            // NOTE: this will cause an exception if the WorkPeriod does not exist in database
            SqlParameter[] sqlParameters =
                [
                    new($"@{nameof(jobStepId)}", (object?)jobStepId ?? DBNull.Value),
                new($"@{nameof(jobTypeId)}", (object?)jobTypeId ?? DBNull.Value),
                new($"@{nameof(equipmentToUser.WorkPeriodId)}", equipmentToUser.WorkPeriodId),
                new($"@{nameof(unlinkedOnEffectiveValue)}", unlinkedOnEffectiveValue)
                ];
            StringBuilder insertQuery = new($"INSERT INTO {base.Context.WorkPeriodJobTypeSteps.EntityType.GetSchemaQualifiedTableName()} ");
            insertQuery.AppendLine($"({nameof(WorkPeriodJobTypeStep.ActivatedOn)}, {nameof(WorkPeriodJobTypeStep.JobTypeId)}, {nameof(WorkPeriodJobTypeStep.JobStepId)}, {nameof(WorkPeriodJobTypeStep.WorkPeriodId)})");
            insertQuery.AppendLine($"VALUES");
            insertQuery.AppendLine($"(@{nameof(unlinkedOnEffectiveValue)}, @{nameof(jobTypeId)}, @{nameof(jobStepId)}, @{nameof(equipmentToUser.WorkPeriodId)})");

            await base.Context.Database.ExecuteSqlRawAsync(insertQuery.ToString(), sqlParameters, default);
        }
        else
        {
            equipmentToUser = await query.Include(t => t.WorkPeriod).FirstOrDefaultAsync();
            if (equipmentToUser is null)
                throw new ArgumentException("No Equipment link to User found");
            equipmentToUser.UnLinkedOn = unlinkedOnValue;
            equipmentToUser.UnlinkedById = actionById;
            equipmentToUser.UnLinkedOnEffective = unlinkedOnEffectiveValue;

            base.Context.WorkPeriodJobTypeSteps.Add(new()
            {
                JobStepId = jobStepId,
                JobTypeId = jobTypeId,
                WorkPeriodId = equipmentToUser.WorkPeriodId,
                ActivatedOn = unlinkedOnEffectiveValue
            });

            if (saveType == SaveType.SaveToDbSet)
                await base.SaveAsync(token);
            else if (saveType == SaveType.TrackToDbSet)
                base.Context.EquipmentsToUsers.Update(equipmentToUser);
        }

        return equipmentToUser;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="workPeriodId"></param>
    /// <param name="actionById"></param>
    /// <param name="unlinkedOn"></param>
    /// <param name="unlinkedOnEffective"></param>
    /// <param name="jobStepId">
    /// Assigns a JobTypeStep matching the jobStepId and jobTypeId provided, to the WorkPeriod. 
    /// Leave null to not assign a JobTypeStep to the WorkPeriod</param>
    /// <param name="jobTypeId">
    /// Assigns a JobTypeStep matching the jobStepId and jobTypeId provided, to the WorkPeriod. 
    /// Leave null to not assign a JobTypeStep to the WorkPeriod</param>
    /// <param name="save"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<IEnumerable<EquipmentsToUser>> UnlinkToUserByWorkPeriodAsync(Guid workPeriodId, Guid actionById,
        DateTime? unlinkedOn = null, DateTime? unlinkedOnEffective = null, Guid? jobStepId = null, Guid? jobTypeId = null, 
        bool? save = null, CancellationToken token = default)
    {
        SaveType saveType = base.GetSaveType(save);
        IQueryable<EquipmentsToUser> query = base.Context.EquipmentsToUsers.Where(t => t.WorkPeriodId == workPeriodId && t.UnLinkedOn == null);
        DateTime unlinkedOnValue = unlinkedOn ?? DateTime.Now;
        DateTime unlinkedOnEffectiveValue = unlinkedOnEffective ?? unlinkedOnValue;
        Guid userId;

        IEnumerable<EquipmentsToUser> equipmentsToUser = await query.ToListAsync();// Enumerable.Empty<EquipmentsToUser>();

        if (!equipmentsToUser.Any())
            return [];

        if (saveType == SaveType.SaveToDb)
        {
            await query.ExecuteUpdateAsync(setters => setters
                .SetProperty(q => q.UnLinkedOn, unlinkedOnValue)
                .SetProperty(q => q.UnlinkedById, actionById)
                .SetProperty(q => q.UnLinkedOnEffective, unlinkedOnEffectiveValue), 
                cancellationToken: token);
            userId = query.Select(q => q.UserId).FirstOrDefault();
            // NOTE: this will cause an exception if the WorkPeriod does not exist in database
            SqlParameter[] sqlParameters =
                [
                    new($"@{nameof(jobStepId)}", (object?)jobStepId ?? DBNull.Value),
                    new($"@{nameof(jobTypeId)}", (object?)jobTypeId ?? DBNull.Value),
                    new($"@{nameof(workPeriodId)}", workPeriodId),
                    new($"@{nameof(unlinkedOnEffectiveValue)}", unlinkedOnEffectiveValue)
                ];
            StringBuilder insertQuery = new($"INSERT INTO {base.Context.WorkPeriodJobTypeSteps.EntityType.GetSchemaQualifiedTableName()} ");
            insertQuery.AppendLine($"({nameof(WorkPeriodJobTypeStep.ActivatedOn)}, {nameof(WorkPeriodJobTypeStep.JobTypeId)}, {nameof(WorkPeriodJobTypeStep.JobStepId)}, {nameof(WorkPeriodJobTypeStep.WorkPeriodId)})");
            insertQuery.AppendLine($"VALUES");
            insertQuery.AppendLine($"(@{nameof(unlinkedOnEffectiveValue)}, @{nameof(jobTypeId)}, @{nameof(jobStepId)}, @{nameof(workPeriodId)})");

            await base.Context.Database.ExecuteSqlRawAsync(insertQuery.ToString(), sqlParameters, default);
        }
        else
        {

            base.Context.WorkPeriodJobTypeSteps.Add(new()
            {
                JobStepId = jobStepId,
                JobTypeId = jobTypeId,
                WorkPeriodId = workPeriodId,
                ActivatedOn = unlinkedOnEffectiveValue
            });

            if (equipmentsToUser is null)
                return [];
            foreach (var item in equipmentsToUser)
            {
                item.UnLinkedOn = unlinkedOnValue;
                item.UnlinkedById = actionById;
                item.UnLinkedOnEffective = unlinkedOnEffectiveValue;

                if (saveType == SaveType.TrackToDbSet)
                    base.Context.EquipmentsToUsers.Update(item);
            }

            if (saveType == SaveType.SaveToDbSet)
                await base.SaveAsync(token);
            userId = equipmentsToUser.FirstOrDefault()?.UserId ?? default;
        }

        return equipmentsToUser;
    }
}
