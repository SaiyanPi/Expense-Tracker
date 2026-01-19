namespace ExpenseTracker.API.Contracts.V1.Common.Pagination;

public class PagedResultRequestV1
{
    public int page {get; set;}
    public int pageSize {get; set;} 
    public string? sortBy {get; set;} = null;
    public bool sortDesc {get; set;} = false;
}