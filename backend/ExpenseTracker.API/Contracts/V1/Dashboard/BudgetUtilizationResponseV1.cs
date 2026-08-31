namespace ExpenseTracker.API.Contracts.V1.Dashboard;

public class BudgetUtilizationResponseV1
{
    public string BudgetName { get; set; } = default!;
    public decimal BudgetTarget { get; set; }
    public decimal? ActualSpent { get; set; }
    public double UtilizationPercentage { get; set; }
}