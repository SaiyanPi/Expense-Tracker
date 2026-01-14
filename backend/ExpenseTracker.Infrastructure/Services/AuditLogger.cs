using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Infrastructure.Services;

public interface IAuditLogger
{
    Task LogSecurityEventAsync(
        SecurityEventType eventType,
        string? userId,
        string? correlationId,
        string details,
        Dictionary<string, object>? additionalData = null);
}

public enum SecurityEventType
{
    // Authentication Events
    LoginSuccess,
    LoginFailure,
    Logout,
    PasswordChange,
    PasswordResetRequest,
    PasswordReset,

    // Authorization Events
    AccessDenied,
    PermissionGranted,
    PermissionRevoked,
    RoleAssigned,
    RoleRemoved,

    // User Management Events
    UserCreated,
    UserUpdated,
    UserDeleted,
    UserLocked,
    UserUnlocked,

    // Sensitive Operations
    SensitiveDataAccessed,
    AdminActionPerformed,

    // Security Incidents
    SuspiciousActivity,
    BruteForceAttempt,
    InvalidTokenUsed
}

public class AuditLogger : IAuditLogger
{
    private readonly ILogger<AuditLogger> _logger;

    public AuditLogger(ILogger<AuditLogger> logger)
    {
        _logger = logger;
    }

    public async Task LogSecurityEventAsync(
        SecurityEventType eventType,
        string? userId,
        string? correlationId,
        string details,
        Dictionary<string, object>? additionalData = null)
    {
        // Create structured log entry
        var logEntry = new
        {
            EventType = eventType.ToString(),
            UserId = userId,
            CorrelationId = correlationId,
            Details = details,
            Timestamp = DateTime.UtcNow,
            AdditionalData = additionalData ?? new Dictionary<string, object>()
        };

        // Use structured logging - Serilog will enrich this automatically
        using var scope = _logger.BeginScope(new Dictionary<string, object?>
        {
            ["EventCategory"] = "Security",
            ["SecurityEventType"] = eventType.ToString(),
            ["CorrelationId"] = correlationId,
            ["UserId"] = userId
        });

        _logger.LogInformation(
            "Security Event: {EventType} - {Details} - User: {UserId}",
            eventType.ToString(),
            details,
            userId ?? "Anonymous"
        );

        // In a production system, you might also:
        // - Write to a separate audit database table
        // - Send to SIEM system (Splunk, ELK, etc.)
        // - Trigger alerts for critical events
        // - Store in immutable audit log

        await Task.CompletedTask; // Placeholder for async operations if needed
    }
}
