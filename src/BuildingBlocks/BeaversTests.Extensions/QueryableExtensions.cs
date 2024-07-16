using BeaversTests.Common.Application.Models;

namespace BeaversTests.Extensions;

public static class QueryableExtensions
{
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