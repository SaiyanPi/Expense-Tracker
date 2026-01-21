using System.Diagnostics.Metrics;
using ExpenseTracker.Application.Common.Observability.Metrics.Cache;

namespace ExpenseTracker.Application.Common.Observability.Metrics.Cache;

public static class CacheMetricsSetup
{
    public static void RegisterCacheMetrics()
    {
        var meter = new Meter(MetricsConstants.MeterName);

        meter.CreateObservableCounter(
            MetricsConstants.Cache.Hits,
            () => CacheMetrics.Hits,
            description: "Total cache hits");

        meter.CreateObservableCounter(
            MetricsConstants.Cache.Misses,
            () => CacheMetrics.Misses,
            description: "Total cache misses");

        meter.CreateObservableGauge(
            MetricsConstants.Cache.HitRatio,
            () => CacheMetrics.HitRatio,
            description: "Cache hit ratio");
    }
}
