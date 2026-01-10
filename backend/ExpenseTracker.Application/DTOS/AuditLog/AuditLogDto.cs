using ExpenseTracker.Domain.SharedKernel;

namespace ExpenseTracker.Application.DTOs.AuditLog;

public class AuditLogDto
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public EntityType EntityName { get; set; } = default!;
    public Guid EntityId { get; set; }

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