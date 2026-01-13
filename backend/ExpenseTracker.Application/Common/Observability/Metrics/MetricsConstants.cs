namespace ExpenseTracker.Application.Common.Observability.Metrics;

public static class MetricsConstants
{
    public const string MeterName = "ExpenseTracker.Application";

    public static class Expense
    {
        // buisness metric counters
        public const string Created = "expense.created";
        public const string Deleted = "expense.deleted";
        public const string BudgetThresholdExceeded = "expense.budget.threshold_exceeded";
        
    }

    public static class Category
    {
        // buisness metric counters
        public const string Created = "category.created";
        public const string Deleted = "category.deleted";
        
    }

    public static class Budget
    {
        // buisness metric counters
        public const string Created = "budget.created";
        public const string Deleted = "budget.deleted";
        
    }
}
