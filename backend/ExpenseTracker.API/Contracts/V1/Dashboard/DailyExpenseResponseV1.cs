namespace ExpenseTracker.API.Contracts.V1.Dashboard;

public class DailyExpenseResponseV1
{
    public DateOnly Date { get; set; }
    public decimal TotalAmount { get; set; }
}