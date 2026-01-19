namespace ExpenseTracker.API.Contracts.V1.Dashboard;

public class CategoryExpenseResponseV1
{
    public string Category { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
}