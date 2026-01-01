namespace ExpenseTracker.Application.Common.Context;

public interface IRequestContext
{
    string CorrelationId { get; }
    string? HttpMethod { get; }
    string? RequestPath { get; }
    string? ClientIp { get; }
    string? UserAgent { get; }
}
