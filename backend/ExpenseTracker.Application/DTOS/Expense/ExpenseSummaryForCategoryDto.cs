namespace ExpenseTracker.Application.DTOS.Expense;

public class ExpenseSummaryForCategoryDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = default!;  
    public Guid? BudgetId { get; set; }     
    public string UserId { get; set; } = default!; 
}