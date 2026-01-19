namespace ExpenseTracker.API.Contracts.V1.Budget;

public class UpdateBudgetRequestV1
{
    public string Name { get; set; } = default!;
    public decimal Amount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Guid? CategoryId { get; set; }
}