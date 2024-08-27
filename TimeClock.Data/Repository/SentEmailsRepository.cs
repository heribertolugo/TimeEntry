using TimeClock.Data.Models;

namespace TimeClock.Data.Repository;

public interface ISentEmailsRepository : IDataRepository<SentEmail> { }

public class SentEmailsRepository : DataRepository<SentEmail>, ISentEmailsRepository
{
    public SentEmailsRepository(TimeClockContext context) : base(context) { }
}
