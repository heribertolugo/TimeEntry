using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TimeClock.Data.Helpers;
using TimeClock.Data.Models;

namespace TimeClock.Data.Repository;

public interface IUsersRepository : IDataRepository<User>
{
    IQueryable<User> GetActive(bool isActive = true, IPaging? paging = null, ISorting<User>? sorting = null, bool? tracking = null);
    Task<IEnumerable<User>> GetActiveAsync(bool isActive = true, IPaging? paging = null, ISorting<User>? sorting = null, bool? tracking = null, CancellationToken token = default, params Expression<Func<User, object>>[] includes);
    IQueryable<User> GetByAnyContains(string value, IPaging? paging = null, ISorting<User>? sorting = null, bool? tracking = null);
    User? GetByEmployeeNumber(string employeeNumber, bool? tracking = null, params Expression<Func<User, object>>[] includes);
    Task<User?> GetByEmployeeNumberAsync(string employeeNumber, bool? tracking = null, CancellationToken token = default, params Expression<Func<User, object>>[] includes);
    IQueryable<User> GetByFirstLastName(string firstName, string lastName, IPaging? paging = null, ISorting<User>? sorting = null, bool? tracking = null);
    IQueryable<User> GetByFirstName(string firstName, IPaging? paging = null, ISorting<User>? sorting = null, bool? tracking = null);
    IQueryable<User> GetByLastName(string lastName, IPaging? paging = null, ISorting<User>? sorting = null, bool? tracking = null);
    IQueryable<User> GetByNameContains(string value, IPaging? paging = null, ISorting<User>? sorting = null, bool? tracking = null);
    /// <summary>
    /// Gets uses who have at least 1 PunchEntry in the given date range. 
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="paging"></param>
    /// <param name="sorting"></param>
    /// <param name="tracking"></param>
    /// <returns></returns>
    IQueryable<User> GetByPunchedInOnDateRange(DateTime from, DateTime to, IPaging? paging = null, ISorting<User>? sorting = null, bool? tracking = null);
    User? GetByUserName(string userName, bool? tracking = null, params Expression<Func<User, object>>[] includes);
    Task<User?> GetByUserNameAsync(string userName, bool? tracking = null, CancellationToken token = default, params Expression<Func<User, object>>[] includes);
    User? GetByBarcode(string barcode, bool? tracking = null, params Expression<Func<User, object>>[] includes);
    Task<User?> GetByBarcodeAsync(string barcode, bool? tracking = null, CancellationToken token = default, params Expression<Func<User, object>>[] includes);
}

public class UsersRepository : DataRepository<User>, IUsersRepository
{
    internal UsersRepository(TimeClockContext context)
        : base(context) { }

