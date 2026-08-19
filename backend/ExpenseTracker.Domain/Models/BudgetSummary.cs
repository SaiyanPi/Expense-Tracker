namespace ExpenseTrackerDomain.Models;


// created to include budget's amount spending status in the GetAllBudgetsByEmailAsync. This was
// needed in the frontend for userfriendly UI
// if we were to pass only the list of budgets, this is not needed at all.
// this is different from BudgetsSummary
public class BudgetSummary
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public decimal Amount { get; set; }

    public decimal TotalSpent { get; set; }
    public decimal Remaining { get; set; }
    public decimal PercentageUsed { get; set; }
    public bool IsOverBudget { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public string UserId { get; set; } = default!;
    public Guid? CategoryId { get; set; }
    public string? CategoryName { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}