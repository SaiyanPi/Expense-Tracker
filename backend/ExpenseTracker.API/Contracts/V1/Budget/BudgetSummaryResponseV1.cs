using ExpenseTracker.API.Contracts.V1.Common.Pagination;

namespace ExpenseTracker.API.Contracts.V1.Budget;

public class BudgetSummaryResponseV1
{
    public decimal TotalBudget { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal Remaining => TotalBudget - TotalExpenses;
    public decimal UsedPercentage => TotalBudget == 0 ? 0 : (TotalExpenses / TotalBudget) * 100;
    public bool IsOverBudget => TotalExpenses > TotalBudget;
    public PagedResultResponseV1<BudgetCategorySummaryResponseV1> Categories { get; set; } = default!;
}