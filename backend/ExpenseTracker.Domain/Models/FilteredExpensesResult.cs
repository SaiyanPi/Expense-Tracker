using ExpenseTracker.Domain.Entities;

namespace ExpenseTracker.Domain.Models;

public class FilteredExpensesResult
{
    public decimal TotalAmount { get; set; }
    public List<Expense> Expenses { get; set; } = new();

    // for paging info
    public int TotalCount { get; set; }
}