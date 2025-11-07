using MediatR;

namespace ExpenseTracker.Application.Features.Expenses.Commands.UpdateExpense;

public record UpdateExpenseCommand(
    Guid Id,
    string Title,
    string Description,
    decimal Amount,
    DateTime Date,
    Guid? CategoryId
) : IRequest<Unit>;
