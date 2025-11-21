namespace ExpenseTracker.Application.DTOs.Budget;

public class CreateBudgetDto
{
    public string Name { get; set; } = default!;
    public decimal Amount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string UserId { get; set; } = default!;
    public Guid? CategoryId { get; set; }
}