using MediatR;

namespace ExpenseTracker.Application.Features.Expenses.Commands.RestoreDeletedExpenseById;

public record RestoreDeletedExpenseByIdCommand(Guid Id) : IRequest<Unit>;