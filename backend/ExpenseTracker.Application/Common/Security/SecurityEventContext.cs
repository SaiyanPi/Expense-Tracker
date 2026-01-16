using Microsoft.AspNetCore.Http;

namespace ExpenseTracker.Application.Common.Security;

public static class SecurityEventContext
{
    private const string CorrelationHeaderName = "X-Correlation-ID";

    public static string? GetCorrelationId(HttpContext context)
    {
        if (context.Items.TryGetValue(CorrelationHeaderName, out var value))
            return value?.ToString();

        return null;
    }

    public static string? GetIp(HttpContext context)
    {
        return context.Connection.RemoteIpAddress?.ToString();
    }

    public static string? GetEndpoint(HttpContext context)
    {
        return $"{context.Request.Method} {context.Request.Path}";
    }

    public static string? GetUserAgent(HttpContext context)
    {
        return context.Request.Headers["User-Agent"].ToString();
    }
}