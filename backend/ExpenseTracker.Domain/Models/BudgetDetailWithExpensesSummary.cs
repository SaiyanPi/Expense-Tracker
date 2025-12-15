
namespace ExpenseTracker.Domain.Models;

public class BudgetDetailWithExpensesSummary
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public decimal Limit { get; set; }
    public decimal TotalSpent { get; set; }
    public decimal Remaining => Limit - TotalSpent;
    public decimal PercentageUsed => Limit == 0 ? 0 : (TotalSpent / Limit) * 100;
    public bool IsOverBudget => TotalSpent > Limit;
    public bool IsActive { get; set; }
    public IReadOnlyList<ExpenseSummary> Expenses { get; set; } = default!;

    // for paging info
    public int TotalCount { get; set; }
}
