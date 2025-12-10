using ExpenseTracker.Application.DTOs.Budget;
using MediatR;

namespace ExpenseTracker.Application.Features.Budgets.Queries.GetBudgetSummaryByEmail;

public record GetBudgetSummaryByEmailQuery(string Email) : IRequest<List<BudgetSummaryDto>>;