using MediatR;

namespace ExpenseTracker.Application.Features.Budgets.Commands.UpdateBudget;

public record UpdateBudgetCommand(
    string Name,
    decimal Amount,
    DateTime StartDate,
    DateTime EndDate,
    Guid? CategoryId) : IRequest<Unit>
{
    public Guid Id { get; init; }
}