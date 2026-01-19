using ExpenseTracker.API.Contracts.V1.Common.Pagination;

namespace ExpenseTracker.API.Contracts.V1.Expense;

public class FilteredExpensesResultResponseV1
{
    public decimal TotalAmount { get; set; }
    public PagedResultResponseV1<ExpenseResponseV1> Expenses { get; set; } = default!;

}