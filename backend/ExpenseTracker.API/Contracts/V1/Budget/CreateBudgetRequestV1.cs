namespace ExpenseTracker.API.Contracts.V1.Budget;

public class CreateBudgetRequestV1
{
    public string Name { get; set; } = default!;
    public decimal Amount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? UserId { get; set; } = default!;
    public Guid? CategoryId { get; set; }
}