namespace ExpenseTracker.Application.DTOs.Expense;

public class FilteredExpensesResultDto
{
    public decimal TotalAmount { get; set; }
    public List<FilteredExpenseDto> Expenses { get; set; } = new();
}