namespace ExpenseTracker.Application.Common.Auditing.Masking;

public static class AuditSensitiveFields
{
    public static readonly HashSet<string> Names = ["amount"];
}