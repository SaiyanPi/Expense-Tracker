namespace ExpenseTracker.Domain.Models;

public class BudgetCategorySummary
{
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = default!;
    public decimal BudgetAmount { get; set; }
    public decimal ExpensesAmount { get; set; }
    public decimal Remaining => BudgetAmount - ExpensesAmount;
    public double UsedPercentage => (double)(ExpensesAmount / BudgetAmount) * 100;
    public bool IsOverBudget => ExpensesAmount > BudgetAmount;

}