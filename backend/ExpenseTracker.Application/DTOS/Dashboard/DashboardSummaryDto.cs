using ExpenseTracker.Application.DTOs.Dashboard;

namespace ExpenseTracker.Application.DTOS.Dashboard;
public class DashboardSummaryDto
{
    public decimal TotalExpenses { get; set; }
    public decimal TotalBudgets { get; set; }
    public CategoryExpenseDto? TopCategory { get; set; }    // nullable in no expenses case

    public decimal? RemainingBudget { get; set; }   // nullable in no budgets case

    public IReadOnlyList<CategoryExpenseDto> ExpenseByCategory { get; set; } = [];

    public IReadOnlyList<DailyExpenseDto> DailyExpenses { get; set; } = [];

    public IReadOnlyList<RecentExpenseDto> RecentExpenses { get; set; } = [];
}