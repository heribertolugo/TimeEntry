using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TimeClock.Data.Helpers;
using TimeClock.Data.Models;
using Device = TimeClock.Data.Models.Device;

namespace TimeClock.Data.Repository
{
    public interface IDevicesRepository : IDataRepository<Device>
    {
        IQueryable<Device> GetActive(bool isActive = true, ISorting<Device>? sorting = null, bool? tracking = null);
        Task<IEnumerable<Device>> GetActiveAsync(IPaging? paging = null, bool isActive = true, ISorting<Device>? sorting = null, bool? tracking = null, CancellationToken token = default, params Expression<Func<Device, object>>[] includes);
        IQueryable<Device> GetByDepartment(Guid departmentId, ISorting<Device>? sorting = null, bool? tracking = null);
        Task<IEnumerable<Device>> GetByDepartmentAsync(Guid departmentId, ISorting<Device>? sorting = null, bool? tracking = null, CancellationToken token = default, params Expression<Func<Device, object>>[] includes);
        IQueryable<Device> GetByLocation(Guid locationId, ISorting<Device>? sorting = null, bool? tracking = null);
        Task<IEnumerable<Device>> GetByLocationAsync(Guid locationId, ISorting<Device>? sorting = null, bool? tracking = null, CancellationToken token = default, params Expression<Func<Device, object>>[] includes);
        IQueryable<Device> GetByLocationDepartment(Guid locationId, Guid departmentId, ISorting<Device>? sorting = null, bool? tracking = null);
        Task<IEnumerable<Device>> GetByLocationDepartmentAsync(Guid locationId, Guid departmentId, ISorting<Device>? sorting = null, bool? tracking = null, CancellationToken token = default, params Expression<Func<Device, object>>[] includes);
        Device? GetByUid(string uid, bool? tracking = null, params Expression<Func<Device, object>>[] includes);
        Task<Device?> GetByUidAsync(string uid, bool? tracking = null, CancellationToken token = default, params Expression<Func<Device, object>>[] includes);
        void AddToDepartmentLocation(Device item, Guid departmentId, Guid locationId, bool? save = null);
        Task AddToDepartmentLocationAsync(Device item, Guid departmentId, Guid locationId, bool? save = null, CancellationToken token = default);
    }

    public class DevicesRepository : DataRepository<Device>, IDevicesRepository
    {
        internal DevicesRepository(TimeClockContext context)
            : base(context) { }

