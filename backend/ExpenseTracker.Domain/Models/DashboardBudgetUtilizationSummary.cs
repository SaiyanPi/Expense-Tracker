namespace ExpenseTracker.Domain.Models;
public class DashboardBudgetUtilizationSummary
{
    public string BudgetName { get; set; } = default!;
    public decimal BudgetTarget { get; set; }
    public decimal? ActualSpent { get; set; }
    public double UtilizationPercentage => BudgetTarget == 0 ? 0 : (double)((ActualSpent ?? 0m) / BudgetTarget) * 100;
}