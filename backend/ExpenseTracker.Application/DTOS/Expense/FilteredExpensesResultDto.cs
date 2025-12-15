using ExpenseTracker.Application.Common.Pagination;

namespace ExpenseTracker.Application.DTOs.Expense;

public class FilteredExpensesResultDto
{
    public decimal TotalAmount { get; set; }
    public PagedResult<ExpenseDto> Expenses { get; set; } = default!;

}