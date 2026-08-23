using System.Diagnostics;
using AutoMapper;
using ExpenseTracker.Application.Common.Caching;
using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.Common.Interfaces;
using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Application.Common.Observability.Metrics.Business.DomainSpecific;
using ExpenseTracker.Application.DTOs.Expense;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.Features.Expenses.Commands.CreateExpense;

public class CreateExpenseCommandHandler : IRequestHandler<CreateExpenseCommand, ExpenseDto>
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IBudgetRepository _budgetRepository;
    private readonly IUserAccessor _userAccessor;
    private readonly IMapper _mapper;
    private readonly INotificationService _notificationService;
    private readonly ILogger<CreateExpenseCommandHandler> _logger;
    private readonly ICacheVersionService _cacheVersionService;


    public CreateExpenseCommandHandler(IExpenseRepository expenseRepository,
        ICategoryRepository categoryRepository,
        IBudgetRepository budgetRepository,
        IUserAccessor userAccessor,
        IMapper mapper,
        INotificationService notificationService,
        ICacheVersionService cacheVersionService,
        ILogger<CreateExpenseCommandHandler> logger)
    {
        _expenseRepository = expenseRepository;
        _categoryRepository = categoryRepository;
        _budgetRepository = budgetRepository;
        _userAccessor = userAccessor;
        _mapper = mapper;
        _notificationService = notificationService; 
        _logger = logger;
        _cacheVersionService = cacheVersionService;
    }

    public async Task<ExpenseDto> Handle(CreateExpenseCommand request, CancellationToken cancellationToken)
    {
        var userId = _userAccessor.UserId;

        // defining date for the request with empty date value
        var expenseDate = request.CreateExpenseDto.Date ?? DateTime.UtcNow;

        _logger.LogInformation(
            "Creating expense for UserId {UserId} with Amount {Amount}, CategoryId {CategoryId}, and BudgetId {BudgetId}, Date {Date}",
            userId,
            request.CreateExpenseDto.Amount,
            request.CreateExpenseDto.CategoryId,
            request.CreateExpenseDto.BudgetId,
            request.CreateExpenseDto.Date
        );

        // BUISNESS RULE:
        // Admins cannot create expenses
        // user cannot create budget's expense with date outside the budget's date range

        // Duplicate titles allowed

        if (!string.IsNullOrWhiteSpace(request.CreateExpenseDto.UserId))
        {
            throw new BadRequestException("No permission. Try again without providing UserId field.");
        }

        // category validation
        if(request.CreateExpenseDto.CategoryId is Guid categoryId)  // equivalent to if(request.CreateExpenseDto.CategoryId.HasValue)
        {   
            // check if the category belongs to the user
            bool ownsCategory = await _categoryRepository.UserOwnsCategoryAsync(categoryId, userId, cancellationToken);
            if (!ownsCategory)
                throw new ConflictException($"You don't have a Category with id '{categoryId}'.");
        }
        
        // budget validation
        if (request.CreateExpenseDto.BudgetId is Guid budgetId)
        {
            // check if the budget belongs to the user
            bool ownsBudget = await _budgetRepository.UserOwnsBudgetAsync(budgetId, userId, cancellationToken);
            if (!ownsBudget)
                throw new ConflictException($"You don't have the Budget with id '{budgetId}'.");
            
            // load the budget once
            var budget = await _budgetRepository.GetByIdAsync(budgetId,cancellationToken);

            if (budget is null)
                throw new NotFoundException(nameof(Budget), budgetId);

            // check the budget isActive 
            var isActive = await _budgetRepository.GetBudgetStatusByIdAsync(budgetId, cancellationToken);
            if (!isActive)
                throw new NotFoundException("You cannot create an expense for an inactive/expired budget.");
            
            if (budget.CategoryId is Guid budgetCategoryId)
            {
                // Check whether the budget's category is still active(not soft-deleted).
                var ownsBudgetCategory = await _categoryRepository.UserOwnsCategoryAsync(budgetCategoryId, userId, cancellationToken);

                if (ownsBudgetCategory)
                {
                    // Budget has an ACTIVE category.
                    // Therefore the expense must use that category.

                    if (request.CreateExpenseDto.CategoryId is not Guid expenseCategoryId)
                    {
                        throw new BadRequestException(
                            $"A category is required when creating an expense under budget '{budget.Name}'.");
                    }

                    if (expenseCategoryId != budgetCategoryId)
                    {
                        throw new ConflictException(
                            $"The expense category must match the category assigned to budget '{budget.Name}'.");
                    }
                }

                // If ownsBudgetCategory == false, the budget's category
                // has been soft-deleted.
                //
                // In that case, treat the budget as having NO category.
                // The user may choose any active category or no category.
            }


            // check if the budget's expense date is out of budget's date range
            if (expenseDate.Date < budget!.StartDate.Date || expenseDate.Date > budget.EndDate.Date)
            {
                throw new BadRequestException($"Expense date must be between " +
                    $"{budget.StartDate:yyyy-MM-dd} and {budget.EndDate:yyyy-MM-dd}.");
            }

            var totalSpent = await _expenseRepository
                .GetTotalExpensesUnderABudgetAsync(budget!.Id, userId, cancellationToken);
            
            var remainingAmount = budget.Amount-totalSpent;
            // calculate spent ratio
            var thresholdPercentage = 50m;
            var percentageUsed = (totalSpent / budget.Amount) * 100m;
            var roundedPercentage = Math.Floor(percentageUsed);
            if(roundedPercentage > thresholdPercentage)
            {
                // hook the business metric
                ExpenseMetrics.BudgetThresholdExceeded();

                _logger.LogWarning(
                    "Budget threshold exceeded for BudgetId {BudgetId}. Used {PercentageUsed}%, Remaining {RemainingAmount}",
                    budget.Id,
                    roundedPercentage,
                    remainingAmount
                );

                // ⚠️⚠️
                // Also, one unrelated issue worth flagging: your budget-threshold calculation happens before the new expense is 
                // added, so percentageUsed represents spending before this expense. If the intention is "notify when this new 
                // expense causes the budget to cross 50%", your calculation needs to include dto.Amount. That's separate from 
                // today's category bug, but it's a genuine logic issue.
                await _notificationService.BudgetExceededAsync(
                    budget.Id,
                    budget.Name,
                    percentageUsed,
                    remainingAmount,
                    userId,
                    cancellationToken);
            }
        }
      
        var expense = _mapper.Map<Expense>(request.CreateExpenseDto);
        expense.UserId = userId;
        expense.Date = expenseDate;
        
        await _expenseRepository.AddAsync(expense, cancellationToken);

        // Invalidate the cache once a new expense is created for the user, so that the next
        // query will fetch fresh data
        _cacheVersionService.IncrementVersion(CacheGroups.Expenses, userId);

        // hook the business metric
        ExpenseMetrics.ExpenseCreated();

        _logger.LogInformation(
            "Expense created successfully. ExpenseId {ExpenseId}, UserId {UserId}, Amount {Amount}, Date{Date}",
            expense.Id,
            userId,
            expense.Amount,
            expense.Date
        );

        return _mapper.Map<ExpenseDto>(expense);
    }
}