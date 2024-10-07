using System.Linq.Expressions;

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
}