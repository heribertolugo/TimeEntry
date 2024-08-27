using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using TimeClock.Data.Models;

namespace TimeClock.Data.Repository;

/// <summary>
/// Contract for basic database operations
/// </summary>
public interface IDataRepository
{
    /// <summary>
    /// Whether to commit the current operation to database. Override SaveOnChanges. Value of null will use value from SaveOnChanges.
    /// </summary>
    public readonly static string SaveParamDescription = "";
    /// <summary>
    /// Boolean specifying whether EF Tracking will be used
    /// </summary>
    public bool UseTracker { get; set; }
    /// <summary>
    /// Boolean specifying if changes should be saved immediately.
    /// </summary>
    public bool SaveOnChanges { get; set; }
    /// <summary>
    /// Marks an item for removal and optional saves the changes.
    /// </summary>
    /// <param name="id">Database ID of item to be removed.</param>
    /// <param name="save">Whether to save the delete operation to database.<inheritdoc cref="SaveParamDescription"/></param>
    void Delete(Guid id, bool? save = null);
    /// <summary>
    /// Saves changes to this Entity to database.
    /// </summary>
    void Save();
    /// <summary>
    /// Saves all pending changes to database.
    /// </summary>
    /// <param name="saveAll"></param>
    void Save(bool saveAll);
    /// <summary>
    /// Retrieves a total count of this entity from database
    /// </summary>
    /// <returns></returns>
    int GetCount();
    /// <summary>
    /// Retrieves a total count of this entity from database asynchronously
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<int> GetCountAsync(CancellationToken token = default);
    /// <summary>
    /// Commits all pending changes to database asynchronously
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    Task SaveAsync(CancellationToken token = default);
}

