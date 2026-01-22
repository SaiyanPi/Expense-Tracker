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
        int year,
        int month)
        => $"category:{userId}:{year}:{month}";
    
    public static string Budget(
        string userId,
        int year,
        int month)
        => $"budget:{userId}:{year}:{month}";
}
