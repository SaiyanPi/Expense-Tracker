using MediatR;

namespace ExpenseTracker.Application.Features.Categories.Commands.RestoreDeletedCategoryById;

public record RestoreDeletedCategoryByIdCommand(Guid Id) : IRequest<Unit>;