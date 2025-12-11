using ExpenseTracker.Domain.Models;

namespace ExpenseTracker.Application.DTOs.Budget;

public class BudgetSummaryDto
{
    public decimal TotalBudget { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal Remaining => TotalBudget - TotalExpenses;
    public decimal UsedPercentage => TotalBudget == 0 ? 0 : (TotalExpenses / TotalBudget) * 100;
    public bool IsOverBudget => TotalExpenses > TotalBudget;
    public IReadOnlyList<BudgetCategorySummaryDto> Categories { get; set; } = new List<BudgetCategorySummaryDto>();
}