using ExpenseTracker.Application.Common.Pagination;
using ExpenseTracker.Application.DTOs.Expense;
using MediatR;

namespace ExpenseTracker.Application.Features.Expenses.Queries.FilterExpenses;

// public record FilterExpensesQuery(
//     DateTime? StartDate, 
//     DateTime? EndDate, 
//     decimal? MinAmount, 
//     decimal? MaxAmount,
//     Guid? CategoryId,
//     string? UserId,
    
//     PagedQuery Paging
// ) : IRequest<FilteredExpensesResultDto>;

public record FilterExpensesQuery(ExpenseFilter Filter, PagedQuery Paging) : IRequest<FilteredExpensesResultDto>;