    public User? GetByEmployeeNumber(string employeeNumber, bool? tracking = null, params Expression<Func<User, object>>[] includes)
    {
        return base.GetTracker(tracking).IncludeMultiple(includes).FirstOrDefault(u => u.EmployeeNumber == employeeNumber);
    }
    public Task<User?> GetByEmployeeNumberAsync(string employeeNumber, bool? tracking = null, CancellationToken token = default, params Expression<Func<User, object>>[] includes)
    {
        var query = base.GetTracker(tracking).IncludeMultiple(includes).Where(u => u.EmployeeNumber.Trim() == employeeNumber.Trim());
        return query.FirstOrDefaultAsync(token);
    }
    public User? GetByUserName(string userName, bool? tracking = null, params Expression<Func<User, object>>[] includes)
    {
        return base.GetTracker(tracking).IncludeMultiple(includes).FirstOrDefault(u => u.UserName == userName);
    }
    public Task<User?> GetByUserNameAsync(string userName, bool? tracking = null, CancellationToken token = default, params Expression<Func<User, object>>[] includes)
    {
        return base.GetTracker(tracking).IncludeMultiple(includes).FirstOrDefaultAsync(u => u.UserName == userName, token);
    }
    public IQueryable<User> GetByFirstName(string firstName, IPaging? paging = null, ISorting<User>? sorting = null, bool? tracking = null)
    {
        IQueryable<User> results = base.SortedDbSet(sorting, tracking).Where(u => u.FirstName == firstName);

        if (paging != null)
            return results.PageResults(paging);

        return results;
    }
    public IQueryable<User> GetByLastName(string lastName, IPaging? paging = null, ISorting<User>? sorting = null, bool? tracking = null)
    {
        IQueryable<User> results = base.SortedDbSet(sorting, tracking).Where(u => u.LastName == lastName);

        if (paging != null)
            return results.PageResults(paging);

        return results;
    }
    public IQueryable<User> GetByFirstLastName(string firstName, string lastName, IPaging? paging = null, ISorting<User>? sorting = null, bool? tracking = null)
    {
        IQueryable<User> results = base.SortedDbSet(sorting, tracking).Where(u => u.FirstName == firstName && u.LastName == lastName);

        if (paging != null)
            return results.PageResults(paging);

        return results;
    }
    public IQueryable<User> GetByNameContains(string value, IPaging? paging = null, ISorting<User>? sorting = null, bool? tracking = null)
    {
        IQueryable<User> results = base.SortedDbSet(sorting, tracking)
            .Where(u => EF.Functions.Like(u.FirstName, $"%{value}%") || EF.Functions.Like(u.LastName, $"%{value}%"));

        if (paging != null)
            return results.PageResults(paging);

        return results;
    }
    public IQueryable<User> GetActive(bool isActive = true, IPaging? paging = null, ISorting<User>? sorting = null, bool? tracking = null)
    {
        IQueryable<User> results = base.SortedDbSet(sorting, tracking).Where(x => x.IsActive == isActive);

        if (paging != null)
            return results.PageResults(paging);

        return results;
    }
    public async Task<IEnumerable<User>> GetActiveAsync(bool isActive = true, IPaging? paging = null, ISorting<User>? sorting = null, bool? tracking = null, CancellationToken token = default, params Expression<Func<User, object>>[] includes)
    {
        return await this.GetActive(isActive, paging, sorting, tracking).IncludeMultiple(includes).ToListAsync(token);
    }
    public IQueryable<User> GetByAnyContains(string value, IPaging? paging = null, ISorting<User>? sorting = null, bool? tracking = null)
    {
        IQueryable<User> results = base.SortedDbSet(sorting, tracking)
            .Where(u => EF.Functions.Like(u.FirstName, $"%{value}%") || EF.Functions.Like(u.LastName, $"%{value}%")
             || EF.Functions.Like(u.EmployeeNumber, $"%{value}%") || EF.Functions.Like(u.UserName, $"%{value}%"));

        if (paging != null)
            return results.PageResults(paging);

        return results;
    }
    //public IQueryable<User> GetByDepartment(int departmentId, IPaging? paging = null, ISorting<User>? sorting = null, bool? tracking = null)
    //{
    //    IQueryable<User> results = base.SortedDbSet(sorting, tracking).Where(x => x. == );

    //    if (paging != null)
    //        return base.PageResults(results, paging);

    //    return results;
    //}
    //public IQueryable<User> GetByLocation(int locationId)
    //{

    //}
    //public IQueryable<User> GetByDepartmentLocation(int departmentId, int locationId)
    //{

    //}
    /// <summary>
    /// <inheritdoc cref="IUsersRepository.GetByPunchedInOnDateRange(DateTime, DateTime, IPaging?, ISorting{User}?, bool?)"/>
    /// The time in the date is considered.
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="paging"></param>
    /// <param name="sorting"></param>
    /// <param name="tracking"></param>
    /// <returns></returns>
    public IQueryable<User> GetByPunchedInOnDateRange(DateTime from, DateTime to, IPaging? paging = null, ISorting<User>? sorting = null, bool? tracking = null)
    {
        IQueryable<User> results = base.SortedDbSet(sorting, tracking)
            .Where(x => x.PunchEntries.Any(p => p.CurrentState!.PunchEntriesHistory.DateTime >= from && p.CurrentState!.PunchEntriesHistory.DateTime <= to));

        if (paging != null)
            return results.PageResults(paging);

        return results;
    }

    public User? GetByBarcode(string barcode, bool? tracking = null, params Expression<Func<User, object>>[] includes)
    {
        var q1 = tracking.HasValue && tracking.Value ? base.Context.Barcodes.AsTracking() : base.Context.Barcodes.AsNoTracking();
        var entity = q1.FirstOrDefault(b => b.Value == barcode && b.DeactivatedOn == null);

        if (entity is null)
            return null;

        return base.GetTracker(tracking).IncludeMultiple(includes).FirstOrDefault(u => u.Id == entity.UserId);
    }

    public async Task<User?> GetByBarcodeAsync(string barcode, bool? tracking = null, CancellationToken token = default, params Expression<Func<User, object>>[] includes)
    {
        var q1 = tracking.HasValue && tracking.Value ? base.Context.Barcodes.AsTracking() : base.Context.Barcodes.AsNoTracking();
        Barcode? entity = null;

        entity = await q1.FirstOrDefaultAsync(b => b.Value == barcode && b.DeactivatedOn == null);

        if (entity is null)
            return null;

        return await base.GetTracker(tracking).IncludeMultiple(includes).Where(u => u.Id == entity.UserId).FirstOrDefaultAsync();
    }
}
