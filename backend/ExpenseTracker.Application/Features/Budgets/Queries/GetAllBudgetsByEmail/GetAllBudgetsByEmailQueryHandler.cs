using AutoMapper;
using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Application.Common.Pagination;
using ExpenseTracker.Application.DTOs.Budget;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;

namespace ExpenseTracker.Application.Features.Budgets.Queries.GetAllBudgetsByEmail;

public class GetAllBudgetQueryHandler : IRequestHandler<GetAllBudgetsByEmailQuery, PagedResult<BudgetDto>>
{
    private readonly IBudgetRepository _budgetRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserAccessor _userAccessor;
    private readonly IMapper _mapper;

    public GetAllBudgetQueryHandler
    (
        IBudgetRepository budgetRepository,
        IUserRepository userRepository,
        IUserAccessor userAccessor,
        IMapper mapper
    )
    {
        _budgetRepository = budgetRepository;
        _userRepository = userRepository;
        _userAccessor = userAccessor;
        _mapper = mapper;
    }

    public async Task<PagedResult<BudgetDto>> Handle(
        GetAllBudgetsByEmailQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _userAccessor.UserId;

        var query = request.Paging;

        var (budgets, totalCount) = await _budgetRepository.GetAllBudgetsByEmailAsync(
            userId,
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