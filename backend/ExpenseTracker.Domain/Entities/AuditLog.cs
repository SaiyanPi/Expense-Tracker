using ExpenseTracker.Domain.SharedKernel;

namespace ExpenseTracker.Domain.Entities;

public class AuditLog
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string EntityName { get; set; } = default!;
    public string EntityId { get; set; } = default!;

    public AuditAction Action { get; set; }

    public string? OldValues { get; set; }
    public string? NewValues { get; set; }

    public string? UserId { get; set; }
    public DateTime CreatedAt { get; set; }


    // for CorrelationId + request metadata
    public string CorrelationId { get; set; } = default!;
    public string? HttpMethod { get; set; }
    public string? RequestPath { get; set; }
    public string? ClientIp { get; set; }
    public string? UserAgent { get; set; }
    
}