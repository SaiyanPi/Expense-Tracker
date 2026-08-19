using ExpenseTracker.Application.DTOs.Expense;

namespace ExpenseTracker.Application.DTOs.Budget;

public class BudgetDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public decimal Amount { get; set; }

    // included for frontend userfriendly alert
    public decimal TotalSpent { get; set; }
    public decimal Remaining { get; set; }
    public decimal PercentageUsed { get; set; }
    public bool IsOverBudget { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public string UserId { get; set; } = default!;
    public Guid? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    
    public bool IsActive => DateTime.UtcNow >= StartDate && DateTime.UtcNow <= EndDate;
    
    //public IReadOnlyList<ExpenseDto> Expenses { get; set; } = [];

    public DateTime CreatedAt { get; set; } = default!;
    public DateTime? UpdatedAt { get; set; }
}