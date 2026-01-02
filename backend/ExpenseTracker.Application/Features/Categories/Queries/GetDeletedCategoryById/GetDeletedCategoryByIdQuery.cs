using ExpenseTracker.Application.DTOs.Category;
using MediatR;

namespace ExpenseTracker.Application.Features.Categories.Queries.GetDeletedCategoryById;

public record GetDeletedCategoryByIdQuery(Guid Id) : IRequest<CategoryDto>;