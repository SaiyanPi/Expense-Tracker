using System.Diagnostics.Metrics;

namespace ExpenseTracker.Application.Common.Observability.Metrics.Business.Generic;

public static class BusinessFailureMetric
{
    private static readonly Meter Meter = new(MetricsConstants.MeterName);

    private static readonly Counter<long> OperationFailures =
        Meter.CreateCounter<long>(
            "business_operation_failures_total",
            description: "Number of failed business operations"
        );

    public static void RecordFailure(string operation, string failureType)
    {
        OperationFailures.Add(
            1,
            new KeyValuePair<string, object?>("operation", operation),
            new KeyValuePair<string, object?>("status", "failure"),
            new KeyValuePair<string, object?>("failure_type", failureType)
        );
    }
}

// business failure and exception metric is not a domain specific metric therefore i've made it generic

// it is measured in exceptionhandling middleware or in pipeline