using Microsoft.EntityFrameworkCore;
using TimeClock.Data.Models;

namespace TimeClock.Data.Repository;

public interface IJobTypeStepToUserRepository : IDataRepository<JobTypeStepToUser>
{
    Task<int> GetCountForUserAsync(Guid userId);
}

public class JobTypeStepToUserRepository : DataRepository<JobTypeStepToUser>, IJobTypeStepToUserRepository
{
    internal JobTypeStepToUserRepository(TimeClockContext context)
            : base(context) { }

    public async Task<int> GetCountForUserAsync(Guid userId)
    {
        return await this.Context.JobTypeStepsToUsers.CountAsync(j => j.UserId == userId);
    }
}
