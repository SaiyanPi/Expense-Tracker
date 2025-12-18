namespace ExpenseTracker.Domain.Models;
public class DashboardDailyExpenseSummary
{
    public DateOnly Date { get; set; }
    public decimal TotalAmount { get; set; }
}