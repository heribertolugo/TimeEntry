using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TimeClock.Data.Models;

namespace TimeClock.Data.Helpers;

internal static class IQueryableExtensions
{
    public static IQueryable<T> IncludeMultiple<T>(this IQueryable<T> query, params Expression<Func<T, object>>[] includes)
        where T : class, IEntityModel
    {
        IQueryable<T> temp = query;
        foreach (Expression<Func<T, object>> item in includes)
        {
            if (item.Body is MethodCallExpression expression)
            {
                if (expression.Method.Name != "Where")
                {
                    System.Collections.ObjectModel.ReadOnlyCollection<Expression> arguments = expression.Arguments;
                    if (arguments.Count > 1)
                    {
                        string navigationPath = string.Empty;
                        for (int i = 0; i < arguments.Count; i++)
                        {
                            Expression arg = arguments[i];
                            string path = arg.ToString().Substring(arg.ToString().IndexOf('.') + 1);

                            navigationPath += (i > 0 ? "." : string.Empty) + path;
                        }
                        temp = temp.Include(navigationPath);
                        continue;
                    }
                }
            }

            temp = temp.Include(item);
        }

        return temp;
    }

    public static IQueryable<T> PageResults<T>(this IQueryable<T> query, IPaging paging)
    {
        return query.Skip(paging.PageSize * (paging.PageNumber - 1)).Take(paging.PageSize);
    }
}
