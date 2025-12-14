namespace ExpenseTracker.Application.Common.Pagination;

public record PagedQuery(
    int Page = 1,
    int PageSize = 20,
    string? SortBy = null,
    bool SortDesc = false)
{
    public int EffectivePage => Page < 1 ? 1 : Page;
    public int EffectivePageSize => PageSize < 1 ? 10 : (PageSize > 200 ? 200 : PageSize); // cap
    public int Skip => (EffectivePage - 1) * EffectivePageSize;
}