        public override void Add(Device item, bool? save = null)
        {
            if (item.DepartmentsToLocationsId == Guid.Empty)
                throw new ArgumentException("No value specified for department and location");

            base.Add(item, save);
        }
        /// <summary>
        /// Adds a Device to a Department Location. Note the Device must already exist.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="departmentId"></param>
        /// <param name="locationId"></param>
        /// <param name="save"></param>
        /// <exception cref="ArgumentException"></exception>
        public void AddToDepartmentLocation(Device item, Guid departmentId, Guid locationId, bool? save = null)
        {
            DepartmentsToLocation? departmentLocation = base.Context.DepartmentsToLocations
                .FirstOrDefault(d => d.DepartmentId == departmentId && d.LocationId == locationId);
            SaveType saveType = base.GetSaveType(save);
            if (departmentLocation == null)
                throw new ArgumentException("No such department and location exists");

            if (saveType == SaveType.SaveToDb)
            {
                base.Context.Devices.Where(d => d.Id == item.Id).ExecuteUpdate(setters =>
                    setters.SetProperty(d => d.DepartmentsToLocationsId, departmentLocation.Id));
            }
            else
            {
                item.DepartmentsToLocations = departmentLocation;

                if (saveType == SaveType.SaveToDbSet)
                    base.Save();
                else if (saveType == SaveType.TrackToDbSet)
                    base.Context.Devices.Update(item);
            }
        }
        public Device? GetByUid(string uid, bool? tracking = null, params Expression<Func<Device, object>>[] includes)
        {
            return base.GetTracker(tracking).IncludeMultiple(includes).FirstOrDefault(x => x.Name == uid);
        }
        public IQueryable<Device> GetActive(bool isActive = true, ISorting<Device>? sorting = null, bool? tracking = null)
        {
            return base.SortedDbSet(sorting, tracking).Where(x => x.IsActive == isActive);
        }
        public IQueryable<Device> GetByLocation(Guid locationId, ISorting<Device>? sorting = null, bool? tracking = null)
        {
            return base.SortedDbSet(sorting, tracking).Where(x => x.DepartmentsToLocations.LocationId == locationId).Select(x => x);
        }
        public IQueryable<Device> GetByDepartment(Guid departmentId, ISorting<Device>? sorting = null, bool? tracking = null)
        {
            return base.SortedDbSet(sorting, tracking).Where(x => x.DepartmentsToLocations.DepartmentId == departmentId).Select(x => x);
        }
        public IQueryable<Device> GetByLocationDepartment(Guid locationId, Guid departmentId, ISorting<Device>? sorting = null, bool? tracking = null)
        {
            return base.SortedDbSet(sorting, tracking)
                .Where(x => x.DepartmentsToLocations.DepartmentId == departmentId && x.DepartmentsToLocations.LocationId == locationId)
                .Select(x => x);
        }
        public Task<Device?> GetByUidAsync(string uid, bool? tracking = null, CancellationToken token = default, params Expression<Func<Device, object>>[] includes)
        {
            return base.GetTracker(tracking).IncludeMultiple(includes).FirstOrDefaultAsync(x => x.Name == uid, token);
        }
        public async Task<IEnumerable<Device>> GetActiveAsync(IPaging? paging = null, bool isActive = true, ISorting<Device>? sorting = null, bool? tracking = null, CancellationToken token = default, params Expression<Func<Device, object>>[] includes)
        {
            IQueryable<Device> query = base.SortedDbSet(sorting, tracking).IncludeMultiple(includes).Where(x => x.IsActive == isActive);

            if (paging != null)
                query= query.Skip(paging.PageSize * (paging.PageNumber - 1)).Take(paging.PageSize);

            return await query.ToListAsync(token);
        }
        public async Task<IEnumerable<Device>> GetByLocationAsync(Guid locationId, ISorting<Device>? sorting = null, bool? tracking = null, CancellationToken token = default, params Expression<Func<Device, object>>[] includes)
        {
            return await base.SortedDbSet(sorting, tracking).IncludeMultiple(includes).Where(x => x.DepartmentsToLocations.LocationId == locationId).Select(x => x).ToListAsync();
        }
        public async Task<IEnumerable<Device>> GetByDepartmentAsync(Guid departmentId, ISorting<Device>? sorting = null, bool? tracking = null, CancellationToken token = default, params Expression<Func<Device, object>>[] includes)
        {
            return await base.SortedDbSet(sorting, tracking).IncludeMultiple(includes).Where(x => x.DepartmentsToLocations.DepartmentId == departmentId).Select(x => x).ToListAsync();
        }
        public async Task<IEnumerable<Device>> GetByLocationDepartmentAsync(Guid locationId, Guid departmentId, ISorting<Device>? sorting = null, bool? tracking = null, CancellationToken token = default, params Expression<Func<Device, object>>[] includes)
        {
            return await base.SortedDbSet(sorting, tracking).IncludeMultiple(includes)
                .Where(x => x.DepartmentsToLocations.DepartmentId == departmentId && x.DepartmentsToLocations.LocationId == locationId)
                .Select(x => x).ToListAsync();
        }
        public override Task AddAsync(Device item, bool? save = null, CancellationToken token = default)
        {
            if (item.DepartmentsToLocationsId == Guid.Empty)
                throw new ArgumentException("No value specified for department and location");
            return base.AddAsync(item, save);
        }
        public async Task AddToDepartmentLocationAsync(Device item, Guid departmentId, Guid locationId, bool? save = null, CancellationToken token = default)
        {
            DepartmentsToLocation? departmentLocation = await base.Context.DepartmentsToLocations
                .FirstOrDefaultAsync(d => d.DepartmentId == departmentId && d.LocationId == locationId);
            SaveType saveType = base.GetSaveType(save);
            if (departmentLocation == null)
                throw new ArgumentException("No such department and location exists");

            if (saveType == SaveType.SaveToDb)
            {
                await base.Context.Devices.Where(d => d.Id == item.Id).ExecuteUpdateAsync(setters =>
                    setters.SetProperty(d => d.DepartmentsToLocationsId, departmentLocation.Id));
            }
            else
            {
                item.DepartmentsToLocations = departmentLocation;

                if (saveType == SaveType.SaveToDbSet)
                    await base.SaveAsync();
                else if (saveType == SaveType.TrackToDbSet)
                    base.Context.Devices.Update(item);
            }
        }
    }
}
