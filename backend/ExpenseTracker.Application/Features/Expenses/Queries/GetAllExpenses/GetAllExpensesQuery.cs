using ExpenseTracker.Application.Common.Pagination;
using ExpenseTracker.Application.DTOs.Expense;
using MediatR;

namespace ExpenseTracker.Application.Features.Expenses.Queries.GetAllExpenses;

public record GetAllExpensesQuery(PagedQuery Paging) : IRequest<PagedResult<ExpenseDto>>;