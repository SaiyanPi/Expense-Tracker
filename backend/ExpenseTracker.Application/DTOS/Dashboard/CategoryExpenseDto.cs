namespace ExpenseTracker.Application.DTOs.Dashboard;

public class CategoryExpenseDto
{
    public string Category { get; set; } = default!;
    public decimal TotalAmount { get; set; }
}