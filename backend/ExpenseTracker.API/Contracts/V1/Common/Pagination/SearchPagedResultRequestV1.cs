namespace ExpenseTracker.API.Contracts.V1.Common.Pagination;

public class SearchPagedResultRequestV1: PagedResultRequestV1
{
    public string? search { get; set; } = null;

}