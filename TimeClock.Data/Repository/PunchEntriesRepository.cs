using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TimeClock.Data.Helpers;
using TimeClock.Data.Models;

namespace TimeClock.Data.Repository;

public interface IPunchEntriesRepository : IDataRepository<PunchEntry>
{
    [Obsolete("Use overload which accepts jobTypeId and JobStepId instead")]
    new void Add(PunchEntry item, bool? save = null);
    void Add(PunchEntry item, int workPeriodThreshold, Guid? jobTypeId, Guid? jobStepId, bool? save = null);
    void Add(Guid userId, PunchEntriesHistory history, int workPeriodThreshold, Guid? jobTypeId, Guid? jobStepId, bool? save = null);
    void Add(PunchEntry item, PunchEntriesHistory history, int workPeriodThreshold, Guid? jobTypeId, Guid? jobStepId, bool? save = null);
    [Obsolete("Use overload which accepts jobTypeId and JobStepId instead")]
    new Task AddAsync(PunchEntry item, bool? save = null, CancellationToken token = default);
    Task AddAsync(PunchEntry item, int workPeriodThreshold, Guid? jobTypeId, Guid? jobStepId, bool? save = null, CancellationToken token = default);
    Task AddAsync(Guid userId, PunchEntriesHistory history, int workPeriodThreshold, Guid? jobTypeId, Guid? jobStepId, bool? save = null, CancellationToken token = default);
    Task AddAsync(PunchEntry item, PunchEntriesHistory history, int workPeriodThreshold, Guid? jobTypeId, Guid? jobStepId, bool? save = null, CancellationToken token = default);
    void AddHistory(PunchEntriesHistory history, int workPeriodThreshold, Guid? jobTypeId, Guid? jobStepId, bool? save = null);
    Task AddHistoryAsync(PunchEntriesHistory history, int workPeriodThreshold, Guid? jobTypeId, Guid? jobStepId, bool? save = null, CancellationToken token = default);
    PunchEntry? GetByHistoryId(Guid id, bool? tracking = null, params Expression<Func<PunchEntry, object>>[] includes);
    Task<PunchEntry?> GetByHistoryIdAsync(Guid id, bool? tracking = null, CancellationToken token = default, params Expression<Func<PunchEntry, object>>[] includes);
    IQueryable<PunchEntry> GetByDates(DateTime from, DateTime to, ISorting<PunchEntry>? sorting = null, bool? tracking = null);
    Task<IEnumerable<PunchEntry>> GetByDatesAsync(DateTime from, DateTime to, ISorting<PunchEntry>? sorting = null, bool? tracking = null, CancellationToken token = default, params Expression<Func<PunchEntry, object>>[] includes);
    PunchEntry? GetCurrentStateById(Guid id, bool? tracking = null, params Expression<Func<PunchEntry, object>>[] includes);
    Task<PunchEntry?> GetCurrentStateByIdAsync(Guid id, bool? tracking = null, CancellationToken token = default, params Expression<Func<PunchEntry, object>>[] includes);
    IQueryable<PunchEntry?> GetCurrentStates(DateTime from, DateTime to, ISorting<PunchEntry>? sorting = null, bool? tracking = null);
    Task<IEnumerable<PunchEntry?>> GetCurrentStatesAsync(DateTime from, DateTime to, ISorting<PunchEntry>? sorting = null, bool? tracking = null, CancellationToken token = default, params Expression<Func<PunchEntry, object>>[] includes);
    IQueryable<PunchEntry?> GetCurrentStatesForUser(Guid userId, DateTime from, DateTime to, ISorting<PunchEntry>? sorting = null, bool? tracking = null);
    Task<IEnumerable<PunchEntry?>> GetCurrentStatesForUserAsync(Guid userId, DateTime from, DateTime to, ISorting<PunchEntry>? sorting = null, bool? tracking = null, CancellationToken token = default, params Expression<Func<PunchEntry, object>>[] includes);
    IQueryable<PunchEntry?> GetWithHistory(DateTime from, DateTime to, ISorting<PunchEntry>? sorting = null, bool? tracking = null);
    Task<IEnumerable<PunchEntry?>> GetWithHistoryAsync(DateTime from, DateTime to, ISorting<PunchEntry>? sorting = null, bool? tracking = null, CancellationToken token = default, params Expression<Func<PunchEntry, object>>[] includes);
    PunchEntry? GetWithHistoryById(Guid id, bool? tracking = null, params Expression<Func<PunchEntry, object>>[] includes);
    Task<PunchEntry?> GetWithHistoryByIdAsync(Guid id, bool? tracking = null, CancellationToken token = default, params Expression<Func<PunchEntry, object>>[] includes);
    IQueryable<PunchEntry?> GetWithHistoryForUser(Guid userId, DateTime from, DateTime to, ISorting<PunchEntry>? sorting = null, bool? tracking = null);
    Task<IEnumerable<PunchEntry?>> GetWithHistoryForUserAsync(Guid userId, DateTime from, DateTime to, ISorting<PunchEntry>? sorting = null, bool? tracking = null, CancellationToken token = default, params Expression<Func<PunchEntry, object>>[] includes);
    Task<PunchStatus?> GetLastStableStatusForUserAsync(Guid userId, CancellationToken token = default);
}

public class PunchEntriesRepository : DataRepository<PunchEntry>, IPunchEntriesRepository
{
    private static readonly string PunchNotExistText = "PunchEntry specified does not exist";
    private static readonly string PunchRequiresHistoryText = "PunchEntry must have a history item attached";
    internal PunchEntriesRepository(TimeClockContext context)
        : base(context) { }

