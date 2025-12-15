using ExpenseTracker.Application.Common.Pagination;
using ExpenseTracker.Application.DTOs.Category;
using MediatR;

namespace ExpenseTracker.Application.Features.Categories.Queries.GetAllCategories;

public record GetAllCategoriesQuery(PagedQuery Paging) : IRequest<PagedResult<CategoryDto>>;
