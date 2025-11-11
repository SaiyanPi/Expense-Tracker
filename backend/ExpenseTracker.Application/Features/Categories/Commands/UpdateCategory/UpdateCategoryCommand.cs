using MediatR;

namespace ExpenseTracker.Application.Features.Categories.Commands.UpdateCategory;

public record UpdateCategoryCommand(
    // Guid Id,
    string Name,
    string? UserId
) : IRequest<Unit>
{
    public Guid Id { get; init; } // set by controller, not from body
}


// Initially Guid Id was passed to check if category exists but we can get the
//  Id as a route parameter in the controller


// UserId is only passed for validation purposes to ensure uniqueness per user