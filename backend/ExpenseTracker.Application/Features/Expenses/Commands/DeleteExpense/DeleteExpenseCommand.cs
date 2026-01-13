using ExpenseTracker.Application.Common.Observability.Metrics;
using MediatR;

namespace ExpenseTracker.Application.Features.Expenses.Commands.DeleteExpense;

public record DeleteExpenseCommand(Guid Id) : IRequest<Unit>, ITrackBusinessLatencyAndSuccess
{
    public string OperationName => BusinessOperationNames.DeleteExpense;
}