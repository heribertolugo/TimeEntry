using TimeClock.Data.Models;

namespace TimeClock.Data.Repository;

public interface IJobTypesRepository : IDataRepository<JobType>
{
}

public class JobTypesRepository : DataRepository<JobType>, IJobTypesRepository
{
    internal JobTypesRepository(TimeClockContext context)
            : base(context) { }
}
