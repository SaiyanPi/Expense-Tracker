using ExpenseTracker.Application.Common.Context;

namespace ExpenseTracker.API.Middleware;

public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IRequestContext requestContext)
    {
        const string HeaderName = "X-Correlation-ID";

        // Get from request or generate new
        var correlationId = context.Request.Headers[HeaderName].FirstOrDefault()
            ?? Guid.NewGuid().ToString();

        // Store for internal use
        context.Items[HeaderName] = correlationId;

        // Set response header safely
        context.Response.Headers[HeaderName] = correlationId;

        await _next(context);
    }
}