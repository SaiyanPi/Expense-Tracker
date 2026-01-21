using System.Diagnostics.Metrics;

namespace ExpenseTracker.Application.Common.Observability.Metrics.Business.DomainSpecific;

public static class CategoryMetrics
{
    private static readonly Meter Meter = new(MetricsConstants.MeterName);

    // buisness metric counters
    private static readonly Counter<long> CategoryCreatedCounter =
        Meter.CreateCounter<long>(
            MetricsConstants.Category.Created,
            description: "Number of categories created");

    private static readonly Counter<long> CategoryDeletedCounter =
        Meter.CreateCounter<long>(
            MetricsConstants.Category.Deleted,
            description: "Number of categories deleted");


    // ------ helpers --------------------------------------------------
    public static void CategoryCreated()
    {
        CategoryCreatedCounter.Add(
            1,
            new KeyValuePair<string, object?>("operation", "create_category")
        );
    }

    public static void CategoryDeleted()
    {
        CategoryDeletedCounter.Add(
            1,
            new KeyValuePair<string, object?>("operation", "delete_category")
        );
    }

}

// business metric counters are domain specific therefore it can be made generic unlike business latency 
// and failure metrics