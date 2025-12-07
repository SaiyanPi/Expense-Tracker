namespace ExpenseTracker.Domain.Models;

public class CategorySummary
{
    // these are only for the budget summary
    public Guid CategoryId { get; set; }
    public decimal BudgetAmount { get; set; }
    public decimal ExpensesAmount { get; set; }
    public decimal Remaining => BudgetAmount - ExpensesAmount;
    public double UsedPercentage => (double)(ExpensesAmount / BudgetAmount) * 100;
    public bool IsOverBudget => ExpensesAmount > BudgetAmount;


    // for Category summary
    public string CategoryName { get; set; } = default!;
    public decimal TotalAmount { get; set; }
}