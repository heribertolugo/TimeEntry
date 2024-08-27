using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Text;
using TimeClock.Data.Models;

namespace TimeClock.Data.Repository
{
    public interface IEventAuditRepository : IDataRepository<EventAudit>
    {
        int GetByEntityForDateCount(Guid entityId, DateTime date, bool? success = false);
        Task<int> GetByEntityForDateCountAsync(Guid entityId, DateTime date, bool? success = false);
        Task InsertDirectlyToDbAsync(params EventAudit[] eventAudits);
    }

    public class EventAuditRepository : DataRepository<EventAudit>, IEventAuditRepository
    {
        internal EventAuditRepository(TimeClockContext context) : base(context) { }

        public new bool UseTracker { get => false; }

        public int GetByEntityForDateCount(Guid entityId, DateTime date, bool? success = false)
        {
            var query = base.Context.EventAudits.Where(a => a.EventDate.Date == date.Date && a.EntityId == entityId);

            if (success.HasValue)
                query.Where(a => a.Success == success.Value);

            return query.Count();
        }
        public Task<int> GetByEntityForDateCountAsync(Guid entityId, DateTime date, bool? success = false)
        {
            var query = base.Context.EventAudits.Where(a => a.EventDate.Date == date.Date && a.EntityId == entityId);

            if (success.HasValue)
                query.Where(a => a.Success == success.Value);

            return query.CountAsync();
        }
        public Task InsertDirectlyToDbAsync(params EventAudit[] eventAudits)
        {
            List<SqlParameter> sqlParameters = new();
            StringBuilder query = new($"INSERT INTO {base.Context.Set<EventAudit>().EntityType.GetSchemaQualifiedTableName()} ");
            query.AppendLine($"({nameof(EventAudit.Id)}, {nameof(EventAudit.EntityId)}, {nameof(EventAudit.EntityType)}, {nameof(EventAudit.EventDate)}, {nameof(EventAudit.EventDescription)}, {nameof(EventAudit.EventId)}, {nameof(EventAudit.EventName)}, {nameof(EventAudit.Success)})");
            query.AppendLine("VALUES");

            for (int index = 0;  index < eventAudits.Length; index++)
            {
                sqlParameters.Add(new($"@{nameof(EventAudit.Id)}{index}", eventAudits[index].Id));
                sqlParameters.Add(new($"@{nameof(EventAudit.EntityId)}{index}", eventAudits[index].EntityId));
                sqlParameters.Add(new($"@{nameof(EventAudit.EntityType)}{index}", eventAudits[index].EntityType));
                sqlParameters.Add(new($"@{nameof(EventAudit.EventDate)}{index}", eventAudits[index].EventDate));
                sqlParameters.Add(new($"@{nameof(EventAudit.EventDescription)}{index}", eventAudits[index].EventDescription));
                sqlParameters.Add(new($"@{nameof(EventAudit.EventId)}{index}", eventAudits[index].EventId));
                sqlParameters.Add(new($"@{nameof(EventAudit.EventName)}{index}", eventAudits[index].EventName));
                sqlParameters.Add(new($"@{nameof(EventAudit.Success)}{index}", eventAudits[index].Success));

                query.AppendLine($@"(
                    @{nameof(EventAudit.Id)}{index}, @{nameof(EventAudit.EntityId)}{index}, @{nameof(EventAudit.EntityType)}{index}, 
                    @{nameof(EventAudit.EventDate)}{index}, @{nameof(EventAudit.EventDescription)}{index}, @{nameof(EventAudit.EventId)}{index}, 
                    @{nameof(EventAudit.EventName)}{index}, @{nameof(EventAudit.Success)}{index}){(index < eventAudits.Length - 1 ? ", " : "")}");
            }

            return base.Context.Database.ExecuteSqlRawAsync(query.ToString(), sqlParameters.ToArray());
        }
    }
}
