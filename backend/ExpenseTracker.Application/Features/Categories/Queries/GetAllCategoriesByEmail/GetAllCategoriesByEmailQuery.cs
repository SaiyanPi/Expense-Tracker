using ExpenseTracker.Application.Common.Pagination;
using ExpenseTracker.Application.DTOs.Category;
using MediatR;

namespace ExpenseTracker.Application.Features.Categories.Queries.GetAllCategoriesByEmail;

public record GetAllCategoriesByEmailQuery(SearchPagedQuery Paging) : IRequest<PagedResult<CategoryDto>>;