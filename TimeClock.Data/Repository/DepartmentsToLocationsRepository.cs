using TimeClock.Data.Models;

namespace TimeClock.Data.Repository
{
    public interface IDepartmentsToLocationsRepository : IDataRepository<DepartmentsToLocation> { }

    public class DepartmentsToLocationsRepository : DataRepository<DepartmentsToLocation>, IDepartmentsToLocationsRepository
    {
        public DepartmentsToLocationsRepository(TimeClockContext context) : base(context) { }
    }
}
