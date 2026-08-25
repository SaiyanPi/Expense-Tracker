using AutoMapper;
using ExpenseTracker.Application.Common.Caching;
using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.Common.Interfaces;
using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Application.Common.Observability.Metrics.Business.DomainSpecific;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.Features.Expenses.Commands.UpdateExpense;

public class UpdateExpenseCommandHandler : IRequestHandler<UpdateExpenseCommand, Unit>
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IBudgetRepository _budgetRepository;
    private readonly IUserAccessor _userAccessor;
    private readonly IMapper _mapper;

    private readonly INotificationService _notificationService;
    private readonly ILogger<UpdateExpenseCommandHandler> _logger;
    private readonly ICacheVersionService _cacheVersionService;

    public UpdateExpenseCommandHandler(
        IExpenseRepository expenseRepository,
        ICategoryRepository categoryRepository,
        IBudgetRepository budgetRepository,
        IUserAccessor userAccessor,
        ICacheVersionService cacheVersionService,
        IMapper mapper,
        INotificationService notificationService,
        ILogger<UpdateExpenseCommandHandler> logger)
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

    public async Task<Unit> Handle(UpdateExpenseCommand request, CancellationToken cancellationToken)
    {
        var userId = _userAccessor.UserId;

        var expense = await _expenseRepository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (expense is null)
            throw new NotFoundException(nameof(Expense), request.Id);

        if (expense.UserId != userId)
            throw new ForbiddenException("You cannot update this expense.");

        // ---------------------------------------------------------
        // Category validation
        // ---------------------------------------------------------

        if (request.CategoryId is Guid categoryId)
        {
            var ownsCategory = await _categoryRepository.UserOwnsCategoryAsync(
                categoryId,
                userId,
                cancellationToken);

            if (!ownsCategory)
            {
                throw new ConflictException(
                    $"You don't have a Category with id '{categoryId}'.");
            }
        }

        // ---------------------------------------------------------
        // Budget validation
        // ---------------------------------------------------------

        if (request.BudgetId is Guid budgetId)
        {
            var ownsBudget = await _budgetRepository.UserOwnsBudgetAsync(
                budgetId,
                userId,
                cancellationToken);

            if (!ownsBudget)
            {
                throw new ConflictException(
                    $"You don't have the Budget with id '{budgetId}'.");
            }

            // Load the budget once.
            var budget = await _budgetRepository.GetByIdAsync(
                budgetId,
                cancellationToken);

            if (budget is null)
            {
                throw new NotFoundException(nameof(Budget), budgetId);
            }

            // -----------------------------------------------------
            // Budget must still be active
            // -----------------------------------------------------

            var isActive = await _budgetRepository.GetBudgetStatusByIdAsync(
                budgetId,
                cancellationToken);

            if (!isActive)
            {
                throw new NotFoundException(
                    "You cannot update an expense for an inactive/expired budget.");
            }

            // -----------------------------------------------------
            // Budget category validation
            // -----------------------------------------------------

            if (budget.CategoryId is Guid budgetCategoryId)
            {
                var ownsBudgetCategory =
                    await _categoryRepository.UserOwnsCategoryAsync(
                        budgetCategoryId,
                        userId,
                        cancellationToken);

                if (ownsBudgetCategory)
                {
                    // Budget still has an active category.
                    // Expense category must match it.

                    if (request.CategoryId is not Guid expenseCategoryId)
                    {
                        throw new BadRequestException(
                            $"A category is required when updating an expense under budget '{budget.Name}'.");
                    }

                    if (expenseCategoryId != budgetCategoryId)
                    {
                        throw new ConflictException(
                            $"The expense category must match the category assigned to budget '{budget.Name}'.");
                    }
                }

                // If ownsBudgetCategory == false,
                // the budget's category was soft-deleted.
                //
                // In that case the budget is treated as having
                // no category, so the expense may use any active
                // category or no category.
            }

            // -----------------------------------------------------
            // Expense date validation
            // -----------------------------------------------------

            var expenseDate = request.Date;

            if (expenseDate.Date < budget.StartDate.Date || expenseDate.Date > budget.EndDate.Date)
            {
                throw new BadRequestException($"Expense date must be between " +
                    $"{budget.StartDate:yyyy-MM-dd} and {budget.EndDate:yyyy-MM-dd}.");
            }

            // -----------------------------------------------------
            // Budget threshold calculation
            // -----------------------------------------------------

            var totalSpent = await _expenseRepository
                .GetTotalExpensesUnderABudgetAsync(
                    budget.Id,
                    userId,
                    cancellationToken);

            // totalSpent currently includes the expense being updated.
            //
            // Remove the old amount and add the new amount.
            var newTotalSpent =
                totalSpent - expense.Amount + request.Amount;

            var remainingAmount =
                budget.Amount - newTotalSpent;

            var thresholdPercentage = 50m;

            var percentageUsed =
                (newTotalSpent / budget.Amount) * 100m;

            var roundedPercentage =
                Math.Floor(percentageUsed);

            if (roundedPercentage > thresholdPercentage)
            {
                ExpenseMetrics.BudgetThresholdExceeded();

                _logger.LogWarning(
                    "Budget threshold exceeded after updating ExpenseId {ExpenseId} " +
                    "for BudgetId {BudgetId}. Used {PercentageUsed}%, Remaining {RemainingAmount}",
                    expense.Id,
                    budget.Id,
                    roundedPercentage,
                    remainingAmount);

                await _notificationService.BudgetExceededAsync(
                    budget.Id,
                    budget.Name,
                    percentageUsed,
                    remainingAmount,
                    userId,
                    cancellationToken);
            }
        }

        // ---------------------------------------------------------
        // Map and update expense
        // ---------------------------------------------------------

        _mapper.Map(request, expense);

        await _expenseRepository.UpdateAsync(
            expense,
            cancellationToken);

        // Expense changed.
        _cacheVersionService.IncrementVersion(
            CacheGroups.Expenses,
            userId);

        // Budget totals/summary may have changed.
        _cacheVersionService.IncrementVersion(
            CacheGroups.Budgets,
            userId);

        return Unit.Value;
    }
}
