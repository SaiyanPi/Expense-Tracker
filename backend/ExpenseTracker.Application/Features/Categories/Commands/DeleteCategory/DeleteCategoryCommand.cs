using ExpenseTracker.Application.Common.Observability.Metrics;
using MediatR;

namespace ExpenseTracker.Application.Features.Categories.Commands.DeleteCategory;

public record DeleteCategoryCommand(Guid Id) : IRequest<Unit>, ITrackBusinessLatencyAndSuccess
{
    public string OperationName => BusinessOperationNames.DeleteCategory;
}