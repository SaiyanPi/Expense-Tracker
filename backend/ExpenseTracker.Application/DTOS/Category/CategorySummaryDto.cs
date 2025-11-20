namespace ExpenseTracker.Application.DTOs.Category;

public class CategorySummaryDto
{
    public string CategoryName { get; set; } = default!;
    public decimal TotalAmount { get; set; }
}