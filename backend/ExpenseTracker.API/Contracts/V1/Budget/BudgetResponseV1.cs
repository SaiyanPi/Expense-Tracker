namespace ExpenseTracker.API.Contracts.V1.Budget;

public class BudgetResponseV1
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public decimal Amount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string UserId { get; set; } = default!;
    public Guid? CategoryId { get; set; }
    public bool IsActive => DateTime.UtcNow >= StartDate && DateTime.UtcNow <= EndDate;
    
    //public IReadOnlyList<ExpenseDto> Expenses { get; set; } = [];
}