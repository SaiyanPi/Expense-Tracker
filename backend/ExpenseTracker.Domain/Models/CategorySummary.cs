namespace ExpenseTracker.Domain.Models;

public class CategorySummary
{
    public string CategoryName { get; set; } = default!;
    public decimal TotalAmount { get; set; }
}