namespace ExpenseTracker.Application.DTOs.Dashboard;

public class DailyExpenseDto
{
    public DateOnly Date { get; set; }
    public decimal TotalAmount { get; set; }
}