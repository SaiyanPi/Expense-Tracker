namespace ExpenseTracker.Application.DTOs.Dashboard;

public class RecentExpenseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public decimal TotalAmount { get; set; }
    public DateTime Date { get; set; }
}