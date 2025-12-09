using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Models;

namespace ExpenseTrackerDomain.Models;

public class BudgetSummary
{
    public decimal TotalBudget { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal Remaining => TotalBudget - TotalExpenses;
    public double UsedPercentage => (double)(TotalExpenses / TotalBudget) * 100;
    public bool IsOverBudget => TotalExpenses > TotalBudget;
    public IReadOnlyList<BudgetCategorySummary> Categories { get; set; } = new List<BudgetCategorySummary>();
}