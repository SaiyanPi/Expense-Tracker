namespace ExpenseTracker.API.Contracts.V1.Budget;

public class BudgetCategorySummaryResponseV1
{
    public decimal BudgetAmount { get; set; }
    public decimal ExpensesAmount { get; set; }
    public decimal Remaining => BudgetAmount - ExpensesAmount;
    public double UsedPercentage => (double)(ExpensesAmount / BudgetAmount) * 100;
    public bool IsOverBudget => ExpensesAmount > BudgetAmount;
    public Guid? CategoryId { get; set; }
    public string? CategoryName { get; set; } = default!;
}

