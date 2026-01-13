using ExpenseTracker.Application.Common.Context;
using Serilog.Context;

namespace ExpenseTracker.API.Middleware;

public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    public const string HeaderName = "X-Correlation-ID";

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {

        // Get from request or generate new
        var correlationId = context.Request.Headers[HeaderName].FirstOrDefault()
            ?? Guid.NewGuid().ToString();

        // Single source of truth. Store for internal use
        context.Items[HeaderName] = correlationId;

        // Set response header safely
        context.Response.Headers[HeaderName] = correlationId;

        // await _next(context);
        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await _next(context);
        }
    }
}