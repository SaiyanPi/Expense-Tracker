using AutoMapper;
using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Application.Common.Observability.Metrics.BusinessMetrics;
using ExpenseTracker.Application.DTOs.Budget;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.Features.Budgets.Commands.CreateBudget;

public class CreateBudgetCommandHandler : IRequestHandler<CreateBudgetCommand, BudgetDto>
{
    private readonly IBudgetRepository _budgetRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUserAccessor _userAccessor;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateBudgetCommandHandler> _logger;

    public CreateBudgetCommandHandler(
        IBudgetRepository budgetRepository,
        ICategoryRepository categoryRepository,
        IUserAccessor userAccessor,
        IMapper mapper,
        ILogger<CreateBudgetCommandHandler> logger
    )
    {
        _budgetRepository = budgetRepository;
        _categoryRepository = categoryRepository;
        _userAccessor = userAccessor;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<BudgetDto> Handle(CreateBudgetCommand request, CancellationToken cancellationToken)
    {
        var userId = _userAccessor.UserId;

        _logger.LogInformation(
            "Creating budget with Amount {Amount}, CategoryId {CategoryId}, and userId {UserId}",
            request.CreateBudgetDto.Amount,
            request.CreateBudgetDto.CategoryId,
            userId
        );

        // BUISNESS RULE:
        // Only regular users can create budget
        // duplicate budget title not allowed

        if (!string.IsNullOrWhiteSpace(request.CreateBudgetDto.UserId))
        {
            throw new BadRequestException("No permission. Try again without providing UserId field.");
        }

        // category validation
        if(request.CreateBudgetDto.CategoryId is Guid categoryId) 
        {   
            // check if the category belongs to the user
            bool ownsCategory = await _categoryRepository.UserOwnsCategoryAsync(categoryId, userId, cancellationToken);
            if (!ownsCategory)
                throw new ConflictException($"You don't have a Category with id '{categoryId}'.");
            
            // prevent duplicate budgets within the user with same category
            var titleExists = await _budgetRepository.ExistByNameUserIdAndCategoryIdAsync(request.CreateBudgetDto.Name,
                userId,
                excludeBudgetId: null,
                categoryId,
                cancellationToken);
            if (titleExists)
                throw new ConflictException($"Budget with name '{request.CreateBudgetDto.Name}' and category '{categoryId}' already exists ");
        }

        //var budget = _mapper.Map<Budget>(request.CreateBudgetDto);
        var budget = _mapper.Map<Budget>(request.CreateBudgetDto);
        budget.UserId = userId;
        await _budgetRepository.AddAsync(budget, cancellationToken);

        _logger.LogInformation(
            "Budget created successfully with id {BudgetId} and Amount {Amount}, CategoryId {CategoryId}, and userId {UserId}",
            budget.Id,
            request.CreateBudgetDto.Amount,
            request.CreateBudgetDto.CategoryId,
            userId
        );

        // hook the business metric
        BudgetMetrics.BudgetCreated();

        return _mapper.Map<BudgetDto>(budget);
    }
}