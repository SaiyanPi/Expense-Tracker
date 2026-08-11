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
        int year,
        int month)
        => $"expense:{userId}:{year}:{month}";

    public static string Category(
        string userId,
        int version,
        int year,
        int month,
        int page,
        int pageSize,
        string? sortBy,
        bool sortDesc)
        => $"category:{userId}:{version}:{year}:{month}:page:{page}:pageSize:{pageSize}:sortBy:{sortBy}:sortDesc:{sortDesc}";

    public static string Budget(
        string userId,
        int version,
        int year,
        int month,
        int page,
        int pageSize,
        string? sortBy,
        bool sortDesc)
        => $"budget:{userId}:{version}:{year}:{month}:page={page}:pageSize={pageSize}:sortBy={sortBy}:sortDesc={sortDesc}";
    

    public static string CacheVersion(string cacheGroup, string userId)
    {
        return $"cache-version:{cacheGroup}:{userId}";
    }
    
}
