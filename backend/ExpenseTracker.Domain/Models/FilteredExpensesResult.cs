using ExpenseTracker.Domain.Entities;

namespace ExpenseTracker.Domain.Models;

public class FilteredExpensesResult
{
    public decimal TotalAmount { get; set; }
    public IReadOnlyList<Expense> Expenses { get; set; } = default!;

    // for paging info
    public int TotalCount { get; set; }
}