using ExpenseTracker.Application.DTOs.Expense;

namespace ExpenseTracker.Application.DTOs.Budget;

public class BudgetDetailWithExpensesDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public decimal Limit { get; set; }
    public decimal TotalSpent { get; set; }
    public decimal Remaining => Limit - TotalSpent;
    public decimal PercentageUsed => Limit == 0 ? 0 : (TotalSpent / Limit) * 100;
    public bool IsOverBudget => TotalSpent > Limit;
    public bool IsActive { get; set; }

    public List<ExpenseDto> Expenses { get; set; } = new();
}