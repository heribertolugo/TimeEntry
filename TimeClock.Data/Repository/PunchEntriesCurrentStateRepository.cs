using TimeClock.Data.Models;

namespace TimeClock.Data.Repository
{
    public interface IPunchEntriesCurrentStateRepository
    {
    }

    public class PunchEntriesCurrentStateRepository : DataRepository<PunchEntriesCurrentState>, IPunchEntriesCurrentStateRepository
    {
        internal PunchEntriesCurrentStateRepository(TimeClockContext context)
            : base(context) { }
    }
}
