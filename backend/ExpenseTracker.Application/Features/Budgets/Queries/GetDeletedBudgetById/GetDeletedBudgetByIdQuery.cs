using ExpenseTracker.Application.DTOs.Budget;
using MediatR;

namespace ExpenseTracker.Application.Features.Budgets.Queries.GetDeletedBudgetById;

public record GetDeletedBudgetByIdQuery(Guid Id) : IRequest<BudgetDto>;