using ExpenseTracker.Application.Common.Pagination;
using ExpenseTracker.Application.DTOs.Category;
using MediatR;

namespace ExpenseTracker.Application.Features.Categories.Queries.GetAllDeletedCategoriesByEmail;

public record GetAllDeletedCategoriesByEmailQuery(PagedQuery Paging) : IRequest<PagedResult<CategoryDto>>;