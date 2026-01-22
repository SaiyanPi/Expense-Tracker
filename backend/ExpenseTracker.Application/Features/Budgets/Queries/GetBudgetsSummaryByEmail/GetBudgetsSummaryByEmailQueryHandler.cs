using AutoMapper;
using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Application.Common.Pagination;
using ExpenseTracker.Application.DTOs.Budget;
using ExpenseTracker.Application.Features.Budgets.Queries.GetBudgetsSummaryByEmail;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;

namespace ExpenseTracker.Application.Features.Budgets.Queries.GetBudgetsSummary;

public class GetBudgetsSummaryByEmailQueryHandler : IRequestHandler<GetBudgetsSummaryByEmailQuery, BudgetSummaryDto>
{
    private readonly IBudgetRepository _budgetRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserAccessor _userAccessor;
    private readonly IMapper _mapper;


    public GetBudgetsSummaryByEmailQueryHandler(
        IBudgetRepository budgetRepository,
        IUserRepository userRepository,
        IUserAccessor userAccessor,
        IMapper mapper)
    {
        _budgetRepository = budgetRepository;
        _userRepository = userRepository;
        _userAccessor = userAccessor;
        _mapper = mapper;
    }

    public async Task<BudgetSummaryDto> Handle(GetBudgetsSummaryByEmailQuery request, CancellationToken cancellationToken)
    {
        var userId = _userAccessor.UserId;

        var query = request.Paging;

        var budgetSummaryByEmail = await _budgetRepository.GetBudgetsSummaryByEmailAsync(
            userId, 

            skip: query.Skip,
            take: query.EffectivePageSize,
            sortBy: query.SortBy,
            sortDesc: query.SortDesc,
            cancellationToken: cancellationToken);

        var mappedBudgetSummaryCategories = _mapper.Map<IReadOnlyList<BudgetCategorySummaryDto>>(budgetSummaryByEmail.Categories);
        
        var pagedBudgetSummaryCategories = new PagedResult<BudgetCategorySummaryDto>(
            mappedBudgetSummaryCategories,
            budgetSummaryByEmail.TotalCount,
            query.EffectivePage,
            query.EffectivePageSize);
        
        return new BudgetSummaryDto
        {
            TotalBudget = budgetSummaryByEmail.TotalBudget,
            TotalExpenses = budgetSummaryByEmail.TotalExpenses,
            Categories = pagedBudgetSummaryCategories
        };
    }
}