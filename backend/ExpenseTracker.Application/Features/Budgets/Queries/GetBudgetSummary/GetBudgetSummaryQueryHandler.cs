using AutoMapper;
using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.DTOs.Budget;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;

namespace ExpenseTracker.Application.Features.Budgets.Queries.GetBudgetSummary;

public class GetBudgetSummaryQueryHandler : IRequestHandler<GetBudgetSummaryQuery, List<BudgetSummaryDto>>
{
    private readonly IBudgetRepository _budgetRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;


    public GetBudgetSummaryQueryHandler(IBudgetRepository budgetRepository,
        IUserRepository userRepository,
        IMapper mapper)
    {
        _budgetRepository = budgetRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<List<BudgetSummaryDto>> Handle(GetBudgetSummaryQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user == null)
            throw new NotFoundException(nameof(BudgetSummaryDto), request.Email);

        var budgetSummary = await _budgetRepository.GetBudgetSummaryAsync(user.Id, cancellationToken);

        return _mapper.Map<List<BudgetSummaryDto>>(budgetSummary) ;
    }
}