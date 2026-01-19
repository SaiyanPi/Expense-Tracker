using ExpenseTracker.Application.DTOs.Dashboard;

namespace ExpenseTracker.API.Contracts.V1.Dashboard;

public class DashboardSummaryResponseV1
{
    public decimal TotalExpenses { get; set; }
    public decimal TotalBudgets { get; set; }
    public CategoryExpenseResponseV1? TopCategory { get; set; }    // nullable in no expenses case

    public decimal? RemainingBudget { get; set; }   // nullable in no budgets case

    public IReadOnlyList<CategoryExpenseResponseV1> ExpenseByCategory { get; set; } = [];
    public IReadOnlyList<DailyExpenseResponseV1> DailyExpenses { get; set; } = [];

    public IReadOnlyList<RecentExpenseResponseV1> RecentExpenses { get; set; } = [];
}