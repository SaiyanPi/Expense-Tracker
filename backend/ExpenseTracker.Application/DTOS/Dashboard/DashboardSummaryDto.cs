using ExpenseTracker.Application.DTOs.Dashboard;

namespace ExpenseTracker.Application.DTOS.Dashboard;
public class DashboardSummaryDto
{
    public decimal TotalExpenses { get; set; }
    public decimal TotalBudgets { get; set; }
    public CategoryExpenseDto? TopCategory { get; set; }    // nullable in no expenses case

    public decimal? RemainingBudget { get; set; }   // nullable in no budgets case

    public List<CategoryExpenseDto> ExpenseByCategory { get; set; } = [];

    public List<DailyExpenseDto> DailyExpenses { get; set; } = [];

    public List<RecentExpenseDto> RecentExpenses { get; set; } = [];
}