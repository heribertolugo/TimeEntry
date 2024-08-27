using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TimeClock.Data.Helpers;
using TimeClock.Data.Models;

namespace TimeClock.Data.Repository;

public interface IWorkPeriodsRepository : IDataRepository<WorkPeriod>
{
    Task<IEnumerable<WorkPeriod>> GetWorkPeriodsWithPunchesAsync(Expression<Func<WorkPeriod, bool>> predicate, IEnumerable<PunchAction>? punchActions = null, ISorting<WorkPeriod>? sorting = null, bool? tracking = null, CancellationToken token = default, params Expression<Func<WorkPeriod, object>>[] includes);
    //Task<IEnumerable<WorkPeriod>> GetPunchTypeForUserByDateRangeAndTypeAsync(Guid userId, DateTime fromDate, DateTime toDate, Guid? jobTypeId, bool? tracking = null, CancellationToken token = default, params Expression<Func<WorkPeriod, object>>[] includes);
    Task<IEnumerable<WorkPeriod>> GetPunchTypeForUserByDateRangeAsync(Guid userId, DateTime fromDate, DateTime toDate, bool? tracking = null, CancellationToken token = default, params Expression<Func<WorkPeriod, object>>[] includes);
    Task<WorkPeriod?> GetLatestPendingForUserAsync(Guid userId, bool? tracking = null, CancellationToken token = default, params Expression<Func<WorkPeriod, object>>[] includes);
    //Task<IEnumerable<WorkPeriod>> GetPendingForUserByDateRangeAndTypeAsync(Guid userId, DateTime fromDate, DateTime toDate, Guid? jobTypeId, bool? tracking = null, CancellationToken token = default, params Expression<Func<WorkPeriod, object>>[] includes);
    Task<IEnumerable<WorkPeriod>> GetPendingForUserByDateRangeAsync(Guid userId, DateTime fromDate, DateTime toDate, bool? tracking = null, CancellationToken token = default, params Expression<Func<WorkPeriod, object>>[] includes);
    //Task<WorkPeriod?> GetLatestPendingForUserByDateAndTypeAsync(Guid userId, DateTime dateTime, Guid? jobTypeId, bool? tracking = null, CancellationToken token = default, params Expression<Func<WorkPeriod, object>>[] includes);
    Task<WorkPeriod?> GetLatestPendingForUserByDateAsync(Guid userId, DateTime dateTime, bool? tracking = null, CancellationToken token = default, params Expression<Func<WorkPeriod, object>>[] includes);
    Task<IEnumerable<WorkPeriod>> GetPreviousIfMissingPunchAsync(Guid workPeriodId, bool? tracking = null, CancellationToken token = default, params Expression<Func<WorkPeriod, object>>[] includes);
    Task<IEnumerable<WorkPeriod>> GetPreviousIfMissingPunchAsync(DateOnly payPeriodEnd, bool? tracking = null, CancellationToken token = default, params Expression<Func<WorkPeriod, object>>[] includes);
}

public class WorkPeriodsRepository : DataRepository<WorkPeriod>, IWorkPeriodsRepository
{
    public WorkPeriodsRepository(TimeClockContext context)
        : base(context) { }

    public async Task<IEnumerable<WorkPeriod>> GetWorkPeriodsWithPunchesAsync(Expression<Func<WorkPeriod, bool>> predicate, IEnumerable<PunchAction>? punchActions = null, ISorting<WorkPeriod>? sorting = null, bool? tracking = null, CancellationToken token = default, params Expression<Func<WorkPeriod, object>>[] includes)
    {
        IQueryable<WorkPeriod> query = this.SortedDbSet(sorting, tracking).IncludeMultiple(includes).Where(predicate);
        
        if (punchActions is not null) 
        {
            query = query.Include(w => (w as WorkPeriod).PunchEntries.Where(p => punchActions.Any(a => a == p.CurrentState.StablePunchEntriesHistory.Action)))
                .ThenInclude(p => p.CurrentState).ThenInclude(s => s.PunchEntriesHistory);
            //query = query.Where(w => w.PunchEntries.Any(p => punchActions.Any(a => a == p.CurrentState.StablePunchEntriesHistory.Action)));
        }
        else
        {
            query = query.Include(w => (w as WorkPeriod).PunchEntries).ThenInclude(p => p.CurrentState).ThenInclude(s => s.StablePunchEntriesHistory);
        }
        query = query.AsSplitQuery();
        return await query.ToListAsync(token);
    }

    public async Task<IEnumerable<WorkPeriod>> GetPunchTypeForUserByDateRangeAsync(Guid userId, DateTime fromDate, DateTime toDate, bool? tracking = null, CancellationToken token = default, params Expression<Func<WorkPeriod, object>>[] includes)
    {
        return await base.GetTracker(tracking).IncludeMultiple(includes)
            .Where(w => w.UserId == userId && w.WorkDate >= fromDate.Date.ToDateOnly() && w.WorkDate <= toDate.Date.ToDateOnly()
                && w.Purpose == WorkPeriodPurpose.PunchEntriesSum)
            .ToListAsync(token);
    }
    //public async Task<IEnumerable<WorkPeriod>> GetPunchTypeForUserByDateRangeAndTypeAsync(Guid userId, DateTime fromDate, DateTime toDate, Guid? jobTypeId, bool? tracking = null, CancellationToken token = default, params Expression<Func<WorkPeriod, object>>[] includes)
    //{
    //    return await base.GetTracker(tracking).IncludeMultiple(includes)
    //        .Where(w => w.UserId == userId && w.WorkDate.Date >= fromDate.Date && w.WorkDate.Date <= toDate.Date && w.JobTypeId == jobTypeId
    //            && w.Purpose == WorkPeriodPurpose.PunchEntriesSum)
    //        .ToListAsync(token);
    //}

