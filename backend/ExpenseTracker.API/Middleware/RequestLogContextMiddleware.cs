using System.Security.Claims;
using Serilog.Context;

namespace ExpenseTracker.API.Middleware;
public class RequestLogContextMiddleware
{
    private readonly RequestDelegate _next;

    public RequestLogContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        // var correlationId = context.TraceIdentifier;
        // read from source of truth(CurrelationIdMiddleware)
        var correlationId = context.Items[CorrelationIdMiddleware.HeaderName]?.ToString();

        context.Response.Headers["X-Correlation-ID"] = correlationId;

        var userId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        // Push into Serilog context
        using (LogContext.PushProperty("CorrelationId", correlationId))
        using (LogContext.PushProperty("UserId", userId))
        {
            await _next(context);
        }
    }
}
