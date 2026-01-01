namespace ExpenseTracker.Application.Common.Auditing.Retention;

public class AuditLogRetentionOptions
{
    public int RetentionDays { get; set; } = 90;
    public int CleanupIntervalHours { get; set; } = 24;
}