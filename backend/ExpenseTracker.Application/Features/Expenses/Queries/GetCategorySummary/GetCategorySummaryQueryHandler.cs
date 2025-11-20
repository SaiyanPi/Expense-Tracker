using ExpenseTracker.Application.DTOs.Category;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;

namespace ExpenseTracker.Application.Features.Expenses.Queries.GetCategorySummary;

public class GetCategorySummaryQueryHandler : IRequestHandler<GetCategorySummaryQuery, List<CategorySummaryDto>>
{
    private readonly IExpenseRepository _expenseRepository;

    public GetCategorySummaryQueryHandler(IExpenseRepository expenseRepository)
    {
        _expenseRepository = expenseRepository;
    }

    public async Task<List<CategorySummaryDto>> Handle(GetCategorySummaryQuery request, CancellationToken cancellationToken)
    {
        var categorySummary = await _expenseRepository.GetCategorySummaryAsync(cancellationToken);
        return categorySummary.Select(cs => new CategorySummaryDto
        {
            CategoryName = cs.CategoryName,
            TotalAmount = cs.TotalAmount
        }).ToList();
    }
}   