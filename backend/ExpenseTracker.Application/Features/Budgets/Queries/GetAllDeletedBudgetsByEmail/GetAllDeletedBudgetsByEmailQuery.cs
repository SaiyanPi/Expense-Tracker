using ExpenseTracker.Application.Common.Pagination;
using ExpenseTracker.Application.DTOs.Budget;
using MediatR;

namespace ExpenseTracker.Application.Features.Budgets.Queries.GetAllDeletedBudgetsByEmail;

public record GetAllDeletedBudgetsByEmailQuery(PagedQuery Paging) : IRequest<PagedResult<BudgetDto>>;