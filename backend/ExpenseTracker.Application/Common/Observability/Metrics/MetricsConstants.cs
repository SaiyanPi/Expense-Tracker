namespace ExpenseTracker.Application.Common.Observability.Metrics;

public static class MetricsConstants
{
    public const string MeterName = "ExpenseTracker.Application";

    public static class Expense
    {
        // buisness metric counters
        public const string Created = "expense.created";
        public const string BudgetThresholdExceeded = "expense.budget.threshold_exceeded";
        
        // buisness latency histogram
        public const string CreationDuration = "expense.creation.duration";
    }
}
