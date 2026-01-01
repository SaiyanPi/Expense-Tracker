namespace ExpenseTracker.Application.Common.Context;

public sealed class SystemRequestContext : IRequestContext
{
    public string CorrelationId => "SYSTEM";
    public string? HttpMethod => null;
    public string? RequestPath => null;
    public string? ClientIp => null;
    public string? UserAgent => null;

}
