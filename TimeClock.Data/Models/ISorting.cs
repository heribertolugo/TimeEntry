using System.Linq.Expressions;
using TimeClock.Data.Models;

namespace TimeClock.Data;

public interface ISorting<T> where T : class, IEntityModel
{
    IList<Sort<T>> Sorts { get; init; }

    IOrderedQueryable<T> DoSort(IQueryable<T> query);
}

public sealed class Sorting<T> : ISorting<T> where T : class, IEntityModel
{
    public Sorting()
    {
        this.Sorts = new List<Sort<T>>();
    }

    public IList<Sort<T>> Sorts { get; init; }
    public IOrderedQueryable<T> DoSort(IQueryable<T> query)
    {
        bool isInitial = true;
        IOrderedQueryable<T>? current = null;

        for (int sort = 0; sort < this.Sorts.Count; sort++)
        {
            if (isInitial)
            {
                switch (this.Sorts[sort].Order)
                {
                    case SortOrder.Descending:
                        current = query.OrderByDescending(this.Sorts[sort].SortBy);
                        break;
                    case SortOrder.Ascending:
                    default:
                        current = query.OrderBy(this.Sorts[sort].SortBy);
                        break;
                }
                isInitial = false;
                continue;
            }

            switch (Sorts[sort].Order)
            {
                case SortOrder.Descending:
                    current = current!.ThenByDescending(this.Sorts[sort].SortBy);
                    break;
                case SortOrder.Ascending:
                default:
                    current = current!.ThenBy(this.Sorts[sort].SortBy);
                    break;
            }
        }

        if (current is null)
            return query.OrderByDescending(t => t.RowId);

        return current;
    }
}

public struct Sort<T> where T : class, IEntityModel
{
    public Sort()
    {
        this.SortBy = (t) => t.RowId;
        this.Order = default;
    }

    public Expression<Func<T, object>> SortBy { get; set; }
    public SortOrder Order { get; set; }
}

public enum SortOrder
{
    Ascending,
    Descending,
}

// implementation by Jon Skeet. can be used if above implementation does not yield desired results
// To Be Determined
// see: https://stackoverflow.com/a/3119608
//public class Ordering<T>
//{
//    private readonly Func<IQueryable<T>, IOrderedQueryable<T>> transform;

//    private Ordering(Func<IQueryable<T>, IOrderedQueryable<T>> transform)
//    {
//        this.transform = transform;
//    }

//    public static Ordering<T> Create<TKey>(Expression<Func<T, TKey>> primary)
//    {
//        return new Ordering<T>(query => query.OrderBy(primary));
//    }
//    public Ordering<T> ThenBy<TKey>(Expression<Func<T, TKey>> secondary)
//    {
//        return new Ordering<T>(query => transform(query).ThenBy(secondary));
//    }

//    // and more for descending ...

//    internal IOrderedQueryable<T> Apply(IQueryable<T> query)
//    {
//        return transform(query);
//    }
//}
