using System.Diagnostics;
using System.Diagnostics.Metrics;
using ExpenseTracker.Application.Common.Interfaces.Services;
using MediatR;

namespace ExpenseTracker.Application.Common.Observability.Metrics.BusinessMetrics.Generic;

public sealed class BusinessMetricBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // Not a business operation where we don't want to measure success and latency metric → do nothing
        if (request is not ITrackBusinessLatency tracked)
        {
            return await next();
        }

        var stopwatch = Stopwatch.StartNew();

        try
        {
            // return await next();
            var response = await next();

            // SUCCESS -> record success metric
            BusinessSuccessMetric.RecordSuccess(
                operationName: tracked.OperationName
            );

            return response;
        }
        finally
        {
            stopwatch.Stop();

            // record latency metric
            if (!cancellationToken.IsCancellationRequested)
            {
                BusinessLatencyMetric.RecordDuration(
                    operationName: tracked.OperationName,
                    durationMs: stopwatch.Elapsed.TotalMilliseconds
                );
            }
        }
    }
}
// now we don't even have to add try-catch block in the handler to hook the business latency metric, it is done
// automatically measured in all handlers
