using System.Diagnostics.Metrics;
using ExpenseTracker.Application.Common.Observability.Metrics;

namespace ExpenseTracker.API.Metrics.ApiMetrics;

public static class ApiMetrics
{
    private static readonly Meter Meter = new(MetricsConstants.MeterName);

    public static readonly Histogram<double> RequestDurationHistogram =
        Meter.CreateHistogram<double>(
            name: "api.request.duration",
            unit: "ms",
            description: "Measures duration of HTTP requests including business operations"
        );
}

// Api/infrastructure Metrics is used by RequestTimingMiddleware and not buisness handlers