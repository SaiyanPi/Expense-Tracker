using ExpenseTracker.Application.Common.Pagination;
using ExpenseTracker.Application.DTOs.Category;
using MediatR;

namespace ExpenseTracker.Application.Features.Categories.Queries.GetAllCategoriesByEmail;

public record GetAllCategoriesByEmailQuery(string Email, PagedQuery Paging) : IRequest<PagedResult<CategoryDto>>;