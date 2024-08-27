using LinqKit;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TimeClock.Data.Helpers;
using TimeClock.Data.Models;

namespace TimeClock.Data.Repository;

public interface IBarcodesRepository : IDataRepository<Barcode>
{
    Barcode? GetByUser(Guid userId, bool? tracking = null, params Expression<Func<Barcode, object>>[] includes);
    Task<Barcode?> GetByUserAsync(Guid userId, bool? tracking = null, CancellationToken cancellationToken = default, params Expression<Func<Barcode, object>>[] includes);
    Task AddAsync(Guid userId, string barcode, bool? save = null, CancellationToken token = default);
    Task AddAsync(Barcode item, Guid actionById, bool? save = null, CancellationToken token = default);
    Task AddAsync(Guid userId, Guid actionById, string barcode, bool? save = null, CancellationToken token = default);
    Task DeleteAsync(Guid id, Guid userId, bool? save = null, CancellationToken token = default);
    void Delete(Guid id, Guid userId, bool? save = null);
    Task DeleteAsync(string value, Guid userId, bool? save = null, CancellationToken token = default);
}

internal class BarcodesRepository : DataRepository<Barcode>, IBarcodesRepository
{
    public BarcodesRepository(TimeClockContext context) : base(context) { }

    public Barcode? GetByUser(Guid userId, bool? tracking = null, params Expression<Func<Barcode, object>>[] includes)
    {
        return base.GetTracker(tracking).IncludeMultiple(includes).FirstOrDefault(b => b.UserId == userId);
    }
    public Task<Barcode?> GetByUserAsync(Guid userId, bool? tracking = null, CancellationToken cancellationToken = default, params Expression<Func<Barcode, object>>[] includes)
    {
        return base.GetTracker(tracking).IncludeMultiple(includes).FirstOrDefaultAsync(b => (b.UserId == userId), cancellationToken);
    }

    public Task AddAsync(Guid userId, string barcode, bool? save = null, CancellationToken token = default)
    {
        return this.AddAsync(userId, userId, barcode, save, token);
    }
    public Task AddAsync(Guid userId, Guid actionById, string barcode, bool? save = null, CancellationToken token = default)
    {
        Barcode item = new()
        {
            ActivatedOn = DateTime.Now,
            UserId = userId,
            Value = barcode
        };

        return this.AddAsync(item, actionById, save, token);
    }

    public async Task AddAsync(Barcode item, Guid actionById, bool? save = null, CancellationToken token = default)
    {
        // this barcode already exists and it is active. we do not need to do anything
        if (await base.Context.Barcodes.AnyAsync(b => b.UserId == item.UserId && b.Value == item.Value && b.DeactivatedOn == null, cancellationToken: token))
            return;
        // deactivate all previous barcodes
        await base.Context.Barcodes.Where(b => b.UserId == item.UserId && b.DeactivatedOn == null).ForEachAsync(b => { b.DeactivatedOn = DateTime.Now; b.DeactivatedById = item.UserId; }, token);

        // we cannot add an empty barcode. add if it has value
        if (!string.IsNullOrWhiteSpace(item.Value)) 
        {
            await base.AddAsync(item, save, token);
        }
    }

    public override async Task AddAsync(Barcode item, bool? save = null, CancellationToken token = default)
    {
        await this.AddAsync(item, item.UserId, save, token);
    
    }
    public async Task DeleteAsync(string value, Guid userId, bool? save = null, CancellationToken token = default)
    {
        Guid? id = await base.Context.Barcodes.Where(b => b.Value == value && b.DeactivatedOn == null).Select(b => b.Id).FirstOrDefaultAsync(token);

        if (id.HasValue)
            await this.DeleteAsync(id.Value, userId, save, token);
    }
    public async Task DeleteAsync(Guid id, Guid userId, bool? save = null, CancellationToken token = default)
    {
        switch (base.GetSaveType(save))
        {
            case SaveType.SaveToDb:
                await base.Context.Barcodes.ExecuteUpdateAsync(setters => setters
                    .SetProperty(b => b.DeactivatedOn, DateTime.Now)
                    .SetProperty(b => b.DeactivatedById, userId), token);
                break;
            case SaveType.SaveToDbSet:
                await base.Context.Barcodes.Where(b => b.Id == id).ForEachAsync(b => { b.DeactivatedOn = DateTime.Now; b.DeactivatedById = userId; }, token);
                break;
            case SaveType.TrackToDbSet:
                var items = base.Context.Barcodes.Where(b => b.Id == id);
                await items.ForEachAsync(b => { b.DeactivatedOn = DateTime.Now; b.DeactivatedById = userId; }, token);
                base.Context.Barcodes.UpdateRange(await items.ToListAsync(token));
                break;
            case SaveType.None:
            default:
                break;
        }
    }

    public void Delete(Guid id, Guid userId, bool? save = null)
    {
        switch (base.GetSaveType(save))
        {
            case SaveType.SaveToDb:
                base.Context.Barcodes.ExecuteUpdate(setters => setters
                    .SetProperty(b => b.DeactivatedOn, DateTime.Now)
                    .SetProperty(b => b.DeactivatedById, userId));
                break;
            case SaveType.SaveToDbSet:
                base.Context.Barcodes.Where(b => b.Id == id).ForEach(b => { b.DeactivatedOn = DateTime.Now; b.DeactivatedById = userId; });
                break;
            case SaveType.TrackToDbSet:
                var items = base.Context.Barcodes.Where(b => b.Id == id);
                items.ForEach(b => { b.DeactivatedOn = DateTime.Now; b.DeactivatedById = userId; });
                base.Context.Barcodes.UpdateRange(items.ToList());
                break;
            case SaveType.None:
            default:
                break;
        }
    }

    [Obsolete("Please use overload which accepts userId")]
    public override void Delete(Guid id, bool? save = null)
    {
        throw new NotImplementedException("Barcode cannot be deleted this way");
    }

    [Obsolete("Please use overload which accepts userId")]
    public override void Delete(Barcode item, bool? save = null)
    {
        this.Delete(item.Id, save);
    }
}
