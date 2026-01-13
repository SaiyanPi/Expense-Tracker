using ExpenseTracker.Application.Common.Observability.Metrics;
using ExpenseTracker.Application.DTOs.Category;
using MediatR;

namespace ExpenseTracker.Application.Features.Categories.Commands.CreateCategory;

public record CreateCategoryCommand(CreateCategoryDto CreateCategoryDto)
    : IRequest<CategoryDto>, ITrackBusinessLatencyAndSuccess
{
    public string OperationName => BusinessOperationNames.CreateCategory;
}