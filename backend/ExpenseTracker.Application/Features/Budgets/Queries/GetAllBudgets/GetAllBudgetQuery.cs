using ExpenseTracker.Application.Common.Pagination;
using ExpenseTracker.Application.DTOs.Budget;
using MediatR;

namespace ExpenseTracker.Application.Features.Budgets.Queries.GetAllBudgets;

public record GetAllBudgetQuery(PagedQuery Paging) : IRequest<PagedResult<BudgetDto>>;