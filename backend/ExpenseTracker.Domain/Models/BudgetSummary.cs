namespace ExpenseTrackerDomain.Models;

public class BudgetSummary
{
    public Guid BudgetId { get; set; }
    public string BudgetName { get; set; } = default!;
    public decimal TotalAmount { get; set; }
    public decimal SpentAmount { get; set; }
    public decimal RemainingAmount => TotalAmount - SpentAmount;
}