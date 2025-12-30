using ExpenseTracker.Domain.Common;

namespace ExpenseTracker.Application.Common.Pagination;

public record ExpenseFilter(
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    decimal? MinAmount =null, 
    decimal? MaxAmount = null,
    Guid? CategoryId = null,
    string? UserId = null
);
