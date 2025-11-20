using ExpenseTracker.Application.DTOs.Category;
using MediatR;

namespace ExpenseTracker.Application.Features.Expenses.Queries.GetCategorySummary;

public record GetCategorySummaryQuery() : IRequest<List<CategorySummaryDto>>;