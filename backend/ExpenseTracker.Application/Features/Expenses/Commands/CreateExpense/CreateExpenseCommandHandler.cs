using AutoMapper;
using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Application.DTOs.Expense;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;

namespace ExpenseTracker.Application.Features.Expenses.Commands.CreateExpense;

public class CreateExpenseCommandHandler : IRequestHandler<CreateExpenseCommand, ExpenseDto>
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IBudgetRepository _budgetRepository;
    private readonly IUserAccessor _userAccessor;
    private readonly IMapper _mapper;

    public CreateExpenseCommandHandler(IExpenseRepository expenseRepository,
        ICategoryRepository categoryRepository,
        IBudgetRepository budgetRepository,
        IUserAccessor userAccessor,
        IMapper mapper)
    {
        _expenseRepository = expenseRepository;
        _categoryRepository = categoryRepository;
        _budgetRepository = budgetRepository;
        _userAccessor = userAccessor;
        _mapper = mapper;
    }

    public async Task<ExpenseDto> Handle(CreateExpenseCommand request, CancellationToken cancellationToken)
    {
        // BUISNESS RULE:
        // Admins cannot create expenses
        // Duplicate titles allowed
        
        var userId = _userAccessor.UserId;

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
            var budget = await _budgetRepository.GetBudgetStatusByIdAsync(budgetId, cancellationToken);
            if (!budget)
                throw new NotFoundException("You cannot create an expense for an inactive/expired budget.");
        }
      
        var expense = _mapper.Map<Expense>(request.CreateExpenseDto);
        expense.UserId = userId;
        await _expenseRepository.AddAsync(expense, cancellationToken);
        return _mapper.Map<ExpenseDto>(expense);
    }
}