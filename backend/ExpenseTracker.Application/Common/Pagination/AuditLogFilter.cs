using ExpenseTracker.Domain.SharedKernel;

namespace ExpenseTracker.Application.Common.Pagination;

public record AuditLogFilter(
    string? EntityName = null,
    string? UserId = null,
    string? Action = null,
    DateTime? StartDate = null,
    DateTime? EndDate = null
);
