using AutoMapper;
using ExpenseTracker.Application.DTOs.Category;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;

namespace ExpenseTracker.Application.Features.Expenses.Queries.GetCategorySummary;

public class GetCategorySummaryQueryHandler : IRequestHandler<GetCategorySummaryQuery, List<CategorySummaryDto>>
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IMapper _mapper;


    public GetCategorySummaryQueryHandler(IExpenseRepository expenseRepository,
        IMapper mapper)
    {
        _expenseRepository = expenseRepository;
        _mapper = mapper;
    }

    public async Task<List<CategorySummaryDto>> Handle(GetCategorySummaryQuery request, CancellationToken cancellationToken)
    {
        var categorySummary = await _expenseRepository.GetCategorySummaryAsync(cancellationToken);
        
        return new List<CategorySummaryDto> { _mapper.Map<CategorySummaryDto>(categorySummary) };
    }
}   