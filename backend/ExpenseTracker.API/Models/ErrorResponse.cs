namespace ExpenseTracker.API.Models;

public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string Error { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public object? Details { get; set; }
    public string TraceId { get; set; } = string.Empty;
    public string CorrelationId { get; set; } = string.Empty;
}