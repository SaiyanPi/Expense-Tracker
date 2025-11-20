using ExpenseTracker.Application.DTOs.Category;
using MediatR;

namespace ExpenseTracker.Application.Features.Expenses.Queries.GetCategorySummaryByEmail;

public record GetCategorySummaryByEmailQuery(string Email) : IRequest<List<CategorySummaryDto>>;