    private IQueryable<PunchEntry> BasicGet(bool? tracking = null)
    {
        return base.GetTracker(tracking).Include(p => p.CurrentState!.PunchEntriesHistory);
    }
    private IQueryable<PunchEntry> BasicGetMany(ISorting<PunchEntry>? sorting = null, bool? tracking = null)
    {
        return base.SortedDbSet(sorting, tracking).Include(p => p.CurrentState!.PunchEntriesHistory).Include(p => p.CurrentState.StablePunchEntriesHistory);
    }

    public override IQueryable<PunchEntry> Get(Expression<Func<PunchEntry, bool>> predicate, ISorting<PunchEntry>? sorting = null, bool? tracking = null, params Expression<Func<PunchEntry, object>>[] includes)
    {
        var query = this.BasicGetMany(sorting, tracking).Where(predicate).IncludeMultiple(includes);
        if (includes.Length > 1)
            query = query.AsSplitQuery();
        return query;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="sorting"></param>
    /// <param name="tracking"></param>
    /// <param name="token"></param>
    /// <param name="includes"></param>
    /// <returns></returns>
    /// <remarks>
    /// This method calls ToListAsync to execute query and return the results. Turns out, async queries will run extremely slow if they contain any max fields in the query, or if the result set is not small. 
    /// Recommend only using the async for result sets that are small, and that query a table with no max size columns (like nvarchar(max)).
    /// </remarks>
    public override async Task<IEnumerable<PunchEntry>> GetAsync(Expression<Func<PunchEntry, bool>> predicate, ISorting<PunchEntry>? sorting = null, bool? tracking = null, CancellationToken token = default, params Expression<Func<PunchEntry, object>>[] includes) 
    {
        var query = this.BasicGetMany(sorting, tracking).Where(predicate).IncludeMultiple(includes);
        if (includes.Length > 1)
            query = query.AsSplitQuery();
        return await query.ToListAsync(token);
        //return await Task.Run<IEnumerable<PunchEntry>>(() => query.ToList());
    }

    public IQueryable<PunchEntry> GetByDates(DateTime from, DateTime to, ISorting<PunchEntry>? sorting = null, bool? tracking = null)
    {
        return this.BasicGetMany(sorting, tracking).Where(t => t.PunchEntriesHistories.Any(h => h.DateTime >= from && h.DateTime <= to));
    }

    public async Task<IEnumerable<PunchEntry>> GetByDatesAsync(DateTime from, DateTime to, ISorting<PunchEntry>? sorting = null, bool? tracking = null, CancellationToken token = default, params Expression<Func<PunchEntry, object>>[] includes)
    {
        return await this.BasicGetMany(sorting, tracking).IncludeMultiple(includes)
            .Where(t => t.PunchEntriesHistories.Any(h => h.DateTime >= from && h.DateTime <= to)).ToListAsync(token);
    }
    public PunchEntry? GetByHistoryId(Guid id, bool? tracking = null, params Expression<Func<PunchEntry, object>>[] includes)
    {
        return this.BasicGet(tracking).IncludeMultiple(includes).FirstOrDefault(p => p.PunchEntriesHistories.FirstOrDefault(h => h.Id == id) != null);
    }
    public Task<PunchEntry?> GetByHistoryIdAsync(Guid id, bool? tracking = null, CancellationToken token = default, params Expression<Func<PunchEntry, object>>[] includes)
    {
        return this.BasicGet(tracking).IncludeMultiple(includes).FirstOrDefaultAsync(p => p.PunchEntriesHistories.FirstOrDefault(h => h.Id == id) != null, token);
    }

    public PunchEntry? GetCurrentStateById(Guid id, bool? tracking = null, params Expression<Func<PunchEntry, object>>[] includes)
    {
        return this.BasicGet(tracking).IncludeMultiple(includes).FirstOrDefault(t => t.Id == id);
    }
    public PunchEntry? GetWithHistoryById(Guid id, bool? tracking = null, params Expression<Func<PunchEntry, object>>[] includes)
    {
        return this.BasicGet(tracking).Include(t => t.PunchEntriesHistories).IncludeMultiple(includes)
            .FirstOrDefault(t => t.Id == id);
    }
    public IQueryable<PunchEntry?> GetWithHistory(DateTime from, DateTime to, ISorting<PunchEntry>? sorting = null, bool? tracking = null)
    {
        return this.BasicGetMany(sorting, tracking).Include(t => t.PunchEntriesHistories)
            .Where(t => t.PunchEntriesHistories.Any(h => h.DateTime >= from && h.DateTime <= to));
    }
    public IQueryable<PunchEntry?> GetCurrentStates(DateTime from, DateTime to, ISorting<PunchEntry>? sorting = null, bool? tracking = null)
    {
        return this.BasicGetMany(sorting, tracking)
            .Where(t => t.CurrentState!.PunchEntriesHistory.DateTime >= from
                && t.CurrentState.PunchEntriesHistory.DateTime <= to);
    }
    public IQueryable<PunchEntry?> GetCurrentStatesForUser(Guid userId, DateTime from, DateTime to, ISorting<PunchEntry>? sorting = null, bool? tracking = null)
    {
        return this.BasicGetMany(sorting, tracking)
            .Where(t => t.UserId == userId
                && t.CurrentState!.PunchEntriesHistory.DateTime >= from
                && t.CurrentState.PunchEntriesHistory.DateTime <= to);
    }
    public IQueryable<PunchEntry?> GetWithHistoryForUser(Guid userId, DateTime from, DateTime to, ISorting<PunchEntry>? sorting = null, bool? tracking = null)
    {
        return this.BasicGetMany(sorting, tracking).Include(t => t.PunchEntriesHistories)
            .Where(t => t.UserId == userId && t.PunchEntriesHistories.Any(h => h.DateTime >= from && h.DateTime <= to));
    }
    public Task<PunchEntry?> GetCurrentStateByIdAsync(Guid id, bool? tracking = null, CancellationToken token = default, params Expression<Func<PunchEntry, object>>[] includes)
    {
        return this.BasicGet(tracking).IncludeMultiple(includes).FirstOrDefaultAsync(t => t.Id == id);
    }
    public Task<PunchEntry?> GetWithHistoryByIdAsync(Guid id, bool? tracking = null, CancellationToken token = default, params Expression<Func<PunchEntry, object>>[] includes)
    {
        return this.BasicGet(tracking).Include(t => t.PunchEntriesHistories).IncludeMultiple(includes)
            .FirstOrDefaultAsync(t => t.Id == id);
    }
    public async Task<IEnumerable<PunchEntry?>> GetWithHistoryAsync(DateTime from, DateTime to, ISorting<PunchEntry>? sorting = null, bool? tracking = null, CancellationToken token = default, params Expression<Func<PunchEntry, object>>[] includes)
    {
        return await this.BasicGetMany(sorting, tracking).Include(t => t.PunchEntriesHistories).IncludeMultiple(includes)
            .Where(t => t.PunchEntriesHistories.Any(h => h.DateTime >= from && h.DateTime <= to)).ToListAsync(token);
    }
    public async Task<IEnumerable<PunchEntry?>> GetCurrentStatesAsync(DateTime from, DateTime to, ISorting<PunchEntry>? sorting = null, bool? tracking = null, CancellationToken token = default, params Expression<Func<PunchEntry, object>>[] includes)
    {
        return await this.BasicGetMany(sorting, tracking).Include(t => t.CurrentState!.PunchEntriesHistory)
            .Where(t => t.CurrentState!.PunchEntriesHistory.DateTime >= from
                && t.CurrentState.PunchEntriesHistory.DateTime <= to)
            .IncludeMultiple(includes)
            .ToListAsync(token);
    }
    public async Task<IEnumerable<PunchEntry?>> GetCurrentStatesForUserAsync(Guid userId, DateTime from, DateTime to, ISorting<PunchEntry>? sorting = null, bool? tracking = null, CancellationToken token = default, params Expression<Func<PunchEntry, object>>[] includes)
    {
            return await this.BasicGetMany(sorting, tracking)
                .Where(t => t.UserId == userId
                    && t.CurrentState!.PunchEntriesHistory.DateTime >= from
                    && t.CurrentState.PunchEntriesHistory.DateTime <= to)
                .IncludeMultiple(includes)
                .ToListAsync(token);
    }
    public async Task<IEnumerable<PunchEntry?>> GetWithHistoryForUserAsync(Guid userId, DateTime from, DateTime to, ISorting<PunchEntry>? sorting = null, bool? tracking = null, CancellationToken token = default, params Expression<Func<PunchEntry, object>>[] includes)
    {
        return await this.BasicGetMany(sorting, tracking).Include(t => t.PunchEntriesHistories)
            .Where(t => t.UserId == userId && t.PunchEntriesHistories.Any(h => h.DateTime >= from && h.DateTime <= to))
            .IncludeMultiple(includes)
            .ToListAsync(token);
    }
    public Task<PunchStatus?> GetLastStableStatusForUserAsync(Guid userId, CancellationToken token = default)
    {
        return base.Context.PunchEntries
            .Where(p => p.UserId == userId && p.CurrentState.StablePunchEntriesHistoryId != null)
            .OrderByDescending(p => p.RowId)
            .Take(1)
            .Select(p => p.CurrentState.StableStatus)
            .SingleOrDefaultAsync(token);
    }


    /// <summary>
    /// Not fully implemented.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="save"></param>
    public override void Delete(Guid id, bool? save = null)
    {
        try
        {
            using (var transaction = base.Context.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
            {
                base.Context.PunchEntriesCurrentStates.Remove(
                    base.Context.PunchEntriesCurrentStates.First(p => p.PunchEntryId == id));
                base.Context.PunchEntriesHistories.RemoveRange(
                    base.Context.PunchEntriesHistories.Where(p => p.PunchEntryId == id));
                base.Context.PunchEntries.Remove(base.Context.PunchEntries.First(p => p.Id == id));
                if (base.GetSaveType(save) != SaveType.None)
                    base.Save();
            }
        }
        catch (Exception) { throw; } // TODO: log exception
    }

    #region Adds
    /// <summary>
    /// Will get or create a <see cref="WorkPeriod"/> using the provided Date whose purpose is <see cref="WorkPeriodPurpose.PunchEntriesSum"/>.
    /// Will NOT save the <see cref="WorkPeriod"/> if it is created.
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public void AddHistory(PunchEntriesHistory history, int workPeriodThreshold, Guid? jobTypeId, Guid? jobStepId, bool? save = null)
    {
        if (!base.Context.PunchEntries.Any(p => p.Id == history.PunchEntryId))
            throw new ArgumentException(PunchEntriesRepository.PunchNotExistText);
        this.Context.PunchEntriesHistories.Add(history);
        var entry = base.Context.PunchEntries.First(p => p.Id == history.PunchEntryId);
        var wp = this.SetCurrentState(entry, workPeriodThreshold, jobTypeId, jobStepId)
            .GetAwaiter().GetResult();

        if (base.GetSaveType(save) != SaveType.None) base.Save();

        if (entry.WorkPeriod is null)
            entry.WorkPeriod = wp;
    }
    /// <summary>
    /// Adds the <see cref="PunchEntry"/> <paramref name="item"/> with its <see cref="PunchEntriesHistory"/> and sets the <see cref="PunchEntriesCurrentState"/> for the provided <see cref="PunchEntry"/>.
    /// Also assigns a <see cref="WorkPeriod"/>. 
    /// The <see cref="WorkPeriod"/> will be created if it does not exist, and the new or existing <see cref="WorkPeriod"/> will be returned in its property. 
    /// A link to the corresponding <see cref="WorkPeriodJobTypeStep"/> will be added if a current match does not exist, using <see langword="null"/> as <see cref="JobType"/>, <see langword="null"/> as <see cref="JobStep"/> and <see cref="WorkPeriod.Id"/>.
    /// The <see cref="WorkPeriod"/> is determined using a threshold of 12 hours.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="save"></param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    [Obsolete("Use overload which accepts jobTypeId and JobStepId instead")]
    public override void Add(PunchEntry item, bool? save = null)
    {
        if (item.PunchEntriesHistories == null || item.PunchEntriesHistories.Count < 1)
            throw new ArgumentNullException("PunchEntry must have a history item attached");
        if (item.UserId == Guid.Empty)
            throw new ArgumentException("Invalid User ID provided");

        base.Add(item, save);
        var wp = this.SetCurrentState(item, 12, null, null).GetAwaiter().GetResult();

        if (item.WorkPeriod is null)
            item.WorkPeriod = wp;
    }
    /// <summary>
    /// Adds the <see cref="PunchEntry"/> <paramref name="item"/> with its <see cref="PunchEntriesHistory"/> and sets the <see cref="PunchEntriesCurrentState"/> for the provided <see cref="PunchEntry"/>.
    /// Also assigns a <see cref="WorkPeriod"/>. 
    /// The <see cref="WorkPeriod"/> will be created if it does not exist, and the new or existing <see cref="WorkPeriod"/> will be returned in its property. 
    /// A link to the corresponding <see cref="WorkPeriodJobTypeStep"/> will be added if a current match does not exist, using the <paramref name="jobTypeId"/>, <paramref name="jobStepId"/> and <see cref="WorkPeriod.Id"/>.
    /// The <see cref="WorkPeriod"/> is determined using the <paramref name="workPeriodThreshold"/> provided.
    /// </summary>
    /// <param name="item">The <see cref="PunchEntry"/> to add. ID can be specified.</param>
    /// <param name="workPeriodThreshold"></param>
    /// <param name="jobTypeId"></param>
    /// <param name="jobStepId"></param>
    /// <param name="save">Whether or not to instantly save this operation.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public void Add(PunchEntry item, int workPeriodThreshold, Guid? jobTypeId, Guid? jobStepId, bool? save = null)
    {
        if (item.PunchEntriesHistories == null || item.PunchEntriesHistories.Count < 1)
            throw new ArgumentNullException("PunchEntry must have a history item attached");
        if (item.UserId == Guid.Empty)
            throw new ArgumentException("Invalid User ID provided");

        base.Add(item, save);
        var wp = this.SetCurrentState(item, workPeriodThreshold, null, null).GetAwaiter().GetResult();

        if (item.WorkPeriod is null)
            item.WorkPeriod = wp;
    }
    /// <summary>
    /// Adds the <see cref="PunchEntry"/> <paramref name="item"/> with its <paramref name="history"/> and sets the <see cref="PunchEntriesCurrentState"/> for the provided <see cref="PunchEntry"/>.
    /// Also assigns a <see cref="WorkPeriod"/>. 
    /// The <see cref="WorkPeriod"/> will be created if it does not exist, and the new or existing <see cref="WorkPeriod"/> will be returned in its property. 
    /// A link to the corresponding <see cref="WorkPeriodJobTypeStep"/> will be added if a current match does not exist, using the <paramref name="jobTypeId"/>, <paramref name="jobStepId"/> and <see cref="WorkPeriod.Id"/>.
    /// The <see cref="WorkPeriod"/> is determined using the <paramref name="workPeriodThreshold"/> provided.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="history"></param>
    /// <param name="workPeriodThreshold"></param>
    /// <param name="jobTypeId"></param>
    /// <param name="jobStepId"></param>
    /// <param name="save">Whether or not to instantly save this operation.</param>
    /// <exception cref="ArgumentException"></exception>
    public void Add(PunchEntry item, PunchEntriesHistory history, int workPeriodThreshold, Guid? jobTypeId, Guid? jobStepId, bool? save = null)
    {
        if (item.UserId == Guid.Empty)
            throw new ArgumentException("Invalid User ID provided");

        if (item.PunchEntriesHistories == null)
            item.PunchEntriesHistories = new List<PunchEntriesHistory>();
        if (item.PunchEntriesHistories.Any(h => h.DateTime == history.DateTime || h.EffectiveDateTime == history.EffectiveDateTime))
        {
            item.PunchEntriesHistories.Remove(item.PunchEntriesHistories.First(h => h.DateTime == history.DateTime || h.EffectiveDateTime == history.EffectiveDateTime));
        }
            
        item.PunchEntriesHistories.Add(history);
        base.Add(item, save); // might have to comment this out if it throws error or creates duplicate
        var wp = this.SetCurrentState(item, workPeriodThreshold, jobTypeId, jobStepId).GetAwaiter().GetResult();

        if (item.WorkPeriod is null)
            item.WorkPeriod = wp;
    }
    /// <summary>
    /// Creates and adds a new <see cref="PunchEntry"/> for the <paramref name="userId"/> with its <paramref name="history"/> and sets the <see cref="PunchEntriesCurrentState"/> for the provided <see cref="PunchEntry"/>.
    /// Also assigns a <see cref="WorkPeriod"/>. 
    /// The <see cref="WorkPeriod"/> will be created if it does not exist, and the new or existing <see cref="WorkPeriod"/> will be returned in its property. 
    /// A link to the corresponding <see cref="WorkPeriodJobTypeStep"/> will be added if a current match does not exist, using the <paramref name="jobTypeId"/>, <paramref name="jobStepId"/> and <see cref="WorkPeriod.Id"/>.
    /// The <see cref="WorkPeriod"/> is determined using the <paramref name="workPeriodThreshold"/> provided.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="history"></param>
    /// <param name="workPeriodThreshold"></param>
    /// <param name="jobTypeId"></param>
    /// <param name="jobStepId"></param>
    /// <param name="save"></param>
    public void Add(Guid userId, PunchEntriesHistory history, int workPeriodThreshold, Guid? jobTypeId, Guid? jobStepId, bool? save = null)
    {
        PunchEntry item = new PunchEntry() { UserId = userId };
        if (item.PunchEntriesHistories == null)
            item.PunchEntriesHistories = new List<PunchEntriesHistory>();
        item.PunchEntriesHistories.Add(history);
        base.Add(item, save);
        var wp = this.SetCurrentState(item, workPeriodThreshold, jobTypeId, jobStepId).GetAwaiter().GetResult();

        if (item.WorkPeriod is null)
            item.WorkPeriod = wp;
    }
    /// <summary>
    /// Adds a PunchEntriesHistory to the PunchEntry specified by PunchEntriesHistory.PunchEntryId.
    /// Will set the CurrentState for the punch entry.
    /// </summary>
    /// <param name="history"></param>
    /// <param name="save"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task AddHistoryAsync(PunchEntriesHistory history, int workPeriodThreshold, Guid? jobTypeId, Guid? jobStepId, bool? save = null, CancellationToken token = default)
    {
        if (!base.Context.PunchEntries.Any(p => p.Id == history.PunchEntryId))
            throw new ArgumentException(PunchEntriesRepository.PunchNotExistText);
        await this.Context.PunchEntriesHistories.AddAsync(history, token);
        var entry = base.Context.PunchEntries.First(p => p.Id == history.PunchEntryId);
        entry.PunchEntriesHistories.Add(history);
        var wp = await this.SetCurrentState(entry, workPeriodThreshold, jobTypeId, jobStepId, token);
        if (base.GetSaveType(save) != SaveType.None) base.Save();
        if (entry.WorkPeriod is null)
            entry.WorkPeriod = wp;
    }

    /// <summary>
    /// <inheritdoc cref="Add(PunchEntry, bool?)"/>
    /// </summary>
    /// <param name="item"><inheritdoc cref="Add(PunchEntry, bool?)" path="/param[@name='item']"/></param>
    /// <param name="save"><inheritdoc cref="Add(PunchEntry, bool?)" path="/param[@name='save']"/></param>
    /// <param name="token"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    [Obsolete("Use overload which accepts jobTypeId and JobStepId instead")]
    public override async Task AddAsync(PunchEntry item, bool? save = null, CancellationToken token = default)
    {
        if (item.PunchEntriesHistories == null || item.PunchEntriesHistories.Count < 1)
            throw new ArgumentNullException(PunchEntriesRepository.PunchRequiresHistoryText);
        var wp = await this.SetCurrentState(item, 12, null, null, token);
        await base.AddAsync(item, save, token);
        if (item.WorkPeriod is null)
            item.WorkPeriod = wp;
    }
    /// <summary>
    /// <inheritdoc cref="Add(PunchEntry, int, Guid?, Guid?, bool?)"/>
    /// </summary>
    /// <param name="item"><inheritdoc cref="Add(PunchEntry, int, Guid?, Guid?, bool?)" path="/param[@name='item']"/></param>
    /// <param name="workPeriodThreshold"><inheritdoc cref="Add(PunchEntry, int, Guid?, Guid?, bool?)" path="/param[@name='workPeriodThreshold']"/></param>
    /// <param name="jobTypeId"><inheritdoc cref="Add(PunchEntry, int, Guid?, Guid?, bool?)" path="/param[@name='jobTypeId']"/></param>
    /// <param name="jobStepId"><inheritdoc cref="Add(PunchEntry, int, Guid?, Guid?, bool?)" path="/param[@name='jobStepId']"/></param>
    /// <param name="save"><inheritdoc cref="Add(PunchEntry, int, Guid?, Guid?, bool?)" path="/param[@name='save']"/></param>
    /// <param name="token"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public async Task AddAsync(PunchEntry item, int workPeriodThreshold, Guid? jobTypeId, Guid? jobStepId, bool? save = null, CancellationToken token = default)
    {
        if (item.PunchEntriesHistories == null || item.PunchEntriesHistories.Count < 1)
            throw new ArgumentNullException(PunchEntriesRepository.PunchRequiresHistoryText);
        var wp = await this.SetCurrentState(item, workPeriodThreshold, jobTypeId, jobStepId, token);
        await base.AddAsync(item, save, token);
        if (item.WorkPeriod is null)
            item.WorkPeriod = wp;
    }
    /// <summary>
    /// <inheritdoc cref="Add(PunchEntry, PunchEntriesHistory, int, Guid?, Guid?, bool?)"/>
    /// </summary>
    /// <param name="item"><inheritdoc cref="Add(PunchEntry, PunchEntriesHistory, int, Guid?, Guid?, bool?)" path="/param[@name='item']"/></param>
    /// <param name="history"><inheritdoc cref="Add(PunchEntry, PunchEntriesHistory, int, Guid?, Guid?, bool?)" path="/param[@name='history']"/></param>
    /// <param name="workPeriodThreshold"><inheritdoc cref="Add(PunchEntry, PunchEntriesHistory, int, Guid?, Guid?, bool?)" path="/param[@name='workPeriodThreshold']"/></param>
    /// <param name="jobTypeId"><inheritdoc cref="Add(PunchEntry, PunchEntriesHistory, int, Guid?, Guid?, bool?)" path="/param[@name='jobTypeId']"/></param>
    /// <param name="jobStepId"><inheritdoc cref="Add(PunchEntry, PunchEntriesHistory, int, Guid?, Guid?, bool?)" path="/param[@name='jobStepId']"/></param>
    /// <param name="save"><inheritdoc cref="Add(PunchEntry, PunchEntriesHistory, int, Guid?, Guid?, bool?)" path="/param[@name='save']"/></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task AddAsync(PunchEntry item, PunchEntriesHistory history, int workPeriodThreshold, Guid? jobTypeId, Guid? jobStepId, bool? save = null, CancellationToken token = default)
    {
        item.PunchEntriesHistories.Add(history);
        var wp = await this.SetCurrentState(item, workPeriodThreshold, jobTypeId, jobStepId);
        await base.AddAsync(item, save, token);
        if (item.WorkPeriod is null)
            item.WorkPeriod = wp;
    }
    /// <summary>
    /// <inheritdoc cref="Add(Guid, PunchEntriesHistory, int, Guid?, Guid?, bool?)"/>
    /// </summary>
    /// <param name="userId"><inheritdoc cref="Add(Guid, PunchEntriesHistory, int, Guid?, Guid?, bool?)" path="/param[@name='userId']"/></param>
    /// <param name="history"><inheritdoc cref="Add(Guid, PunchEntriesHistory, int, Guid?, Guid?, bool?)" path="/param[@name='history']"/></param>
    /// <param name="workPeriodThreshold"><inheritdoc cref="Add(Guid, PunchEntriesHistory, int, Guid?, Guid?, bool?)" path="/param[@name='workPeriodThreshold']"/></param>
    /// <param name="jobTypeId"><inheritdoc cref="Add(Guid, PunchEntriesHistory, int, Guid?, Guid?, bool?)" path="/param[@name='jobTypeId']"/></param>
    /// <param name="jobStepId"><inheritdoc cref="Add(Guid, PunchEntriesHistory, int, Guid?, Guid?, bool?)" path="/param[@name='jobStepId']"/></param>
    /// <param name="save"><inheritdoc cref="Add(Guid, PunchEntriesHistory, int, Guid?, Guid?, bool?)" path="/param[@name='save']"/></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task AddAsync(Guid userId, PunchEntriesHistory history, int workPeriodThreshold, Guid? jobTypeId, Guid? jobStepId, bool? save = null, CancellationToken token = default)
    {
        PunchEntry item = new PunchEntry() { UserId = userId };
        item.PunchEntriesHistories.Add(history);
        var wp = await this.SetCurrentState(item, workPeriodThreshold, jobTypeId, jobStepId, token);
        await base.AddAsync(item, save, token);
        if (item.WorkPeriod is null)
            item.WorkPeriod = wp;
    }
    #endregion Adds


    #region Helpers
    /// <summary>
    /// Adds or updates the <see cref="PunchEntriesCurrentState"/> for a <see cref="PunchEntry"/>, and assigns a <see cref="WorkPeriod"/>. 
    /// The <see cref="WorkPeriod"/> will be created if it does not exist, and the new or existing <see cref="WorkPeriod"/> will be returned in its property. 
    /// A link to the corresponding <see cref="WorkPeriodJobTypeStep"/> will be added if a current match does not exist.
    /// </summary>
    /// <param name="entry">A <see cref="PunchEntry"/> with at least 1 valid <see cref="PunchEntriesHistory"/> to be used as the <see cref="PunchEntriesCurrentState"/> </param>
    /// <param name="workPeriodThreshold">
    /// A threshold used to decide if the previous punch is too long ago to re-use previous WorkPeriod. 
    /// As it would not make sense to use a WorkPeriod from 2 days prior (as an example).
    /// This prevents creating a closing punch which is due to a forgotten punch, and therefore should not have been attached to the previous WorkPeriod cycle. 
    /// This value is in hours.
    /// </param>
    /// <param name="jobTypeId"></param>
    /// <param name="jobStepId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async ValueTask<WorkPeriod> SetCurrentState(PunchEntry entry, int workPeriodThreshold, Guid? jobTypeId, Guid? jobStepId, CancellationToken cancellationToken = default)
    {
        IQueryable<PunchEntriesHistory> entriesHistories = base.Context.PunchEntriesHistories.AsNoTracking()
            .Where(h => h.PunchEntryId == entry.Id)
            .OrderByDescending(h => h.UtcTimeStamp);
        // compare entry.history date to db current state history date
        // use the new one as current state
        // get work period using current date. new if not exist
        PunchEntriesHistory? dbHistory = await entriesHistories.FirstOrDefaultAsync(cancellationToken: cancellationToken);
        PunchEntriesHistory entryHistory = entry.PunchEntriesHistories.OrderByDescending(h => h.UtcTimeStamp).First();
        // this would probably make sense to take entry history only, and ignore what's currently in db
        PunchEntriesHistory newestHistory = dbHistory?.UtcTimeStamp > entryHistory.UtcTimeStamp ? dbHistory : entryHistory;
        PunchEntriesCurrentState? state = await base.Context.PunchEntriesCurrentStates.AsNoTracking().FirstOrDefaultAsync(s => s.PunchEntry == entry);
        //Guid workPeriodId = history.WorkPeriodId;// == default ? (histories.FirstOrDefault(h => h.WorkPeriodId != default)?.WorkPeriodId ?? Guid.Empty) : history.WorkPeriodId;
        PunchEntriesHistory? stableDbHistory = await entriesHistories.FirstOrDefaultAsync(h => PunchActionEx.StableStates.Contains(h.Action) || h.Action == PunchAction.Void, cancellationToken: cancellationToken);
        PunchEntriesHistory? stableEntryHistory = entry.PunchEntriesHistories.OrderByDescending(h => h.UtcTimeStamp).FirstOrDefault(h => PunchActionEx.StableStates.Contains(h.Action) || h.Action == PunchAction.Void);
        PunchEntriesHistory? stableHistory = stableDbHistory?.UtcTimeStamp > stableEntryHistory?.UtcTimeStamp ? stableDbHistory : stableEntryHistory;
        bool needsStatuses = state is null;
        // allow grabbing void Actions so we do not bypass the void. but voided is not a stable state, so make it null
        if (state is null)
        {
            state = new PunchEntriesCurrentState()
            {
                //PunchEntryId = entry.Id,
                PunchEntry = entry,
                PunchEntriesHistory = newestHistory,
                StablePunchEntriesHistory = stableHistory?.Action == PunchAction.Void ? null : stableHistory
            };

            //base.Context.PunchEntriesCurrentStates.Add(state);
        }
        else
        {
            state.PunchEntriesHistory = newestHistory;
            state.StablePunchEntriesHistory = stableHistory?.Action == PunchAction.Void ? null : stableHistory;
        }

        if (newestHistory.Id != default)
            state.PunchEntriesHistoryId = newestHistory.Id;
        if (stableHistory?.Id != default)
            state.StablePunchEntriesHistoryId = stableHistory?.Action == PunchAction.Void ? null : stableHistory?.Id;

        // this is redundant, but to ensure current state and work period get saved, if only the punch entry table is saved.
        entry.CurrentState = state;
        // attaches workPeriod and also updates workPeriod hours worked
        var workPeriod = await this.GetOrCreateWorkPeriodAsync(entry, workPeriodThreshold, false, cancellationToken);

        if (state.StablePunchEntriesHistory is not null && !await base.Context.WorkPeriodJobTypeSteps.AnyAsync(j => j.PunchEntryId == entry.Id))
        {
            WorkPeriodJobTypeStep workPeriodJobType = new()
            {
                ActivatedOn = (entry.CurrentState.StablePunchEntriesHistory?.EffectiveDateTime ?? entry.CurrentState.PunchEntriesHistory.EffectiveDateTime!).Value,
                JobTypeId = jobTypeId,
                JobStepId = jobStepId,
            };
            if (entry.WorkPeriod is not null)
                workPeriodJobType.WorkPeriod = entry.WorkPeriod;
            else
                workPeriodJobType.WorkPeriodId = entry.WorkPeriodId;
            if (entry.Id != default)
                workPeriodJobType.PunchEntryId = entry.Id;
            else
                workPeriodJobType.PunchEntry = entry;
                base.Context.WorkPeriodJobTypeSteps.Add(workPeriodJobType);
        }
        
        if (needsStatuses)
        {
            try
            {
                // if workPeriod.RowId is 0, this means it is a new workPeriod. and so it will have only the punchEntry we just created.
                // otherwise, we need to add 1 to our punchEntry collection to account for the entry we just created.
                // we know creation of entry to be true because needsStatuses was true. no state existed, so no punch existed
                state.Status = workPeriod.RowId == 0 ? PunchStatus.In : 
                    (await base.Context.WorkPeriods.Where(w => w.Id == workPeriod.Id).Select(w => w.PunchEntries.Count + 1).FirstOrDefaultAsync()) % 2 == 0 ? PunchStatus.Out : PunchStatus.In;
                state.StableStatus = workPeriod.RowId == 0 ? PunchStatus.In : 
                    (await base.Context.WorkPeriods.Where(w => w.Id == workPeriod.Id).Select(w => w.PunchEntries.Count(p => p.CurrentState.StablePunchEntriesHistoryId != null) + 1).FirstOrDefaultAsync()) % 2 == 0 ? PunchStatus.Out : PunchStatus.In;
            }
            catch(Exception ex)
            {
                // missing a status isn't a good reason to halt execution. but we should know it happened
#warning this error should get logged
                Console.WriteLine(ex.Message);
            }
        }

        return workPeriod;
    }

    private async Task<WorkPeriod> GetOrCreateWorkPeriodAsync(PunchEntry punchEntry, int workPeriodBuffer, bool tracking = false, CancellationToken cancellationToken = default)
    {
        #region Setup
        PunchEntriesHistory history = punchEntry.CurrentState.PunchEntriesHistory;
        if (!history.DateTime.HasValue) throw new Exception($"{nameof(GetOrCreateWorkPeriodAsync)} PunchEntriesHistory.DateTime must have a value.");

        WorkPeriodsRepository workPeriodRepo = new(base.Context);
        Expression<Func<PunchEntry, object>>[] include = [(p) => p.WorkPeriod];
        DateTime minDate = history.DateTime.Value.AddHours(workPeriodBuffer * -1);
        DateTime maxDate = history.DateTime.Value.AddHours(workPeriodBuffer);
        // sorting to use in db
        ISorting<PunchEntry> sorting = new Sorting<PunchEntry>();
        sorting.Sorts.Add(new() { SortBy = (p) => p.CurrentState.PunchEntriesHistory.DateTime!, Order = SortOrder.Descending });
        WorkPeriod? workPeriod = null;
        #endregion Setup

        // if we already assigned a work period to this history item, return that. 
        // this can happen if the work period is set in the UI as a correction
        if (punchEntry.WorkPeriodId != default)
        {
            // if it is assigned a work period, but we do not have it - go fetch it from db
            workPeriod = punchEntry.WorkPeriod ?? (await workPeriodRepo.GetAsync(punchEntry.WorkPeriodId, tracking: tracking, token: cancellationToken));
        }

        //// check if we find a work period for the punch's date
        //if (workPeriod is null)
        //{
        //    workPeriod = await workPeriodRepo.GetLatestPendingForUserByDateAsync(punchEntry.UserId, history.DateTime.Value.Date, tracking, cancellationToken);
        //}

        if (workPeriod is not null)
        {
            List<PunchEntriesCurrentState> currentStates = await base.Context.PunchEntriesCurrentStates
                .AsNoTracking()
                .Where(c => c.PunchEntry.WorkPeriodId == workPeriod.Id && PunchActionEx.StableStates.Contains(c.PunchEntriesHistory.Action))
                .Include(c => c.PunchEntriesHistory)
                .Include(c => c.StablePunchEntriesHistory)
                .ToListAsync();
            // no current state exists, so add it
            if (!currentStates.Any(s => s.Id == punchEntry.CurrentState.Id))
            {
                currentStates.Add(punchEntry.CurrentState);
            }
            else
            {// current state exists, we need to update it
                int index = currentStates.FindIndex(c => c.Id == punchEntry.CurrentState.Id);
                currentStates.RemoveAt(index);
                currentStates.Insert(index, punchEntry.CurrentState);
            }

            //if (punchEntry.WorkPeriodId != workPeriod.Id)
            if (punchEntry.WorkPeriodId == default)
            {
                punchEntry.WorkPeriodId = workPeriod.Id;
                //punchEntry.WorkPeriod = workPeriod;
            }
            //// this causes ef to try and insert the already existing work period, causing an exception
            //// possibly due to using work period repo, instead of direct context
            //if (punchEntry.WorkPeriod is null)
            //{
            //    punchEntry.WorkPeriod = workPeriod;
            //}
            workPeriod.HoursWorked = currentStates.TotalPunchTime().TotalHours;

            await workPeriodRepo.UpdateToDbAsync(workPeriod.Id, (s => s.SetProperty(w => w.HoursWorked, workPeriod.HoursWorked)));

            if (workPeriod.IsPreviousMissingPunch is null)
            {
                workPeriod.IsPreviousMissingPunch = await base.Context.WorkPeriods.IsPreviousMissingPunch(workPeriod);
            }

            return workPeriod;
        }

        // get punch entries before and after our new punch entry, using the threshold
        List<PunchEntry> entries = new(await this.GetAsync(p =>
            p.CurrentState.PunchEntriesHistory.DateTime >= minDate && p.CurrentState.PunchEntriesHistory.DateTime <= maxDate && 
            p.UserId == punchEntry.UserId,
            sorting: sorting, false, cancellationToken, include));

        if (!entries.Any(p => p.Id == punchEntry.Id)) 
        {
            entries.Add(punchEntry); // add current punch entry, since it is not in DB yet
            entries = entries.OrderByDescending(p => p.CurrentState.PunchEntriesHistory.DateTime) // re-sort since we added a punch entry
            .ToList();
        }
        
        if (entries.Count > 1) 
        {
            // if found more than 1, which one should we use to determine the work period? the closest one
            PunchEntry? nearestPunchEntry = entries.MinBy(p => p.CurrentState.PunchEntriesHistory.DateTime!.Value.Subtract(history.DateTime.Value).TotalHours);
            if (nearestPunchEntry?.WorkPeriod is not null)
            {
                workPeriod = nearestPunchEntry.WorkPeriod;

                //punchEntry.WorkPeriod = workPeriod;
                punchEntry.WorkPeriodId = workPeriod.Id;
                workPeriod.HoursWorked = entries.Where(p => p.WorkPeriodId == workPeriod.Id).TotalPunchTime().TotalHours;

                await workPeriodRepo.UpdateToDbAsync(workPeriod.Id, (s => s.SetProperty(w => w.HoursWorked, workPeriod.HoursWorked)));
                //await workPeriodRepo.UpdateAsync(workPeriod, true);

                if (workPeriod.IsPreviousMissingPunch is null)
                {
                    workPeriod.IsPreviousMissingPunch = await base.Context.WorkPeriods.IsPreviousMissingPunch(workPeriod);
                }

                return workPeriod;
            }
        }
        // couldn't use an existing work period, so create a new one
        Guid newWorkPeriodId = Guid.NewGuid();
        workPeriod = new()
        {
            Id = newWorkPeriodId,
            HoursWorked = 0,
            PayPeriodEnd = DateOnly.FromDateTime(history.DateTime.Value.Date.NextDayOfWeek(DayOfWeek.Sunday).AddMicroseconds(-1)),
            Purpose = WorkPeriodPurpose.PunchEntriesSum,
            UserId = punchEntry.UserId,
            WorkDate = history.DateTime.Value.Date.ToDateOnly(),
            WorkPeriodStatusHistories = []
        };
        WorkPeriodStatusHistory newWorkPeriodStatus = new() { WorkPeriodId = newWorkPeriodId, DateTime = DateTime.Now, Status = WorkPeriodStatus.Pending, WorkPeriod = workPeriod };
        workPeriod.WorkPeriodStatusHistories.Add(newWorkPeriodStatus);

        base.Context.WorkPeriods.Add(workPeriod);
        base.Context.WorkPeriodStatusHistories.Add(newWorkPeriodStatus);
        punchEntry.WorkPeriod = workPeriod;
        //punchEntry.WorkPeriodId = workPeriod.Id;
        if (workPeriod.IsPreviousMissingPunch is null)
        {
            workPeriod.IsPreviousMissingPunch = await base.Context.WorkPeriods.IsPreviousMissingPunch(workPeriod);
        }

        return workPeriod;
    }

    #endregion Helpers
}
