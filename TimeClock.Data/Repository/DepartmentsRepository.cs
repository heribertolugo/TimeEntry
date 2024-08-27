using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TimeClock.Data.Helpers;
using TimeClock.Data.Models;

namespace TimeClock.Data.Repository
{
    #region Interface
    public interface IDepartmentsRepository : IDataRepository<Department>
    {
        IQueryable<Department> GetActive(bool isActive = true, ISorting<Department>? sorting = null, bool? tracking = null);
        Task<IEnumerable<Department>> GetActiveAsync(IPaging paging, bool isActive = true, ISorting<Department>? sorting = null, bool? tracking = null, CancellationToken token = default, params Expression<Func<Department, object>>[] includes);
        Department? GetByName(string name, bool? tracking = null);
        Task<Department?> GetByNameAsync(string name, bool? tracking = null, CancellationToken token = default, params Expression<Func<Department, object>>[] includes);
        void AddToLocation(Guid departmentId,Guid locationId, bool? save = null);
        Task AddToLocationAsync(Guid departmentId,Guid locationId, bool? save = null, CancellationToken token = default);
        void DisableDepartmentInLocation(Guid departmentId,Guid locationId, bool? save = null);
        Task DisableDepartmentInLocationAsync(Guid departmentId,Guid locationId, bool? save = null, CancellationToken token = default);
    }
    #endregion Interface

    public class DepartmentsRepository : DataRepository<Department>, IDepartmentsRepository
    {
        internal DepartmentsRepository(TimeClockContext context)
            : base(context) { }

        public Department? GetByName(string name, bool? tracking = null)
        {
            return base.GetTracker(tracking).FirstOrDefault(x => x.Name == name);
        }
        public IQueryable<Department> GetActive(bool isActive = true, ISorting<Department>? sorting = null, bool? tracking = null)
        {
            return base.SortedDbSet(sorting, tracking).Where(x => x.IsActive == isActive);
        }
        public void AddToLocation(Guid departmentId, Guid locationId, bool? save = null)
        {
            DepartmentsToLocation? link = base.Context.DepartmentsToLocations.FirstOrDefault(d => d.DepartmentId == departmentId && d.LocationId == locationId);
            SaveType saveType = base.GetSaveType(save);

            if (link is not null)
                link.IsActive = true;
            else
                base.Context.DepartmentsToLocations.Add(new DepartmentsToLocation() { DepartmentId = departmentId, LocationId = locationId });
            
            if (saveType != SaveType.None) base.Save();
        }
        public void DisableDepartmentInLocation(Guid departmentId,Guid locationId, bool? save = null)
        {
            DepartmentsToLocation? link = base.Context.DepartmentsToLocations.FirstOrDefault(d => d.DepartmentId == departmentId && d.LocationId == locationId);
#warning MUST IMPLEMENT
            if (link is null)
                return;
            link.IsActive = false;
            //if (save) base.Save();
        }
        public Task<Department?> GetByNameAsync(string name, bool? tracking = null, CancellationToken token = default, params Expression<Func<Department, object>>[] includes)
        {
            return base.GetTracker(tracking).IncludeMultiple(includes).FirstOrDefaultAsync(x => x.Name == name, token);
        }
        public async Task<IEnumerable<Department>> GetActiveAsync(IPaging paging, bool isActive = true, ISorting<Department>? sorting = null, bool? tracking = null, CancellationToken token = default, params Expression<Func<Department, object>>[] includes)
        {
            return await base.SortedDbSet(sorting, tracking).IncludeMultiple(includes).Where(x => x.IsActive == isActive).Skip(paging.PageSize * (paging.PageNumber - 1)).Take(paging.PageSize).ToListAsync(token);
        }
        public async Task AddToLocationAsync(Guid departmentId, Guid locationId, bool? save = null, CancellationToken token = default)
        {
            DepartmentsToLocation? link = await base.Context.DepartmentsToLocations.FirstOrDefaultAsync(d => d.DepartmentId == departmentId && d.LocationId == locationId);

            if (link is not null)
                link.IsActive = true;
            else
                await base.Context.DepartmentsToLocations.AddAsync(new DepartmentsToLocation() { DepartmentId = departmentId, LocationId = locationId });
#warning MUST IMPLEMENT
            //if (save) await base.SaveAsync();
        }
        public async Task DisableDepartmentInLocationAsync(Guid departmentId,Guid locationId, bool? save = null, CancellationToken token = default)
        {
            DepartmentsToLocation? link = await base.Context.DepartmentsToLocations.FirstOrDefaultAsync(d => d.DepartmentId == departmentId && d.LocationId == locationId);

            if (link is null)
                return;
            link.IsActive = false;
#warning MUST IMPLEMENT
            //if (save) await base.SaveAsync();
        }
    }
}
