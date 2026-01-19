namespace ExpenseTracker.Application.DTOs.Dashboard;

public class CategoryExpenseDto
{
    public string Category { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
}