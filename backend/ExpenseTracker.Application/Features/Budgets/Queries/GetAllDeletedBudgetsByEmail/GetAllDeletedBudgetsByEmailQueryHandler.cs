using AutoMapper;
using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Application.Common.Pagination;
using ExpenseTracker.Application.DTOs.Budget;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;

namespace ExpenseTracker.Application.Features.Budgets.Queries.GetAllDeletedBudgetsByEmail;

public class GetAllDeletedBudgetsByEmailQueryHandler 
    : IRequestHandler<GetAllDeletedBudgetsByEmailQuery, PagedResult<BudgetDto>>
{
    private readonly IBudgetRepository _budgetRepository;
    private readonly IUserAccessor _userAccessor;
    private readonly IMapper _mapper;

    public GetAllDeletedBudgetsByEmailQueryHandler(
        IBudgetRepository budgetRepository,
        IUserAccessor userAccessor,
        IMapper mapper)
    {
        _budgetRepository = budgetRepository;
        _userAccessor = userAccessor;
        _mapper = mapper;
    }

     public async Task<PagedResult<BudgetDto>> Handle(
        GetAllDeletedBudgetsByEmailQuery request,
        CancellationToken cancellationToken)
    {
        // BUISNESS RULE:
        // Only user can view their own deleted budgets

        var userId = _userAccessor.UserId;
        
        var query = request.Paging;

        var(softDeletedBudgets, totalCount) = await _budgetRepository.GetAllDeletedBudgetsByEmailAsync(
            userId,
            skip: query.Skip,
            take: query.EffectivePageSize,
            sortBy: query.SortBy,
            sortDesc: query.SortDesc,
            cancellationToken: cancellationToken);
        
        var mappedBudgets = _mapper.Map<IReadOnlyList<BudgetDto>>(softDeletedBudgets);
        return new PagedResult<BudgetDto>(mappedBudgets, totalCount, query.EffectivePage, query.EffectivePageSize);
    }
}
