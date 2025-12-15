using AutoMapper;
using ExpenseTracker.Application.Common.Pagination;
using ExpenseTracker.Application.DTOs.Budget;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;

namespace ExpenseTracker.Application.Features.Budgets.Queries.GetAllBudgets;

public class GetAllBudgetQueryHandler : IRequestHandler<GetAllBudgetQuery, PagedResult<BudgetDto>>
{
    private readonly IBudgetRepository _budgetRepository;
    private readonly IMapper _mapper;

    public GetAllBudgetQueryHandler(
        IBudgetRepository budgetRepository,
        IMapper mapper)
    {
        _budgetRepository = budgetRepository;
        _mapper = mapper;
    }

    public async Task<PagedResult<BudgetDto>> Handle(
        GetAllBudgetQuery request,
        CancellationToken cancellationToken)
    {
        var query = request.Paging;

        var (budgets, totalCount) = await _budgetRepository.GetAllAsync(
            skip: query.Skip,
            take: query.EffectivePageSize,
            sortBy: query.SortBy,
            sortDesc: query.SortDesc,
            cancellationToken: cancellationToken);
        
        var mappedBudgets = _mapper.Map<IReadOnlyList<BudgetDto>>(budgets);
        return new PagedResult<BudgetDto>(
           mappedBudgets,
           totalCount,
           query.EffectivePage,
           query.EffectivePageSize);
    }
}