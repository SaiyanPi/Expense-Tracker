using System.Diagnostics;
using ExpenseTracker.API.Metrics.ApiMetrics;
using Serilog.Context;

namespace ExpenseTracker.API.Middleware;
public class RequestTimingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestTimingMiddleware> _logger;

    public RequestTimingMiddleware(
        RequestDelegate next,
        ILogger<RequestTimingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        await _next(context);

        stopwatch.Stop();

        var elapsedMs = stopwatch.ElapsedMilliseconds;
        var path = context.Request.Path;
        var method = context.Request.Method;
        var statusCode = context.Response.StatusCode;
        var route = context.GetEndpoint()?.DisplayName ?? "unknown";

        // Push correlationId and userId into Serilog context for structured logging
        var correlationId = context.Items["X-Correlation-ID"]?.ToString() ?? context.TraceIdentifier;
        var userId = context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        using (LogContext.PushProperty("CorrelationId", correlationId))
        using (LogContext.PushProperty("UserId", userId))
        {
            // Record histogram metric(this is not a buisness latency metric, this is infrastructure metric)
            ApiMetrics.RequestDurationHistogram.Record(
                elapsedMs,
                new("http.method", method),
                new("http.route", route),
                new("http.status_code", statusCode.ToString())
            );

            // Log timing based on thresholds

            if (elapsedMs > 2000)
            {
                _logger.LogError(
                    "Very slow request: {Method} {Path} responded {StatusCode} in {ElapsedMs} ms",
                    method,
                    path,
                    statusCode,
                    elapsedMs
                );
            }
            else if (elapsedMs > 500)
            {
                _logger.LogWarning(
                    "Slow request: {Method} {Path} responded {StatusCode} in {ElapsedMs} ms",
                    method,
                    path,
                    statusCode,
                    elapsedMs
                );
            }
            else
            {
                _logger.LogInformation(
                    "Request completed: {Method} {Path} responded {StatusCode} in {ElapsedMs} ms",
                    method,
                    path,
                    statusCode,
                    elapsedMs
                );
            }
        }
    }
}
