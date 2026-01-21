namespace ExpenseTracker.Application.Common.Retention;

public class LogRetentionOptions
{
    public int RetentionDays { get; set; } = 90;
    public int CleanupIntervalHours { get; set; } = 24;
}