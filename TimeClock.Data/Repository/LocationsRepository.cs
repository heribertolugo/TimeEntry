using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TimeClock.Data.Helpers;
using TimeClock.Data.Models;

namespace TimeClock.Data.Repository
{
    public interface ILocationsRepository : IDataRepository<Location>
    {
        IQueryable<Location> GetActive(bool isActive = true, ISorting<Location>? sorting = null, bool? tracking = null);
        Task<IEnumerable<Location>> GetActiveAsync(bool isActive = true, ISorting<Location>? sorting = null, bool? tracking = null, CancellationToken token = default, params Expression<Func<Location, object>>[] includes);
        IQueryable<Location?> GetByName(string name, ISorting<Location>? sorting = null, bool? tracking = null);
        Task<IEnumerable<Location?>> GetByNameAsync(string name, ISorting<Location>? sorting = null, bool? tracking = null, CancellationToken token = default, params Expression<Func<Location, object>>[] includes);
    }

    public class LocationsRepository : DataRepository<Location>, ILocationsRepository
    {
        internal LocationsRepository(TimeClockContext context)
            : base(context) { }

        public IQueryable<Location?> GetByName(string name, ISorting<Location>? sorting = null, bool? tracking = null)
        {
            return base.SortedDbSet(sorting, tracking).Where(x => x.Name == name);
        }
        public IQueryable<Location> GetActive(bool isActive = true, ISorting<Location>? sorting = null, bool? tracking = null)
        {
            return base.SortedDbSet(sorting, tracking).Where(x => x.IsActive == isActive);
        }
        public async Task<IEnumerable<Location?>> GetByNameAsync(string name, ISorting<Location>? sorting = null, bool? tracking = null, CancellationToken token = default, params Expression<Func<Location, object>>[] includes)
        {
            return await base.SortedDbSet(sorting, tracking).IncludeMultiple(includes).Where(x => x.Name == name).ToListAsync(token);
        }
        public async Task<IEnumerable<Location>> GetActiveAsync(bool isActive = true, ISorting<Location>? sorting = null, bool? tracking = null, CancellationToken token = default, params Expression<Func<Location, object>>[] includes)
        {
            return await base.SortedDbSet(sorting, tracking).IncludeMultiple(includes).Where(x => x.IsActive == isActive).ToListAsync(token);
        }
    }
}
