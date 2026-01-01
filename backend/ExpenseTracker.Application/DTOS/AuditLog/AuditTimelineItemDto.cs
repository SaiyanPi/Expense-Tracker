namespace ExpenseTracker.Application.DTOs.AuditLog;

public sealed record AuditTimelineItemDto(
    Guid Id,
    string Action,
    string? OldValues,
    string? NewValues,
    DateTime CreatedAt,
    string? UserId,
    string? CorrelationId,
    string? HttpMethod,
    string? RequestPath
);