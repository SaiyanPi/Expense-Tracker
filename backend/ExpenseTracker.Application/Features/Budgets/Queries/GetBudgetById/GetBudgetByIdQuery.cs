using ExpenseTracker.Application.DTOs.Budget;
using MediatR;

namespace ExpenseTracker.Application.Features.Budgets.Queries.GetBudgetById;

public record GetBudgetByIdQuery(Guid Id) : IRequest<BudgetDto>;