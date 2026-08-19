using ExpenseTracker.Application.Common.Pagination;
using ExpenseTracker.Application.DTOs.Expense;

namespace ExpenseTracker.Application.DTOs.Budget;

public class BudgetDetailWithExpensesDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public decimal Limit { get; set; }
    public decimal TotalSpent { get; set; }
    public decimal Remaining { get; set; }
    public decimal PercentageUsed { get; set; }
    public bool IsOverBudget { get; set; }
    public bool IsActive { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Guid? CategoryId { get; set; }
    public string? CategoryName { get; set; }

    public PagedResult<ExpenseDto> Expenses { get; set; } = default!;
}