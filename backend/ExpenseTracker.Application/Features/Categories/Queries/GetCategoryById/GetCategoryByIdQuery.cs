using ExpenseTracker.Application.DTOs.Category;
using MediatR;

namespace ExpenseTracker.Application.Features.Categories.Queries.GetCategoryById;

public record GetCategoryByIdQuery(Guid Id) : IRequest<CategoryDto>;