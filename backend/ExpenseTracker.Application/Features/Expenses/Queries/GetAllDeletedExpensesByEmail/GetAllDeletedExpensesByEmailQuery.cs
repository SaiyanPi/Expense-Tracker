using ExpenseTracker.Application.Common.Pagination;
using ExpenseTracker.Application.DTOs.Expense;
using MediatR;

namespace ExpenseTracker.Application.Features.Expenses.Queries.GetAllDeletedExpensesByEmail;

public record GetAllDeletedExpensesByEmailQuery(PagedQuery Paging) : IRequest<PagedResult<ExpenseDto>>;