using System.Diagnostics.Metrics;
using ExpenseTracker.Domain.SharedKernel;

namespace ExpenseTracker.Application.Common.Observability.Metrics.Security;
public static class SecurityEventMetric 
{
    private static readonly Meter Meter = new(MetricsConstants.MeterName);

    private static readonly Counter<long> SecurityEventsCounter =
        Meter.CreateCounter<long>(
            MetricsConstants.Security.SecurityEventsTotal,
            description: "Total number of security events");

    public static void RecordEvent(SecurityEventTypes type, string outcome)
    {
        SecurityEventsCounter.Add(
            1,
            new KeyValuePair<string, object?>("event_type", type.ToString()),
            new KeyValuePair<string, object?>("outcome", outcome)
        );
    }
}
