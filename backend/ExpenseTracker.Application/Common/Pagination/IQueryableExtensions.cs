using System.Linq.Expressions;

namespace ExpenseTracker.Application.Common.Pagination;

public static class IQueryableExtensions
{
    public static IQueryable<T> ApplySorting<T>(
        this IQueryable<T> query,
        string? sortBy,
        bool sortDesc)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
            return query;   // no sort

        // Get property info from entity
        var entityType = typeof(T);
        var property = entityType.GetProperty(sortBy,
            System.Reflection.BindingFlags.IgnoreCase |
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.Instance);
        
        if (property == null)
            return query;  // property not found, return unsorted
        
        // Build the expression tree: x => x.Property
        var parameter = Expression.Parameter(entityType, "x");
        var propertyAccess = Expression.MakeMemberAccess(parameter, property);
        var orderByExpression = Expression.Lambda(propertyAccess, parameter);


        string methodName = sortDesc ? "OrderByDescending" : "OrderBy";

        var resultExpression = Expression.Call(
            typeof(Queryable),
            methodName,
            new Type[] { typeof(T), property.PropertyType },
            query.Expression,
            Expression.Quote(orderByExpression));

        return query.Provider.CreateQuery<T>(resultExpression);
    }
}