/// <summary>
/// Contract for common database operations for a T IEntityModel
/// </summary>
/// <typeparam name="T">A type which implements IEntityModel</typeparam>
public interface IDataRepository<T> : IDataRepository where T : class, IEntityModel
{
    /// <summary>
    /// Whether to use tracking for this operation. This will override current tracking set for this operation if this parameter is not null.
    /// </summary>
    public readonly static string TrackingParamDescription = "";
    /// <summary>
    /// A conditional Expression to use for retrieving a collection of Entity.
    /// </summary>
    public readonly static string ExpressionParamDescription = "";
    /// <summary>
    /// Paging to specify the position first item in the collection and the size of the collection.
    /// </summary>
    public readonly static string PagingParamDescription = "";
    /// <summary>
    /// A IQueryable collection of type T Entity.
    /// </summary>
    public readonly static string ReturnIqueryCollectionDescription = "";
    /// <summary>
    /// An item of type IEntityModel
    /// </summary>
    public readonly static string ItemParamDescription = "";
    /// <summary>
    /// A CancellationToken to observe while waiting for the task to complete.
    /// </summary>
    public readonly static string CancelTokenParamDescription = "";
    /// <summary>
    /// Optional sorting to use when fetching from database.
    /// </summary>
    public readonly static string SortParamDescription = "";
    /// <summary>
    /// Retrieves an Entity by ID
    /// </summary>
    /// <param name="id">Database ID of item to be retreived.</param>
    /// <param name="tracking"><inheritdoc cref="TrackingParamDescription"/></param>
    /// <returns>An Entity from database matching the provided ID. Null if no matching Entity is found.</returns>
    T? Get(Guid id, bool? tracking = null, params Expression<Func<T, object>>[] includes);
    /// <summary>
    /// Retrieves an Entity by conditions specified.
    /// </summary>
    /// <param name="predicate"><inheritdoc cref="ExpressionParamDescription"/></param>
    /// <param name="sorting"><inheritdoc cref="SortParamDescription"/></param>
    /// <param name="tracking"><inheritdoc cref="TrackingParamDescription"/></param>
    /// <returns>A collection of Entity from database matching the provided Expression.</returns>
    IQueryable<T> Get(Expression<Func<T, bool>> predicate, ISorting<T>? sorting = null, bool? tracking = null, params Expression<Func<T, object>>[] includes);
    /// <summary>
    /// Gets all Entity's.
    /// </summary>
    /// <param name="sorting"><inheritdoc cref="SortParamDescription"/></param>
    /// <param name="tracking"><inheritdoc cref="TrackingParamDescription"/></param>
    /// <returns><inheritdoc cref="ReturnIqueryCollectionDescription"/></returns>
    IQueryable<T> GetAll(ISorting<T>? sorting = null, bool ? tracking = null, params Expression<Func<T, object>>[] includes);
    /// <summary>
    /// Retrieves an IQueryable collection of Entity by conditions specified.
    /// </summary>
    /// <param name="predicate"><inheritdoc cref="ExpressionParamDescription"/></param>
    /// <param name="paging"><inheritdoc cref="PagingParamDescription"/></param>
    /// <param name="sorting"><inheritdoc cref="SortParamDescription"/></param>
    /// <param name="tracking"><inheritdoc cref="TrackingParamDescription"/></param>
    /// <returns><inheritdoc cref="ReturnIqueryCollectionDescription"/></returns>
    IQueryable<T> GetPaged(Expression<Func<T, bool>> predicate, IPaging paging, ISorting<T>? sorting = null, bool? tracking = null, params Expression<Func<T, object>>[] includes);
    /// <summary>
    /// Retrieves a paged collection of IQueryable type T
    /// </summary>
    /// <param name="paging"><inheritdoc cref="PagingParamDescription"/></param>
    /// <param name="sorting"><inheritdoc cref="SortParamDescription"/></param>
    /// <param name="tracking"><inheritdoc cref="TrackingParamDescription"/></param>
    /// <returns><inheritdoc cref="ReturnIqueryCollectionDescription"/></returns>
    IQueryable<T> GetAllPaged(IPaging paging, ISorting<T>? sorting = null, bool? tracking = null, params Expression<Func<T, object>>[] includes);
    /// <summary>
    /// Gets a count of all items matching the predicate from the database
    /// </summary>
    /// <param name="predicate">The predicate to use to construct the where clause for determining the count</param>
    /// <returns>The number of items which match the provided predicate</returns>
    int GetCount(Expression<Func<T, bool>> predicate);
    /// <summary>
    /// Gets a count of all items matching the predicate from the database using an asynchronous operation
    /// </summary>
    /// <param name="predicate">The predicate to use to construct the where clause for determining the count</param>
    /// <returns>The number of items which match the provided predicate</returns>
    Task<int> GetCountAsync(Expression<Func<T, bool>> predicate);
    /// <summary>
    /// Adds an Entity of type T to the database collection.
    /// </summary>
    /// <param name="item"><inheritdoc cref="ItemParamDescription"/> to add to the database collection.</param>
    /// <param name="save"><inheritdoc cref="SaveParamDescription"/></param>
    void Add(T item, bool? save = null);
    /// <summary>
    /// Updates an Entity of type <typeparamref name="T"/> in the database collection.
    /// This will NOT directly push to DB, even if <paramref name="save"/> is true, and tracking is disabled.
    /// For pushing directly to DB please use <see cref=" UpdateToDbAsync(T, bool?, CancellationToken, Tuple{Func{T, object}, object}[])"/>
    /// </summary>
    /// <param name="item"><inheritdoc cref="ItemParamDescription"/> to add to update in the database collection.</param>
    /// <param name="save"><inheritdoc cref="SaveParamDescription"/></param>
    void Update(T item, bool? save = null);
    /// <summary>
    /// Removes the the Entity of type T item from the database collection. Marks the item for deletion from the database.
    /// </summary>
    /// <param name="item"><inheritdoc cref="ItemParamDescription"/> to remove from the database collection.</param>
    /// <param name="save"><inheritdoc cref="SaveParamDescription"/></param>
    void Delete(T item, bool? save = null);

