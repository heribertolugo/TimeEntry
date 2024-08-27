using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TimeClock.Data.Helpers;
using TimeClock.Data.Models;

namespace TimeClock.Data.Repository
{
    public interface IEquipmentTypesRepository : IDataRepository<EquipmentType>
    {
        IQueryable<EquipmentType> GetActive(bool isActive = true, ISorting<EquipmentType>? sorting = null, bool? tracking = null);
        Task<IEnumerable<EquipmentType>> GetActiveAsync(bool isActive = true, ISorting<EquipmentType>? sorting = null, bool? tracking = null, CancellationToken token = default, params Expression<Func<EquipmentType, object>>[] includes);
        IQueryable<EquipmentType?> GetByName(string name, ISorting<EquipmentType>? sorting = null, bool? tracking = null);
        Task<IEnumerable<EquipmentType?>> GetByNameAsync(string name, ISorting<EquipmentType>? sorting = null, bool? tracking = null, CancellationToken token = default, params Expression<Func<EquipmentType, object>>[] includes);
    }

    internal class EquipmentTypesRepository : DataRepository<EquipmentType>, IEquipmentTypesRepository
    {
        internal EquipmentTypesRepository(TimeClockContext context)
            : base(context) { }

        public IQueryable<EquipmentType?> GetByName(string name, ISorting<EquipmentType>? sorting = null, bool? tracking = null)
        {
            return base.SortedDbSet(sorting, tracking).Where(x => x.Name == name);
        }
        public IQueryable<EquipmentType> GetActive(bool isActive = true, ISorting<EquipmentType>? sorting = null, bool? tracking = null)
        {
            return base.SortedDbSet(sorting, tracking).Where(x => x.IsActive == isActive);
        }
        public async Task<IEnumerable<EquipmentType?>> GetByNameAsync(string name, ISorting<EquipmentType>? sorting = null, bool? tracking = null, CancellationToken token = default, params Expression<Func<EquipmentType, object>>[] includes)
        {
            return await base.SortedDbSet(sorting, tracking).IncludeMultiple(includes).Where(x => x.Name == name).ToListAsync(token);
        }
        public async Task<IEnumerable<EquipmentType>> GetActiveAsync(bool isActive = true, ISorting<EquipmentType>? sorting = null, bool? tracking = null, CancellationToken token = default, params Expression<Func<EquipmentType, object>>[] includes)
        {
            return await base.SortedDbSet(sorting, tracking).IncludeMultiple(includes).Where(x => x.IsActive == isActive).ToListAsync(token);
        }
    }
}
