using AutoMapper;
using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.Common.Pagination;
using ExpenseTracker.Application.DTOs.Budget;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;

namespace ExpenseTracker.Application.Features.Budgets.Queries.GetAllBudgetsByEmail;

public class GetAllBudgetQueryHandler : IRequestHandler<GetAllBudgetsByEmailQuery, PagedResult<BudgetDto>>
{
    private readonly IBudgetRepository _budgetRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetAllBudgetQueryHandler
    (
        IBudgetRepository budgetRepository,
        IUserRepository userRepository,
        IMapper mapper
    )
    {
        _budgetRepository = budgetRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<PagedResult<BudgetDto>> Handle(
        GetAllBudgetsByEmailQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user == null)
            throw new NotFoundException(nameof(Domain.Entities.User), request.Email);

        var query = request.Paging;

        var (budgets, totalCount) = await _budgetRepository.GetAllBudgetsByEmailAsync(
            user.Id,
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