using ExpenseTracker.Application.Common.Observability.Metrics;
using MediatR;

namespace ExpenseTracker.Application.Features.Budgets.Commands.DeleteBudget;

public record DeleteBudgetCommand(Guid Id) : IRequest<Unit>, ITrackBusinessLatencyAndSuccess
{
    public string OperationName => BusinessOperationNames.DeleteBudget;
}