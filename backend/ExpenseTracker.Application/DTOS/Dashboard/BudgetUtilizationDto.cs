namespace ExpenseTracker.Application.DTOS.Dashboard;

public class BudgetUtilizationDto
{
    public string BudgetName { get; set; } = default!;
    public decimal BudgetTarget { get; set; }
    public decimal? ActualSpent { get; set; }
    public double UtilizationPercentage { get; set; }
}