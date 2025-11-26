using ExpenseTracker.Application.DTOs.Budget;
using MediatR;

namespace ExpenseTracker.Application.Features.Budgets.Queries.GetAllBudgetsByEmail;

public record GetAllBudgetsByEmailQuery(string Email) : IRequest<IReadOnlyList<BudgetDto>>;