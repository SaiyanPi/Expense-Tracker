namespace ExpenseTracker.Application.DTOs.SecurityEventLog;

public class SecurityEventLogDto
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string EventType { get; set; } = null!;
    public string? UserId { get; set; }
    public string? UserEmail { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? CorrelationId { get; set; }
    public string? IpAddress { get; set; }
    public string? Endpoint { get; set; }
    public string? Outcome { get; set; } // e.g., Success, Failed, Denied
    public string? UserAgent { get; set; }
}