    public Task<WorkPeriod?> GetLatestPendingForUserAsync(Guid userId, bool? tracking = null, CancellationToken token = default, params Expression<Func<WorkPeriod, object>>[] includes)
    {
        return base.GetTracker(tracking).IncludeMultiple(includes).OrderByDescending(w => w.RowId)
            .Where(w =>
                w.WorkPeriodStatusHistories.OrderByDescending(h => h.RowId).First().Status == WorkPeriodStatus.Pending &&
                w.Purpose == WorkPeriodPurpose.PunchEntriesSum &&
                w.UserId == userId)
            .OrderByDescending(w => w.RowId)
            .FirstOrDefaultAsync(token);
    }

    public Task<WorkPeriod?> GetLatestPendingForUserByDateAsync(Guid userId, DateTime dateTime, bool? tracking = null, CancellationToken token = default, params Expression<Func<WorkPeriod, object>>[] includes)
    {
        return base.GetTracker(tracking).IncludeMultiple(includes).OrderByDescending(w => w.RowId)
            .Where(w => 
                w.WorkPeriodStatusHistories.OrderByDescending(h => h.RowId).First().Status == WorkPeriodStatus.Pending && 
                w.Purpose == WorkPeriodPurpose.PunchEntriesSum &&
                w.UserId == userId && 
                w.WorkDate == dateTime.Date.ToDateOnly())
            .FirstOrDefaultAsync( token);
    }
    //public Task<WorkPeriod?> GetLatestPendingForUserByDateAndTypeAsync(Guid userId, DateTime dateTime, Guid? jobTypeId, bool? tracking = null, CancellationToken token = default, params Expression<Func<WorkPeriod, object>>[] includes)
    //{
    //    return base.GetTracker(tracking).IncludeMultiple(includes).OrderByDescending(w => w.RowId)
    //        .Where(w =>  w.WorkPeriodStatusHistories.OrderByDescending(h => h.RowId).First().Status == WorkPeriodStatus.Pending && w.Purpose == WorkPeriodPurpose.PunchEntriesSum)
    //        .FirstOrDefaultAsync(w => w.UserId == userId && w.WorkDate.Date == dateTime.Date && w.JobTypeId == jobTypeId, token);
    //}

    public async Task<IEnumerable<WorkPeriod>> GetPendingForUserByDateRangeAsync(Guid userId, DateTime fromDate, DateTime toDate, bool? tracking = null, CancellationToken token = default, params Expression<Func<WorkPeriod, object>>[] includes)
    {
        return await base.GetTracker(tracking).IncludeMultiple(includes)
            .Where(w => w.UserId == userId && w.WorkDate >= fromDate.Date.ToDateOnly() && w.WorkDate <= toDate.Date.ToDateOnly() 
                && w.WorkPeriodStatusHistories.OrderByDescending(h => h.RowId).First().Status == WorkPeriodStatus.Pending && w.Purpose == WorkPeriodPurpose.PunchEntriesSum)
            .ToListAsync(token);
    }
    //public async Task<IEnumerable<WorkPeriod>> GetPendingForUserByDateRangeAndTypeAsync(Guid userId, DateTime fromDate, DateTime toDate, Guid? jobTypeId, bool? tracking = null, CancellationToken token = default, params Expression<Func<WorkPeriod, object>>[] includes)
    //{
    //    return await base.GetTracker(tracking).IncludeMultiple(includes)
    //        .Where(w => w.UserId == userId && w.WorkDate.Date >= fromDate.Date && w.WorkDate.Date <= toDate.Date && w.JobTypeId == jobTypeId
    //            && w.WorkPeriodStatusHistories.OrderByDescending(h => h.RowId).First().Status == WorkPeriodStatus.Pending && w.Purpose == WorkPeriodPurpose.PunchEntriesSum)
    //        .ToListAsync(token);
    //}

    public async Task<IEnumerable<WorkPeriod>> GetPreviousIfMissingPunchAsync(Guid workPeriodId, bool? tracking = null, CancellationToken token = default, params Expression<Func<WorkPeriod, object>>[] includes)
    {
        int? rowId = workPeriodId == default ? int.MaxValue : await base.Context.WorkPeriods.Where(w => w.Id == workPeriodId).Select(w => w.RowId).FirstOrDefaultAsync();
        var query = base.GetTracker(tracking).IncludeMultiple(includes)
            .OrderByDescending(w => w.RowId)
            .Where(w => w.Purpose == WorkPeriodPurpose.PunchEntriesSum && w.RowId < rowId && w.PunchEntries.Count % 2 != 0);
        if (includes.Length > 1)
            query = query.AsSplitQuery();
        return await query.ToListAsync();
    }
    public async Task<IEnumerable<WorkPeriod>> GetPreviousIfMissingPunchAsync(DateOnly payPeriodEnd, bool? tracking = null, CancellationToken token = default, params Expression<Func<WorkPeriod, object>>[] includes)
    {
        var query = base.GetTracker(tracking).IncludeMultiple(includes)
            .OrderByDescending(w => w.RowId)
            .Where(w => w.Purpose == WorkPeriodPurpose.PunchEntriesSum && w.PayPeriodEnd == payPeriodEnd && w.PunchEntries.Count % 2 != 0);
        if (includes.Length > 1)
            query = query.AsSplitQuery();
        return await query.ToListAsync();
    }
}
