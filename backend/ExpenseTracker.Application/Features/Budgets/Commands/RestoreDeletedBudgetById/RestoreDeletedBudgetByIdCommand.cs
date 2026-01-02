using MediatR;

namespace ExpenseTracker.Application.Features.Budgets.Commands.RestoreDeletedBudgetById;

public record RestoreDeletedBudgetByIdCommand(Guid Id) : IRequest<Unit>;