namespace ExpenseTracker.Domain.Models;

public class DashboardCategoryExpenseSummary
{
    public string Category { get; set; } = default!;
    public decimal TotalAmount { get; set; }
}