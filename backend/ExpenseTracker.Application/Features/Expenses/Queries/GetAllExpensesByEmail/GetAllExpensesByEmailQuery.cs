using ExpenseTracker.Application.Common.Pagination;
using ExpenseTracker.Application.DTOs.Expense;
using MediatR;

namespace ExpenseTracker.Application.Features.Expenses.Queries.GetAllExpensesByEmail;

public record GetAllExpensesByEmailQuery(PagedQuery Paging) : IRequest<PagedResult<ExpenseDto>>;