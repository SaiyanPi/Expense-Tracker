namespace ExpenseTracker.Domain.Models;

public class ExpenseSummary
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public Guid CategoryId { get; set; }    // foreign key
    public string CategoryName { get; set; } = default!;    // included for user-friendly display
    public Guid? BudgetId { get; set; }     // foreign key
    public string? UserId { get; set; }     // foreign key
    
}