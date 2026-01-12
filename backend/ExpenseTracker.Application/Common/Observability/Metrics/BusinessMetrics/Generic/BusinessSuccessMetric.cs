using System.Diagnostics.Metrics;

namespace ExpenseTracker.Application.Common.Observability.Metrics.BusinessMetrics.Generic;

public static class BusinessSuccessMetric
{
    private static readonly Meter Meter = new(MetricsConstants.MeterName);


    private static readonly Counter<long> BusinessOperationSuccessCounter =
        Meter.CreateCounter<long>(
            name: "business_operation_success_total",
            description: "Number of successfully completed business operations"
        );

    public static void RecordSuccess(
        string operationName)
    {
        BusinessOperationSuccessCounter.Add(
            1,
            new KeyValuePair<string, object?>("operation", operationName)
        );
    }
}
