using ExpenseTracker.Domain.SharedKernel;

namespace ExpenseTracker.Application.DTOs.SecurityEventLog;

public class SecurityLogsExportDto
{

    // Type of event: LoginSuccess, LoginFailed, AccessDenied, RoleChanged, etc.
    public SecurityEventTypes EventType { get; set; } 

    // User performing the action. Nullable for failed login attempts
    public string? UserId { get; set; }
    public string? UserEmail { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    // Reuse correlation ID system
    public string? CorrelationId { get; set; }

    // Security-specific metadata
    public string? IpAddress { get; set; }
    public string? Endpoint { get; set; }
    public SecurityEventOutcome Outcome { get; set; } // e.g., Success, Failed, Denied
    public string? UserAgent { get; set; }
}