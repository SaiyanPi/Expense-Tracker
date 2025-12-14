using AutoMapper;
using ExpenseTracker.Application.Common.Pagination;
using ExpenseTracker.Application.DTOs.Category;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;

namespace ExpenseTracker.Application.Features.Expenses.Queries.GetCategorySummary;

public class GetCategorySummaryQueryHandler : IRequestHandler<GetCategorySummaryQuery, PagedResult<CategorySummaryDto>>
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IMapper _mapper;


    public GetCategorySummaryQueryHandler(IExpenseRepository expenseRepository,
        IMapper mapper)
    {
        _expenseRepository = expenseRepository;
        _mapper = mapper;
    }

    public async Task<PagedResult<CategorySummaryDto>> Handle(
        GetCategorySummaryQuery request,
        CancellationToken cancellationToken)
    {
        var query = request.Paging;

        var (categorySummary, totalCount) = await _expenseRepository.GetCategorySummaryAsync(
            skip: query.Skip,
            take: query.EffectivePageSize,
            sortBy: query.SortBy,
            sortDesc: query.SortDesc,
            cancellationToken:cancellationToken);
        
        var mappedCategorySummary = _mapper.Map<IReadOnlyList<CategorySummaryDto>>(categorySummary);
        return new PagedResult<CategorySummaryDto>(mappedCategorySummary, totalCount, query.EffectivePage, query.EffectivePageSize);
    }
}   