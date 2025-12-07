using ExpenseTracker.Application.DTOs.Budget;
using MediatR;

namespace ExpenseTracker.Application.Features.Budgets.Queries.GetBudgetSummary;

public record GetBudgetSummaryQuery(string Email) : IRequest<List<BudgetSummaryDto>>;