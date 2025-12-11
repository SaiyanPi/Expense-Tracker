using AutoMapper;
using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.DTOs.Budget;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;

namespace ExpenseTracker.Application.Features.Budgets.Queries.GetBudgetDetailWithExpensesByEmail;

public class GetBudgetDetailWithExpensesByEmailQueryHandler : IRequestHandler<GetBudgetDetailWithExpensesByEmailQuery, BudgetDetailWithExpensesDto>
{
    private readonly IBudgetRepository _budgetRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetBudgetDetailWithExpensesByEmailQueryHandler(
        IBudgetRepository budgetRepository,
        IUserRepository userRepository,
        IMapper mapper
    )
    {
        _budgetRepository = budgetRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<BudgetDetailWithExpensesDto> Handle(GetBudgetDetailWithExpensesByEmailQuery request, CancellationToken cancellationToken)
    {
        var budget = await _budgetRepository.GetByIdAsync(request.BudgetId, cancellationToken);
        if (budget == null)
            throw new NotFoundException(nameof(Budget), request.BudgetId);
        
        var user = await _userRepository.GetByEmailAsync(request.email, cancellationToken);
        if(user == null)
            throw new NotFoundException(nameof(User), request.email);
        
        var budgetDetailWithExpensesByEmailSummary = await _budgetRepository.GetBudgetDetailWithExpensesByEmailAsync(budget.Id, user.Id, cancellationToken);

        return _mapper.Map<BudgetDetailWithExpensesDto>(budgetDetailWithExpensesByEmailSummary);
    }
}