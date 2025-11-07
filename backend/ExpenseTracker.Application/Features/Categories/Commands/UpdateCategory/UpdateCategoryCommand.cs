using MediatR;

namespace ExpenseTracker.Application.Features.Categories.Commands.UpdateCategory;

public record UpdateCategoryCommand(
    Guid Id,
    string Name,
    string? UserId
) : IRequest<Unit>;