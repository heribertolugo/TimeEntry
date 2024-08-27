using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using TimeClock.Data.Helpers;
using TimeClock.Data.Models;

namespace TimeClock.Data.Repository;

public class DataRepository<T>(TimeClockContext context) : IDataRepository<T> 
    where T : class, IEntityModel
{
    private bool _useTracker = false;

    protected TimeClockContext Context { get; init; } = context;
    protected IQueryable<T> Tracker { get; set; } = context.Set<T>().AsNoTracking();
    public bool SaveOnChanges { get; set; } = false;

    public bool UseTracker
    {
        get => this._useTracker;
        set
        {
            this._useTracker = value;
            this.Tracker = value ? this.Context.Set<T>().AsTracking() : this.Context.Set<T>().AsNoTracking();
        }
    }

    public virtual void Add(T item, bool? save = null)
    {
        this.Context.Set<T>().Add(item);

        if (this.GetSaveType(save) != SaveType.None) this.Save();
    }

    public virtual async Task AddAsync(T item, bool? save = null, CancellationToken token = default)
    {
        await this.Context.AddAsync(item, cancellationToken: token);

        if (this.GetSaveType(save) != SaveType.None)
            await this.SaveAsync(false, token);
    }

    public virtual void Delete(T item, bool? save = null)
    {
        SaveType saveType = this.GetSaveType(save);

        switch (saveType)
        {
            case DataRepository<T>.SaveType.SaveToDb:
                this.Context.Set<T>().Where(i => i.Id == item.Id).ExecuteDelete();
                break;
            case DataRepository<T>.SaveType.SaveToDbSet:
                this.Context.Set<T>().Remove(item);
                this.Save();
                break;
            case DataRepository<T>.SaveType.TrackToDbSet:
                this.Context.Set<T>().Remove(item);
                break;
        }
    }

    public virtual void Delete(Guid id, bool? save = null)
    {
        SaveType saveType = this.GetSaveType(save);

        switch (saveType)
        {
            case DataRepository<T>.SaveType.SaveToDb:
                this.Context.Set<T>().Where(i => i.Id == id).ExecuteDelete();
                break;
            case DataRepository<T>.SaveType.SaveToDbSet:
                this.Context.Set<T>().Remove(this.Context.Set<T>().First(t => t.Id == id));
                this.Save();
                break;
            case DataRepository<T>.SaveType.TrackToDbSet:
                this.Context.Set<T>().Remove(this.Context.Set<T>().First(t => t.Id == id));
                break;
        }
    }

    public virtual T? Get(Guid id, bool? tracking = null, params Expression<Func<T, object>>[] includes)
    {
        return this.GetTracker(tracking).IncludeMultiple(includes).FirstOrDefault(t => t.Id == id);
    }

    public virtual IQueryable<T> Get(Expression<Func<T, bool>> predicate, ISorting<T>? sorting = null, bool? tracking = null, params Expression<Func<T, object>>[] includes)
    {
        var query = this.SortedDbSet(sorting, tracking).Where(predicate).IncludeMultiple(includes);
        if (includes.Length > 1)
            query = query.AsSplitQuery();
        return query;
    }
    public virtual int GetCount(Expression<Func<T, bool>> predicate)
    {
        return this.Context.Set<T>().Where(predicate).Count();
    }

    public virtual Task<T?> GetAsync(Guid id, bool? tracking = null, CancellationToken token = default, params Expression<Func<T, object>>[] includes)
    {
        var query = this.GetTracker(tracking).IncludeMultiple(includes);
        if (includes.Length > 1)
            query = query.AsSplitQuery();
        return query.FirstOrDefaultAsync(t => t.Id == id, token);
    }

    public virtual async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicate, ISorting<T>? sorting = null, bool? tracking = null, CancellationToken token = default, params Expression<Func<T, object>>[] includes)
    {
        var query = this.SortedDbSet(sorting, tracking).IncludeMultiple(includes).Where(predicate);
        if (includes.Length > 1)
            query = query.AsSplitQuery();

        return await query.ToListAsync(token);
    }
    public virtual Task<int> GetCountAsync(Expression<Func<T, bool>> predicate)
    {
        return this.Context.Set<T>().Where(predicate).CountAsync();
    }

    public virtual void Save()
    {
        this.Save(false);
    }

    public virtual Task SaveAsync(CancellationToken token = default)
    {
        return this.Context.SaveChangesAsync(false, cancellationToken: token);
    }
    public virtual void Save(bool saveAll)
    {
        this.Context.SaveChanges(saveAll);
    }
    protected virtual Task SaveAsync(bool saveAll, CancellationToken token = default)
    {
        return this.Context.SaveChangesAsync(saveAll, cancellationToken: token);
    }
    public virtual void Update(T item, bool? save = null)
    {
        this.Context.Set<T>().Update(item);

        if (this.GetSaveType(save) != SaveType.None)
            this.Save();
    }
    public virtual async Task UpdateAsync(T item, bool? save = null, CancellationToken token = default)
    {
        this.Context.Set<T>().Update(item);

        if (this.GetSaveType(save) != SaveType.None)
            await this.SaveAsync(false, token: token);
    }
    public virtual Task UpdateToDbAsync(Guid id, Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> updates)
    {
        return this.Context.Set<T>().Where(n => n.Id == id).ExecuteUpdateAsync(updates);
    }

    public virtual IQueryable<T> GetAll(ISorting<T>? sorting = null, bool? tracking = null, params Expression<Func<T, object>>[] includes)
    {
        var query = this.SortedDbSet(sorting, tracking).IncludeMultiple(includes);
        if (includes.Length > 1)
            query = query.AsSplitQuery();
        return query;
    }

    public virtual IQueryable<T> GetPaged(Expression<Func<T, bool>> predicate, IPaging paging, ISorting<T>? sorting = null, bool? tracking = null, params Expression<Func<T, object>>[] includes)
    {
        var query = this.SortedDbSet(sorting, tracking).Where(predicate).Skip(paging.PageSize * (paging.PageNumber - 1)).Take(paging.PageSize).IncludeMultiple(includes);
        if (includes.Length > 1)
            query = query.AsSplitQuery();
        return query;
    }

    public virtual IQueryable<T> GetAllPaged(IPaging paging, ISorting<T>? sorting = null, bool? tracking = null, params Expression<Func<T, object>>[] includes)
    {
        var query = this.SortedDbSet(sorting, tracking).OrderBy(i => i.Id).Skip(paging.PageSize * (paging.PageNumber - 1)).Take(paging.PageSize).IncludeMultiple(includes);
        if (includes.Length > 1)
            query = query.AsSplitQuery();
        return query;
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(ISorting<T>? sorting = null, bool? tracking = null, CancellationToken token = default, params Expression<Func<T, object>>[] includes)
    {
        var query = this.SortedDbSet(sorting, tracking).IncludeMultiple(includes);
        if (includes.Length > 1)
            query = query.AsSplitQuery();
        return await query.ToListAsync(token);
    }

    public virtual async Task<IEnumerable<T>> GetPagedAsync(Expression<Func<T, bool>> predicate, IPaging paging, ISorting<T>? sorting = null, bool? tracking = null, CancellationToken token = default, params Expression<Func<T, object>>[] includes)
    {
        var query = this.SortedDbSet(sorting, tracking).IncludeMultiple(includes).Where(predicate).PageResults(paging);
        if (includes.Length > 1)
            query = query.AsSplitQuery();
        return await query.ToListAsync();
        // ** expando is no longer used since a better way of managing includes abstracly was found ** \\
        //var expandoQuery = query.AsExpandable().Where(predicate).PageResults(paging);
        //if (includes.Length > 1)
        //    expandoQuery = expandoQuery.AsSplitQuery();
        //// when using AsExpandable, cannot return ToListAsync
        //return Task.FromResult(expandoQuery.AsEnumerable());
    }

    public virtual async Task<IEnumerable<T>> GetAllPagedAsync(IPaging paging, ISorting<T>? sorting = null, bool? tracking = null, CancellationToken token = default, params Expression<Func<T, object>>[] includes)
    {
        var query = this.SortedDbSet(sorting, tracking).PageResults(paging).IncludeMultiple(includes);
        if (includes.Length > 1)
            query = query.AsSplitQuery();
        return await query.ToListAsync();
    }

    public int GetCount()
    {
        return this.Context.Set<T>().Count();
    }

    public Task<int> GetCountAsync(CancellationToken token = default)
    {
        return this.Context.Set<T>().CountAsync(token);
    }

    protected virtual IQueryable<T> GetTracker(bool? tracking)
    {
        if (tracking.HasValue)
            return tracking.Value ? this.Context.Set<T>().AsTracking() : this.Context.Set<T>().AsNoTracking();
        return this.Tracker;
    }
    /// <summary>
    /// Gets a IQueryable&lt;T&gt; of the DbSet sorted, with or without Tracking per UseTracker property value or tracking param override.
    /// </summary>
    /// <param name="sorting"></param>
    /// <param name="tracking"></param>
    /// <returns></returns>
    protected virtual IQueryable<T> SortedDbSet(ISorting<T>? sorting, bool? tracking = null)
    {
        if (sorting is null)
            sorting = new Sorting<T>();
        return sorting.DoSort(this.GetTracker(tracking));
    }

    protected virtual SaveType GetSaveType(bool? save)
    {
        // we are not tracking and have chosen to save this operation directly to DB
        if (!this.UseTracker && ((save.HasValue && save.Value) || (!save.HasValue && this.SaveOnChanges))) 
            return SaveType.SaveToDb;
        // we are tracking and have chosen to NOT save this operation
        else if (this.UseTracker && ((save.HasValue && !save.Value) || (!save.HasValue && !this.SaveOnChanges))) 
            return SaveType.TrackToDbSet;
        // we are tracking and have chosen to save this operation
        else if (this.UseTracker && ((save.HasValue && save.Value) || (!save.HasValue && this.SaveOnChanges)))
            return SaveType.SaveToDbSet;
        // we have chosen to NOT save this operation
        return SaveType.None;
    }

    protected enum SaveType
    {
        /// <summary>
        /// we have chosen to NOT save this operation
        /// </summary>
        None,
        /// <summary>
        /// we are not tracking and have chosen to save this operation directly to DB
        /// </summary>
        SaveToDb,
        /// <summary>
        /// we are tracking and have chosen to save this operation
        /// </summary>
        SaveToDbSet,
        /// <summary>
        /// we are tracking and have chosen to NOT save this operation
        /// </summary>
        TrackToDbSet
    }
}
