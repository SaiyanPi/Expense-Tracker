using ExpenseTracker.Domain.SharedKernel;

namespace ExpenseTracker.Application.Common.Pagination;

public record SecurityEventLogFilter(
    string? EventType = null,
    string? Outcome = null,
    string? UserId = null,
    string? UserEmail = null,
    DateTime? StartDate = null,
    DateTime? EndDate = null
);
