using ExpenseTracker.Application.DTOs.Expense;
using MediatR;

namespace ExpenseTracker.Application.Features.Expenses.Queries.FilterExpenses;

public record FilterExpensesQuery(
    DateTime? StartDate, 
    DateTime? EndDate, 
    decimal? MinAmount, 
    decimal? MaxAmount,
    Guid? CategoryId,
    string? UserId
) : IRequest<FilteredExpensesResultDto>;