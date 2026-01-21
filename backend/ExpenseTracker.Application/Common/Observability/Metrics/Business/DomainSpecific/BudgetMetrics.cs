using System.Diagnostics.Metrics;

namespace ExpenseTracker.Application.Common.Observability.Metrics.Business.DomainSpecific;

public static class BudgetMetrics
{
    private static readonly Meter Meter = new(MetricsConstants.MeterName);

    // buisness metric counters
    private static readonly Counter<long> BudgetCreatedCounter =
        Meter.CreateCounter<long>(
            MetricsConstants.Budget.Created,
            description: "Number of budgets created");

    private static readonly Counter<long> BudgetDeletedCounter =
        Meter.CreateCounter<long>(
            MetricsConstants.Budget.Deleted,
            description: "Number of budgets deleted");


    // ------ helpers --------------------------------------------------
    public static void BudgetCreated()
    {
        BudgetCreatedCounter.Add(
            1,
            new KeyValuePair<string, object?>("operation", "create_budget")
        );
    }

    public static void BudgetDeleted()
    {
        BudgetDeletedCounter.Add(
            1,
            new KeyValuePair<string, object?>("operation", "delete_budget")
        );
    }

}

// business metric counters are domain specific therefore it can be made generic unlike business latency 
// and failure metrics