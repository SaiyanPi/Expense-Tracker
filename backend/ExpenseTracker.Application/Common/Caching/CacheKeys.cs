namespace ExpenseTracker.Application.Common.Caching;

public static class CacheKeys
{
    public static string Dashboard(
        string userId,
        int year,
        int month)
        => $"dashboard:{userId}:{year}:{month}";
}
