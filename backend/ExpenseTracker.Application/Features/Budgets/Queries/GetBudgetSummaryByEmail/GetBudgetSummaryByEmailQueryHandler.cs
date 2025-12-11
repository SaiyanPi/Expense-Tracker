using AutoMapper;
using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.DTOs.Budget;
using ExpenseTracker.Application.Features.Budgets.Queries.GetBudgetSummaryByEmail;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;

namespace ExpenseTracker.Application.Features.Budgets.Queries.GetBudgetSummary;

public class GetBudgetSummaryByEmailQueryHandler : IRequestHandler<GetBudgetSummaryByEmailQuery, List<BudgetSummaryDto>>
{
    private readonly IBudgetRepository _budgetRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;


    public GetBudgetSummaryByEmailQueryHandler(IBudgetRepository budgetRepository,
        IUserRepository userRepository,
        IMapper mapper)
    {
        _budgetRepository = budgetRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<List<BudgetSummaryDto>> Handle(GetBudgetSummaryByEmailQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user == null)
            throw new NotFoundException(nameof(User), request.Email);

        var budgetSummaryByEmail = await _budgetRepository.GetBudgetSummaryByEmailAsync(user.Id, cancellationToken);

        return _mapper.Map<List<BudgetSummaryDto>>(budgetSummaryByEmail) ;
    }
}