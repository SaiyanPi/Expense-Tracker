namespace ExpenseTracker.API.Contracts.V1.Category;

public class CategorySummaryResponseV1
{
    public string CategoryName { get; set; } = default!;
    public decimal TotalAmount { get; set; }
}