    /// <summary>
    /// <inheritdoc cref="Get(Guid, bool?)"/> asynchronously
    /// </summary>
    /// <param name="id">Database ID of item to be retreived.</param>
    /// <param name="tracking"><inheritdoc cref="TrackingParamDescription"/></param>
    /// <param name="token"><inheritdoc cref="CancelTokenParamDescription"/></param>
    /// <returns><inheritdoc cref="Get(int, bool?)"/></returns>
    Task<T?> GetAsync(Guid id, bool? tracking = null, CancellationToken token = default, params Expression<Func<T, object>>[] includes);
    /// <summary>
    /// <inheritdoc cref="Get(Expression{Func{T, bool}}, bool?)"/> asynchronously
    /// </summary>
    /// <param name="predicate"><inheritdoc cref="ExpressionParamDescription"/></param>
    /// <param name="sorting"><inheritdoc cref="SortParamDescription"/></param>
    /// <param name="tracking"><inheritdoc cref="TrackingParamDescription"/></param>
    /// <param name="token"><inheritdoc cref="CancelTokenParamDescription"/></param>
    /// <returns><inheritdoc cref="Get(Expression{Func{T, bool}}, bool?)"/></returns>
    Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicate, ISorting<T>? sorting = null, bool? tracking = null, CancellationToken token = default, params Expression<Func<T, object>>[] includes);
    /// <summary>
    /// <inheritdoc cref="GetAll(bool?)"/> asynchronously
    /// </summary>
    /// <param name="sorting"><inheritdoc cref="SortParamDescription"/></param>
    /// <param name="tracking"><inheritdoc cref="TrackingParamDescription"/></param>
    /// <param name="token"><inheritdoc cref="CancelTokenParamDescription"/></param>
    /// <returns><inheritdoc cref="GetAll(bool?)"/></returns>
    Task<IEnumerable<T>> GetAllAsync(ISorting<T>? sorting = null, bool? tracking = null, CancellationToken token = default, params Expression<Func<T, object>>[] includes);
    /// <summary>
    /// <inheritdoc cref="GetPaged(Expression{Func{T, bool}}, IPaging, bool?)"/> asynchronously
    /// </summary>
    /// <param name="predicate"><inheritdoc cref="ExpressionParamDescription"/></param>
    /// <param name="paging"><inheritdoc cref="PagingParamDescription"/></param>
    /// <param name="sorting"><inheritdoc cref="SortParamDescription"/></param>
    /// <param name="tracking"><inheritdoc cref="TrackingParamDescription"/></param>
    /// <param name="token"><inheritdoc cref="CancelTokenParamDescription"/></param>
    /// <returns><inheritdoc cref="GetPaged(Expression{Func{T, bool}}, IPaging, bool?)"/></returns>
    Task<IEnumerable<T>> GetPagedAsync(Expression<Func<T, bool>> predicate, IPaging paging, ISorting<T>? sorting = null, bool? tracking = null, CancellationToken token = default, params Expression<Func<T, object>>[] includes);
    /// <summary>
    /// <inheritdoc cref="GetAllPaged(IPaging, bool?)"/> asynchronously
    /// </summary>
    /// <param name="paging"><inheritdoc cref="PagingParamDescription"/></param>
    /// <param name="sorting"><inheritdoc cref="SortParamDescription"/></param>
    /// <param name="tracking"><inheritdoc cref="TrackingParamDescription"/></param>
    /// <param name="token"><inheritdoc cref="CancelTokenParamDescription"/></param>
    /// <returns><inheritdoc cref="GetAllPaged(IPaging, bool?)"/></returns>
    Task<IEnumerable<T>> GetAllPagedAsync(IPaging paging, ISorting<T>? sorting = null, bool? tracking = null, CancellationToken token = default, params Expression<Func<T, object>>[] includes);
    /// <summary>
    /// <inheritdoc cref="Add(T, bool)"/> asynchronously
    /// </summary>
    /// <param name="item"><inheritdoc cref="Add(T, bool)" path="/param[@name='item']"/></param>
    /// <param name="save"><inheritdoc cref="SaveParamDescription"/></param>
    /// <param name="token"><inheritdoc cref="CancelTokenParamDescription"/></param>
    /// <returns><inheritdoc cref="Add(T, bool)"/></returns>
    Task AddAsync(T item, bool? save = null, CancellationToken token = default);
    /// <summary>
    /// <inheritdoc cref="Update(T, bool?)"/>
    /// </summary>
    /// <param name="item"><inheritdoc cref="Update(T, bool)" path="/param[@name='item']"/></param>
    /// <param name="save"><inheritdoc cref="SaveParamDescription"/></param>
    /// <param name="token"><inheritdoc cref="CancelTokenParamDescription"/></param>
    /// <returns></returns>
    Task UpdateAsync(T item, bool? save = null, CancellationToken token = default);
    /// <summary>
    /// Performs an update directly to database. Bypasses and ignores tracking. Changes are NOT reflected back onto the Entity.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="updates"></param>
    /// <returns></returns>
    Task UpdateToDbAsync(Guid id, Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> updates);
}
