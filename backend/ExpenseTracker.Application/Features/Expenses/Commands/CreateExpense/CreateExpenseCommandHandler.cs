using AutoMapper;
using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.Common.Interfaces;
using ExpenseTracker.Application.Common.Interfaces.Services;
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


    public CreateExpenseCommandHandler(IExpenseRepository expenseRepository,
        ICategoryRepository categoryRepository,
        IBudgetRepository budgetRepository,
        IUserAccessor userAccessor,
        IMapper mapper,
        INotificationService notificationService,
        ILogger<CreateExpenseCommandHandler> logger)
    {
        _expenseRepository = expenseRepository;
        _categoryRepository = categoryRepository;
        _budgetRepository = budgetRepository;
        _userAccessor = userAccessor;
        _mapper = mapper;
        _notificationService = notificationService; 
        _logger = logger;
    }

    public async Task<ExpenseDto> Handle(CreateExpenseCommand request, CancellationToken cancellationToken)
    {
        var userId = _userAccessor.UserId;

        _logger.LogInformation(
            "Creating expense for UserId {UserId} with Amount {Amount} and CategoryId {CategoryId}",
            userId,
            request.CreateExpenseDto.Amount,
            request.CreateExpenseDto.CategoryId
        );

        // BUISNESS RULE:
        // Admins cannot create expenses
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

            // check the budget isActive 
            var isActive = await _budgetRepository.GetBudgetStatusByIdAsync(budgetId, cancellationToken);
            if (!isActive)
                throw new NotFoundException("You cannot create an expense for an inactive/expired budget.");
            
            var budget = await _budgetRepository.GetByIdAsync(budgetId, cancellationToken);

            var totalSpent = await _expenseRepository
                .GetTotalExpensesUnderABudgetAsync(budget!.Id, userId, cancellationToken);
            
            var remainingAmount = budget.Amount-totalSpent;
            // calculate spent ratio
            var thresholdPercentage = 50m;
            var percentageUsed = (totalSpent / budget.Amount) * 100m;
            var roundedPercentage = Math.Floor(percentageUsed);
            if(roundedPercentage > thresholdPercentage)
            {
                _logger.LogWarning(
                    "Budget threshold exceeded for BudgetId {BudgetId}. Used {PercentageUsed}%, Remaining {RemainingAmount}",
                    budget.Id,
                    roundedPercentage,
                    remainingAmount
                );

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
        await _expenseRepository.AddAsync(expense, cancellationToken);

        _logger.LogInformation(
            "Expense created successfully. ExpenseId {ExpenseId}, UserId {UserId}, Amount {Amount}",
            expense.Id,
            userId,
            expense.Amount
        );

        return _mapper.Map<ExpenseDto>(expense);
    }
}