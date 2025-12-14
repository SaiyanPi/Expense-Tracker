using ExpenseTracker.Application.Common.Pagination;
using ExpenseTracker.Application.DTOs.Category;
using MediatR;

namespace ExpenseTracker.Application.Features.Expenses.Queries.GetCategorySummary;

public record GetCategorySummaryQuery(PagedQuery Paging) : IRequest<PagedResult<CategorySummaryDto>>;