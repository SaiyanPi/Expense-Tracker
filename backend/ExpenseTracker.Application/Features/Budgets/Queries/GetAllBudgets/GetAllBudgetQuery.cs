using ExpenseTracker.Application.DTOs.Budget;
using MediatR;

namespace ExpenseTracker.Application.Features.Budgets.Queries.GetAllBudgets;

public record GetAllBudgetQuery() : IRequest<IReadOnlyList<BudgetDto>>;