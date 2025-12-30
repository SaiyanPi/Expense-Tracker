using ExpenseTracker.Domain.Common;

namespace ExpenseTracker.Application.Common.Pagination;

public record AuditLogFilter(
    string? EntityName = null,
    string? UserId = null,
    AuditAction? Action = null,
    DateTime? StartDate = null,
    DateTime? EndDate = null
);
