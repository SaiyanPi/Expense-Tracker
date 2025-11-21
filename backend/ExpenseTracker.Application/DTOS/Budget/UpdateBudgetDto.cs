namespace ExpenseTracker.Application.DTOs.Budget;

public class UpdateBudgetDto
{
    public string Name { get; set; } = default!;
    public decimal Amount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Guid? CategoryId { get; set; }
}