using ExpenseTracker.Application.Common.Pagination;
using ExpenseTracker.Application.DTOs.Budget;
using MediatR;

namespace ExpenseTracker.Application.Features.Budgets.Queries.GetAllBudgetsByEmail;

public record GetAllBudgetsByEmailQuery(PagedQuery Paging) : IRequest<PagedResult<BudgetDto>>;