using ExpenseTracker.Application.Common.Pagination;
using ExpenseTracker.Application.DTOs.Category;
using MediatR;

namespace ExpenseTracker.Application.Features.Expenses.Queries.GetCategorySummaryByEmail;

public record GetCategorySummaryByEmailQuery(string Email, PagedQuery Paging) : IRequest<PagedResult<CategorySummaryDto>>;