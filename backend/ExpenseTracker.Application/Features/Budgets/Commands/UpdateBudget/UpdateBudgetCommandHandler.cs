using AutoMapper;
using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;

namespace ExpenseTracker.Application.Features.Budgets.Commands.UpdateBudget;

public class UpdateBudgetCommandHandler : IRequestHandler<UpdateBudgetCommand, Unit>
{
    private readonly IBudgetRepository _budgetRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUserAccessor _userAccessor;
    private readonly IMapper _mapper;

    public UpdateBudgetCommandHandler(
        IBudgetRepository budgetRepository,
        ICategoryRepository categoryRepository,
        IUserAccessor userAccessor,
        IMapper mapper)
    {
        _budgetRepository = budgetRepository;
        _categoryRepository = categoryRepository;
        _userAccessor = userAccessor;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(UpdateBudgetCommand request, CancellationToken cancellationToken)
    {
        // BUISNESS RULE:
        // duplicate budget title not allowed

        var userId = _userAccessor.UserId;

        var budget = await _budgetRepository.GetByIdAsync(request.Id, cancellationToken);
        if (budget == null)
            throw new NotFoundException(nameof(Domain.Entities.Budget), request.Id);

        if(budget.UserId != userId)
            throw new ForbiddenException("You cannot update this budget.");

        // if categoryId is provided in the request body
        if(request.CategoryId is Guid categoryId)
        {
            // check if the user owns a category 
            var ownsCategory = await _categoryRepository.UserOwnsCategoryAsync(categoryId, userId, cancellationToken);
            if (!ownsCategory)
                throw new ConflictException($"You don't have the Category with id '{categoryId}'.");
            
            // prevent duplicate budgets within the user with same category
            var titleExists = await _budgetRepository.ExistByNameUserIdAndCategoryIdAsync(request.Name,
                userId,
                excludeBudgetId: null,
                categoryId,
                cancellationToken);
            if (titleExists)
                throw new ConflictException($"Budget with name '{request.Name}' and category '{categoryId}' already exists ");
        }

        _mapper.Map(request, budget);

        await _budgetRepository.UpdateAsync(budget, cancellationToken);
        return Unit.Value;
    }
}