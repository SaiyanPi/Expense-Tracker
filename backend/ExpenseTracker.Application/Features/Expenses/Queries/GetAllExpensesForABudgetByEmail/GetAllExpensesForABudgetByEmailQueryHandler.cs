using AutoMapper;
using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Application.Common.Pagination;
using ExpenseTracker.Application.DTOs.Expense;
using ExpenseTracker.Application.DTOS.Expense;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;

namespace ExpenseTracker.Application.Features.Expenses.Queries.GetAllExpensesForABudgetByEmail;

public record GetAllExpensesForABudgetByEmailQueryHandler : IRequestHandler<GetAllExpensesForABudgetByEmailQuery, PagedResult<ExpenseSummaryForBudgetDto>>
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IBudgetRepository _budgetRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserAccessor _userAccessor;
    private readonly IMapper _mapper;

    public GetAllExpensesForABudgetByEmailQueryHandler(
        IExpenseRepository expenseRepository,
        IBudgetRepository budgetRepository,
        IUserRepository userRepository,
        IUserAccessor userAccessor,
        IMapper mapper)
    {
        _expenseRepository = expenseRepository;
        _budgetRepository = budgetRepository;
        _userRepository = userRepository;   
        _userAccessor = userAccessor;
        _mapper = mapper;
    }

    public async Task<PagedResult<ExpenseSummaryForBudgetDto>> Handle(
        GetAllExpensesForABudgetByEmailQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _userAccessor.UserId;
   
        var budget = await _budgetRepository.GetByIdAsync(request.BudgetId);
        if (budget == null)
            throw new NotFoundException(nameof(Budget), request.BudgetId);
        
        var query = request.Paging;

        var(expenses, totalCount) = await _expenseRepository.GetAllExpensesForABudgetByEmailAsync(
            budget.Id,
            userId,
            skip: query.Skip,
            take: query.EffectivePageSize,
            sortBy: query.SortBy,
            sortDesc: query.SortDesc,
            cancellationToken: cancellationToken);
        
        var mappedExpenses = _mapper.Map<IReadOnlyList<ExpenseSummaryForBudgetDto>>(expenses);
        return new PagedResult<ExpenseSummaryForBudgetDto>(mappedExpenses, totalCount, query.EffectivePage, query.EffectivePageSize);

    }
    
}