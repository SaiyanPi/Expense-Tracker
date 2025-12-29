using AutoMapper;
using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;

namespace ExpenseTracker.Application.Features.Expenses.Commands.UpdateExpense;

public class UpdateExpenseCommandHandler : IRequestHandler<UpdateExpenseCommand, Unit>
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IBudgetRepository _budgetRepository;
    private readonly IUserAccessor _userAccessor;
    private readonly IMapper _mapper;

    public UpdateExpenseCommandHandler(
        IExpenseRepository expenseRepository,
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

    public async Task<Unit> Handle(UpdateExpenseCommand request, CancellationToken cancellationToken)
    {
        var userId = _userAccessor.UserId;
        
        var expense = await _expenseRepository.GetByIdAsync(request.Id, cancellationToken);
        if (expense == null)
            throw new NotFoundException(nameof(Expense), request.Id);

        if(expense.UserId != userId)
            throw new ForbiddenException("You cannot update this expense.");
            
        // if categoryId is provided in the request body
        if(request.CategoryId is Guid categoryId)
        {
            // check if the user owns a category 
            var ownsCategory = await _categoryRepository.UserOwnsCategoryAsync(categoryId, userId, cancellationToken);
            if (!ownsCategory)
                throw new ConflictException($"You don't have the Category with id '{categoryId}'.");
        }

        // if the BudgetId is provided in the request body
        if(request.BudgetId is Guid budgetId)
        {
            // check if the user owns a budget 
            var ownsBudget = await _budgetRepository.UserOwnsBudgetAsync(budgetId, userId, cancellationToken);
            if (!ownsBudget)
                throw new ConflictException($"You don't have the Budget with id '{budgetId}'.");
        }
        
        _mapper.Map(request, expense);

        await _expenseRepository.UpdateAsync(expense, cancellationToken);
        return Unit.Value;
    }
}
