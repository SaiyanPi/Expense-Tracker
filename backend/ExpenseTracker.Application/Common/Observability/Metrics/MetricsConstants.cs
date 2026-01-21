namespace ExpenseTracker.Application.Common.Observability.Metrics;

public static class MetricsConstants
{
    public const string MeterName = "ExpenseTracker.Application";

    // Business metrics ---------------------------------
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

    // Technical metrics ---------------------------------
    public static class Cache
    {
        public const string Hits = "cache.hits";
        public const string Misses = "cache.misses";
        public const string HitRatio = "cache.hit_ratio";
    }

    public static class Security
    {
        public const string SecurityEventsTotal = "security.events.total";
    }
}

// NOTE: Turns out we don't need to implement rate limiting metric, because 
// ASP.NET Core Rate Limiting is already instrumented with OpenTelemetry, the framework provides
// metrics like:
// # TYPE aspnetcore_rate_limiting_requests_total counter
// # HELP aspnetcore_rate_limiting_requests_total Number of requests that tried to acquire a rate limiting lease. Requests could be rejected by global or endpoint rate limiting policies. Or the request could be canceled while waiting for the lease.
// aspnetcore_rate_limiting_requests_total{otel_scope_name="Microsoft.AspNetCore.RateLimiting",aspnetcore_rate_limiting_result="acquired"} 8 1768990477637
// aspnetcore_rate_limiting_requests_total{otel_scope_name="Microsoft.AspNetCore.RateLimiting",aspnetcore_rate_limiting_policy="Auth",aspnetcore_rate_limiting_result="acquired"} 6 1768990477637
// aspnetcore_rate_limiting_requests_total{otel_scope_name="Microsoft.AspNetCore.RateLimiting",aspnetcore_rate_limiting_policy="Heavy",aspnetcore_rate_limiting_result="acquired"} 5 1768990477637
// aspnetcore_rate_limiting_requests_total{otel_scope_name="Microsoft.AspNetCore.RateLimiting",aspnetcore_rate_limiting_policy="Auth",aspnetcore_rate_limiting_result="endpoint_limiter"} 3 1768990477637
    