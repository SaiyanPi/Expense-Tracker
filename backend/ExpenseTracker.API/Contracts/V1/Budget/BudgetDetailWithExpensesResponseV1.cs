using ExpenseTracker.API.Contracts.V1.Common.Pagination;
using ExpenseTracker.API.Contracts.V1.Expense;
using ExpenseTracker.Application.Common.Pagination;
using ExpenseTracker.Application.DTOs.Expense;

namespace ExpenseTracker.API.Contracts.V1.Budget;

public class BudgetDetailWithExpensesResponseV1
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public decimal Limit { get; set; }
    public decimal TotalSpent { get; set; }
    public decimal Remaining => Limit - TotalSpent;
    public decimal PercentageUsed => Limit == 0 ? 0 : (TotalSpent / Limit) * 100;
    public bool IsOverBudget => TotalSpent > Limit;
    public bool IsActive { get; set; }

    public PagedResultResponseV1<ExpenseResponseV1> Expenses { get; set; } = default!;
}