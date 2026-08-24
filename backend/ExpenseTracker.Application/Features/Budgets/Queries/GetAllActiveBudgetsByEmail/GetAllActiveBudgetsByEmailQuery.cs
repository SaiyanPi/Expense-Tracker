using ExpenseTracker.Application.Common.Pagination;
using ExpenseTracker.Application.DTOs.Budget;
using MediatR;

namespace ExpenseTracker.Application.Features.Budgets.Queries.GetAllActiveBudgetsByEmail;

public record GetAllActiveBudgetsByEmailQuery(PagedQuery Paging) : IRequest<PagedResult<BudgetDto>>;