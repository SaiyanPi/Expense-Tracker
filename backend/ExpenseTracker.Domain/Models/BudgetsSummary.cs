using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Models;

namespace ExpenseTrackerDomain.Models;

public class BudgetsSummary
{
    public decimal TotalBudget { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal Remaining => TotalBudget - TotalExpenses;
    public double UsedPercentage => TotalBudget == 0? 0 : (double)(TotalExpenses / TotalBudget) * 100;
    public bool IsOverBudget => TotalExpenses > TotalBudget;
    public IReadOnlyList<BudgetCategorySummary> Categories { get; set; } = new List<BudgetCategorySummary>();

    // for paging info
    public int TotalCount { get; set; }
}