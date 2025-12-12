namespace ExpenseTracker.Application.Common.Pagination;

public class PaginatedResult<T>
{
    public IReadOnlyList<T> Items { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }

    public bool HasNext => PageNumber < TotalPages;
    public bool HasPrevious => PageNumber > 1;

    public PaginatedResult(IReadOnlyList<T> items, int count, int pageNumber, int pageSize)
    {
        Items = items;
        TotalCount = count;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
    }
}
