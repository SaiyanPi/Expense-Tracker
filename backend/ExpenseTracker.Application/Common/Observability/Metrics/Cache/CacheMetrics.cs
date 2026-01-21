namespace ExpenseTracker.Application.Common.Observability.Metrics.Cache;

public static class CacheMetrics
{
    private static long _hits;
    private static long _misses;

    public static void RecordHit() => 
        Interlocked.Increment(ref _hits);

    public static void RecordMiss() => 
        Interlocked.Increment(ref _misses);

    public static long Hits => 
        Volatile.Read(ref _hits);
    public static long Misses => 
        Volatile.Read(ref _misses);

    public static double HitRatio
    {
        get
        {
            var total = Hits + Misses;
            return total == 0 ? 0 : (double)Hits / total;
        }
    }
}
