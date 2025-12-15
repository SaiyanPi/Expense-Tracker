using ExpenseTracker.Application.Common.Pagination;
using ExpenseTracker.Application.DTOs.Budget;
using MediatR;

namespace ExpenseTracker.Application.Features.Budgets.Queries.GetBudgetsSummaryByEmail;

public record GetBudgetsSummaryByEmailQuery(string Email, PagedQuery Paging) : IRequest<BudgetSummaryDto>;