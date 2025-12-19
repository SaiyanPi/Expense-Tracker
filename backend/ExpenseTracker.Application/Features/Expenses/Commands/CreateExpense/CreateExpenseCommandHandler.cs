using AutoMapper;
using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.DTOs.Expense;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;

namespace ExpenseTracker.Application.Features.Expenses.Commands.CreateExpense;

public class CreateExpenseCommandHandler : IRequestHandler<CreateExpenseCommand, ExpenseDto>
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUserRepository _userRepository;
    private readonly IBudgetRepository _budgetRepository;
    private readonly IMapper _mapper;

    public CreateExpenseCommandHandler(IExpenseRepository expenseRepository,
        ICategoryRepository categoryRepository,
        IUserRepository userRepository,
        IBudgetRepository budgetRepository,
        IMapper mapper)
    {
        _expenseRepository = expenseRepository;
        _categoryRepository = categoryRepository;
        _userRepository = userRepository;
        _budgetRepository = budgetRepository;
        _mapper = mapper;
    }

    public async Task<ExpenseDto> Handle(CreateExpenseCommand request, CancellationToken cancellationToken)
    {
        // check if the user exist
        var user = await _userRepository.GetByIdAsync(request.CreateExpenseDto.UserId, cancellationToken);
        if (user is null)
            throw new NotFoundException(nameof(User), request.CreateExpenseDto.UserId);
        
        // check if the category exist
        var category = await _categoryRepository.GetByIdAsync(request.CreateExpenseDto.CategoryId, cancellationToken);
            if (category is null)
                throw new NotFoundException(nameof(Category), request.CreateExpenseDto.CategoryId);

        // check if the category belongs to the user
        bool ownsCategory = await _categoryRepository.UserOwnsCategoryAsync(request.CreateExpenseDto.CategoryId, request.CreateExpenseDto.UserId, cancellationToken);
        if (!ownsCategory)
            throw new ConflictException($"Category with id '{request.CreateExpenseDto.CategoryId}' does not belong to user '{request.CreateExpenseDto.UserId}'.");

        if (request.CreateExpenseDto.BudgetId.HasValue)
        {
            // check if the budget exist
            var budgetExists = await _budgetRepository.GetByIdAsync(request.CreateExpenseDto.BudgetId.Value, cancellationToken);
            if (budgetExists is null)
                throw new NotFoundException(nameof(Budget), request.CreateExpenseDto.BudgetId.Value);

            // check if the budget belongs to the user
            bool ownsBudget = await _budgetRepository.UserOwnsBudgetAsync(request.CreateExpenseDto.BudgetId.Value, request.CreateExpenseDto.UserId, cancellationToken);
            if (!ownsBudget)
                throw new ConflictException($"Budget with id '{request.CreateExpenseDto.BudgetId}' does not belong to user '{request.CreateExpenseDto.UserId}'.");

            // check the budget isActive 
            var budget = await _budgetRepository.GetBudgetStatusByIdAsync(request.CreateExpenseDto.BudgetId.Value, cancellationToken);
            if (!budget)
                throw new NotFoundException("You cannot create an expense for an inactive/expired budget.");
            
        }
      
        var expense = _mapper.Map<Expense>(request.CreateExpenseDto);
        await _expenseRepository.AddAsync(expense, cancellationToken);
        return _mapper.Map<ExpenseDto>(expense);
    }
}