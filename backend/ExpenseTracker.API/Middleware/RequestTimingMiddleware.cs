using System.Diagnostics;

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

    public async Task Invoke(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        await _next(context);

        stopwatch.Stop();

        var elapsedMs = stopwatch.ElapsedMilliseconds;
        var path = context.Request.Path;
        var method = context.Request.Method;
        var statusCode = context.Response.StatusCode;

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
