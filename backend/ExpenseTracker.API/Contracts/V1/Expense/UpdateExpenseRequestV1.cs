namespace ExpenseTracker.API.Contracts.V1.Expense;

public class UpdateExpenseRequestV1
{
    public string Title { get; set; } = default!;
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public Guid? CategoryId { get; set; }    // foreign key
    public Guid? BudgetId { get; set; }    // foreign key
   
}