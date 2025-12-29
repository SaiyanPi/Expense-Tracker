using AutoMapper;
using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Application.Common.Pagination;
using ExpenseTracker.Application.DTOs.Budget;
using ExpenseTracker.Application.DTOs.Expense;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;

namespace ExpenseTracker.Application.Features.Budgets.Queries.GetBudgetDetailWithExpensesByEmail;

public class GetBudgetDetailWithExpensesByEmailQueryHandler : IRequestHandler<GetBudgetDetailWithExpensesByEmailQuery, BudgetDetailWithExpensesDto>
{
    private readonly IBudgetRepository _budgetRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserAccessor _userAccessor;
    private readonly IMapper _mapper;

    public GetBudgetDetailWithExpensesByEmailQueryHandler(
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

    public async Task<BudgetDetailWithExpensesDto> Handle(GetBudgetDetailWithExpensesByEmailQuery request, CancellationToken cancellationToken)
    {
        var userId = _userAccessor.UserId;
        var userEmail = _userAccessor.UserEmail;

        var budget = await _budgetRepository.GetByIdAsync(request.BudgetId, cancellationToken);
        if (budget == null)
            throw new NotFoundException(nameof(Budget), request.BudgetId);
        
        if(budget.UserId != userId)
            throw new ForbiddenException($"You do not own the budget '{budget.Id}'.");
        
        var query = request.Paging;

        var budgetDetailWithExpensesByEmailSummary = await _budgetRepository.GetBudgetDetailWithExpensesByEmailAsync(
            budget.Id,
            userId,
            
            skip: query.Skip,
            take: query.EffectivePageSize,
            sortBy: query.SortBy,
            sortDesc: query.SortDesc,
            cancellationToken: cancellationToken);
        
        var mappedExpenses = _mapper.Map<IReadOnlyList<ExpenseDto>>(budgetDetailWithExpensesByEmailSummary.Expenses);
        
        var pagedExpenses = new PagedResult<ExpenseDto>(
            mappedExpenses,
            budgetDetailWithExpensesByEmailSummary.TotalCount,
            query.EffectivePage,
            query.EffectivePageSize);
        
        return new BudgetDetailWithExpensesDto
        {
            Id = budgetDetailWithExpensesByEmailSummary.Id,
            Name = budgetDetailWithExpensesByEmailSummary.Name,
            Limit = budgetDetailWithExpensesByEmailSummary.Limit,
            TotalSpent = budgetDetailWithExpensesByEmailSummary.TotalSpent,
            IsActive = budgetDetailWithExpensesByEmailSummary.IsActive,
            Expenses = pagedExpenses
        };
    }
}