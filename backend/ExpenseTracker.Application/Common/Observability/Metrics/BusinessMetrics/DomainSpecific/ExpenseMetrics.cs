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

    private static readonly Counter<long> BudgetThresholdExceededCounter =
        Meter.CreateCounter<long>(
            MetricsConstants.Expense.BudgetThresholdExceeded,
            description: "Number of times a budget threshold was exceeded");
    
    // // failure and exception metric
    // public static readonly Counter<long> ExpenseOperationFailures =
    //     Meter.CreateCounter<long>(
    //         "expense_operation_failures_total",
    //         description: "Number of failed expense operations");

    // // buisness latency histogram
    // private static readonly Histogram<double> ExpenseCreationDurationHistogram =
    // Meter.CreateHistogram<double>(
    //     MetricsConstants.Expense.CreationDuration,
    //     unit: "ms",
    //     description: "Time taken to create an expense including business rules"
    // );


    // ------ helpers --------------------------------------------------
    public static void ExpenseCreated()
    {
        ExpenseCreatedCounter.Add(
            1,
            new KeyValuePair<string, object?>("operation", "create_expense")
        );
    }

    public static void BudgetThresholdExceeded()
    {
        BudgetThresholdExceededCounter.Add(
            1,
            new KeyValuePair<string, object?>("operation", "create_expense")
        );
    }

    // // unlike other this metric will be hooked in exception handling middleware
    // public static void RecordFailure(string operation, string failureType)
    // {
    //     ExpenseOperationFailures.Add(
    //         1,
    //         new KeyValuePair<string, object?>("operation", operation),
    //         new KeyValuePair<string, object?>("failure_type", failureType)
    //     );
    // }

    // public static void RecordCreationDuration(double durationMs)
    // {
    //     ExpenseCreationDurationHistogram.Record(
    //         durationMs,
    //         new KeyValuePair<string, object?>("operation", "create_expense")
    //     );
    // }

    
}

// business metric counters are domain specific therefore it can be made generic unlike business latency 
// and failure metrics