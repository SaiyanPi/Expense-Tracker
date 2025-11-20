namespace ExpenseTracker.Application.DTOs.Expense;

public class FilteredExpenseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = default!;
    public string UserId { get; set; } = null!;
}