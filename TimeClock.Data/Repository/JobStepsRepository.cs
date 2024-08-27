using TimeClock.Data.Models;

namespace TimeClock.Data.Repository;

public interface IJobStepsRepository : IDataRepository<JobStep>
{
}

public class JobStepsRepository : DataRepository<JobStep>, IJobStepsRepository
{
    internal JobStepsRepository(TimeClockContext context)
            : base(context) { }
}