using System.Diagnostics.Metrics;

namespace ExpenseTracker.Application.Common.Observability.Metrics.BusinessMetrics;

public static class ExpenseMetrics
{
    private static readonly Meter Meter = new(MetricsConstants.MeterName);

    // buisness metric counters
    private static readonly Counter<long> ExpenseCreatedCounter =
        Meter.CreateCounter<long>(
            MetricsConstants.Expense.Created,
            description: "Number of expenses created");

    private static readonly Counter<long> ExpenseDeletedCounter =
        Meter.CreateCounter<long>(
            MetricsConstants.Expense.Deleted,
            description: "Number of expenses deleted");

    private static readonly Counter<long> BudgetThresholdExceededCounter =
        Meter.CreateCounter<long>(
            MetricsConstants.Expense.BudgetThresholdExceeded,
            description: "Number of times a budget threshold was exceeded");

    // ------ helpers --------------------------------------------------
    public static void ExpenseCreated()
    {
        ExpenseCreatedCounter.Add(
            1,
            new KeyValuePair<string, object?>("operation", "create_expense")
        );
    }

    public static void ExpenseDeleted()
    {
        ExpenseDeletedCounter.Add(
            1,
            new KeyValuePair<string, object?>("operation", "delete_expense")
        );
    }

    public static void BudgetThresholdExceeded()
    {
        BudgetThresholdExceededCounter.Add(
            1,
            new KeyValuePair<string, object?>("operation", "create_expense")
        );
    }

}

// business metric counters are domain specific therefore it can be made generic unlike business latency 
// and failure metrics