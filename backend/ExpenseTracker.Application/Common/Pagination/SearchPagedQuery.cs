using ExpenseTracker.Application.Common.Pagination;

namespace ExpenseTracker.Application.Common.Pagination;
public record SearchPagedQuery(
    string? Search = null,
    int Page = 1,
    int PageSize = 20,
    string? SortBy = null,
    bool SortDesc = false)
    : PagedQuery(Page, PageSize, SortBy, SortDesc);