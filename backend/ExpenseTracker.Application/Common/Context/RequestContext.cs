using Microsoft.AspNetCore.Http;

namespace ExpenseTracker.Application.Common.Context;

public class RequestContext : IRequestContext
{
    private readonly IHttpContextAccessor _http;

    public RequestContext(IHttpContextAccessor http)
    {
        _http = http;
    }

    public string CorrelationId
    {
        get
        {
            // Always use the header/middleware value if available
            if (_http.HttpContext?.Items["X-Correlation-ID"] is string id)
                return id;

            // Optional: fallback to request header if middleware missed it
            var headerId = _http.HttpContext?.Request.Headers["X-Correlation-ID"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(headerId))
            {
                _http.HttpContext!.Items["X-Correlation-ID"] = headerId;
                return headerId;
            }

            // If all else fails (very rare), generate a new GUID
            var newId = Guid.NewGuid().ToString();
            _http.HttpContext?.Items.Add("X-Correlation-ID", newId);
            return newId;
        }
    }

    public string? HttpMethod =>
        _http.HttpContext?.Request.Method;

    public string? RequestPath =>
        _http.HttpContext?.Request.Path.Value;

    public string? ClientIp =>
        _http.HttpContext?.Connection.RemoteIpAddress?.ToString();

    public string? UserAgent =>
        _http.HttpContext?.Request.Headers["User-Agent"].ToString();
}
