namespace ExpenseTracker.API.Contracts.V1.Dashboard;

public class RecentExpenseResponseV1
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
}