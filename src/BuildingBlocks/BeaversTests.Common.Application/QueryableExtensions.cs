using System.Linq.Expressions;
using BeaversTests.Common.Application.Models;

namespace BeaversTests.Common.Application;

public static class QueryableExtensions
{
    public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Func<T, bool> predicate)
    {
        if (condition)
        {
            var method = predicate.Method;
            var parameter = Expression.Parameter(typeof(T));
            var body = Expression.Call(method, parameter);

            var expression = Expression.Lambda<Func<T, bool>>(body, parameter);
            
            return query.Where(expression);
        }

        return query;
    }
    
    public static IQueryable<T> PageBy<T>(this IQueryable<T> query, int skipCount, int maxItemsCount)
    {
        ArgumentNullException.ThrowIfNull(query);
        
        return query
            .Skip(skipCount)
            .Take(maxItemsCount);
    }
    
    public static IQueryable<T> PageBy<T>(this IQueryable<T> query, PaginatedQuery paginatedQuery)
    {
        ArgumentNullException.ThrowIfNull(query);
        
        return query.PageBy(
            paginatedQuery.SkipCount, 
            paginatedQuery.MaxItemsCount);
    }
}