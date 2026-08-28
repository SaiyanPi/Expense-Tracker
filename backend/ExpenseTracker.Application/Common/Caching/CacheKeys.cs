namespace ExpenseTracker.Application.Common.Caching;

public static class CacheKeys
{
    public static string Dashboard(
        string userId,
        int year,
        int month)
        => $"dashboard:{userId}:{year}:{month}";
    
    public static string Expense(
        string userId,
        int version,
        int year,
        int month,
        int page,
        int pageSize,
        string? sortBy,
        bool sortDesc,
        string? search)
        => $"expense:{userId}:{version}:{year}:{month}:page:{page}:pageSize:{pageSize}:sortBy:{sortBy}:sortDesc:{sortDesc}:search:{search}";

    public static string Category(
        string userId,
        int version,
        int year,
        int month,
        int page,
        int pageSize,
        string? sortBy,
        bool sortDesc,
        string? search)
        => $"category:{userId}:{version}:{year}:{month}:page:{page}:pageSize:{pageSize}:sortBy:{sortBy}:sortDesc:{sortDesc}:search:{search}";

    public static string Budget(
        string userId,
        int version,
        int year,
        int month,
        int page,
        int pageSize,
        string? sortBy,
        bool sortDesc,
        string? search)
        => $"budget:{userId}:{version}:{year}:{month}:page:{page}:pageSize:{pageSize}:sortBy:{sortBy}:sortDesc:{sortDesc}:search:{search}";
    
    // public static string FilteredExpenses(
    //     string userId,
    //     int version,
    //     int year,
    //     int month,
    //     int page,
    //     int pageSize,
    //     string? sortBy,
    //     bool sortDesc,
    //     string? search,
    //     Guid? categoryId,
    //     Guid? budgetId,
    //     DateTime? startDate,
    //     DateTime? endDate,
    //     decimal? minAmount,
    //     decimal? maxAmount)
    //     => $"expense:filtered:{userId}:{version}:{year}:{month}:page:{page}:pageSize:{pageSize}:sortBy:{sortBy}:sortDesc:{sortDesc}:search:{search}:categoryId:{categoryId}:budgetId:{budgetId}:startDate:{startDate}:endDate:{endDate}:minAmount:{minAmount}:maxAmount:{maxAmount}";
    

    public static string CacheVersion(string cacheGroup, string userId)
    {
        return $"cache-version:{cacheGroup}:{userId}";
    }
    
}
