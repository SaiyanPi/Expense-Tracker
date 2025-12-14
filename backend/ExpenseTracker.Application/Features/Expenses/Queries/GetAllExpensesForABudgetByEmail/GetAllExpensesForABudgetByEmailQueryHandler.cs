using AutoMapper;
using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.Common.Pagination;
using ExpenseTracker.Application.DTOs.Expense;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;

namespace ExpenseTracker.Application.Features.Expenses.Queries.GetAllExpensesForABudgetByEmail;

public record GetAllExpensesForABudgetByEmailQueryHandler : IRequestHandler<GetAllExpensesForABudgetByEmailQuery, PagedResult<ExpenseDto>>
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IBudgetRepository _budgetRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetAllExpensesForABudgetByEmailQueryHandler(
        IExpenseRepository expenseRepository,
        IBudgetRepository budgetRepository,
        IUserRepository userRepository,
        IMapper mapper)
    {
        _expenseRepository = expenseRepository;
        _budgetRepository = budgetRepository;
        _userRepository = userRepository;   
        _mapper = mapper;
    }

    public async Task<PagedResult<ExpenseDto>> Handle(
        GetAllExpensesForABudgetByEmailQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user == null)
            throw new NotFoundException(nameof(User), request.Email);
   
        var budget = await _budgetRepository.GetByIdAsync(request.BudgetId);
        if (budget == null)
            throw new NotFoundException(nameof(Budget), request.BudgetId);
        
        var query = request.Paging;

        var(expenses, totalCount) = await _expenseRepository.GetAllExpensesForABudgetByEmailAsync(
            budget.Id,
            user.Id,
            skip: query.Skip,
            take: query.EffectivePageSize,
            sortBy: query.SortBy,
            sortDesc: query.SortDesc,
            cancellationToken: cancellationToken);
        
        var mappedExpenses = _mapper.Map<IReadOnlyList<ExpenseDto>>(expenses);
        return new PagedResult<ExpenseDto>(mappedExpenses, totalCount, query.EffectivePage, query.EffectivePageSize);

    }
    
}