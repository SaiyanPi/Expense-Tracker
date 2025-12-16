using MediatR;

namespace ExpenseTracker.Application.Features.Expenses.Commands.UpdateExpense;

public record UpdateExpenseCommand(
    // Guid Id,
    string Title,
    string Description,
    decimal Amount,
    DateTime Date,
    Guid? CategoryId,
    Guid? BudgetId
) : IRequest<Unit>
{
    public Guid Id { get; init; }

}
















// We use primitive properties for updating Expense entity instead of using DTO.

