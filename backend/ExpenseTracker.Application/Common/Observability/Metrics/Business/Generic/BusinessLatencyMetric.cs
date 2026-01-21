using System.Diagnostics.Metrics;

namespace ExpenseTracker.Application.Common.Observability.Metrics.Business.Generic;

public static class BusinessLatencyMetric
{
    private static readonly Meter Meter = new(MetricsConstants.MeterName);

    private static readonly Histogram<double> OperationDurationHistogram =
        Meter.CreateHistogram<double>(
            name: "business_operation_duration",
            unit: "ms",
            description: "Duration of business operations ");

    public static void RecordDuration(
        string operationName,
        double durationMs)
    {
        OperationDurationHistogram.Record(
            durationMs,
            new KeyValuePair<string, object?>("operation", operationName)
        );
    }
}

// business latency metric is not a domain specific metric therefore it can be made made generic.
// Note that this is not api/infrastructure latency metric.

// it is measured in business handlers but since it is made generic with the BusinessMetricBehavior
// latency is measured automatically in all handlers that implement ITrackBusinessLatency.

// ITrackBusinessLatency is a marker interface that allows us to pick in which handlers we want to measure
// business latency. Automatically measuring latency in all handlers is wrong