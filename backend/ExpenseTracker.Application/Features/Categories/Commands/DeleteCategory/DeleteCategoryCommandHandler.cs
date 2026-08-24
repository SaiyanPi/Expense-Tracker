using ExpenseTracker.Application.Common.Caching;
using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Application.Common.Observability.Metrics.Business.DomainSpecific;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.Features.Categories.Commands.DeleteCategory;

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Unit>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IExpenseRepository _expenseRepository;
    private readonly IBudgetRepository _budgetRepository;
    private readonly IUserAccessor _userAccessor;
    private readonly IUserRoleService _userRoleService;
    private readonly ILogger<DeleteCategoryCommandHandler> _logger;
    private readonly ICacheVersionService _cacheVersionService;

    public DeleteCategoryCommandHandler(
        ICategoryRepository categoryRepository,
        IExpenseRepository expenseRepository,
        IBudgetRepository budgetRepository,
        IUserAccessor userAccessor,
        IUserRoleService userRoleService,
        ICacheVersionService cacheVersionService,
        ILogger<DeleteCategoryCommandHandler> logger)
    {
        _categoryRepository = categoryRepository;
        _expenseRepository = expenseRepository;
        _budgetRepository = budgetRepository;
        _userAccessor = userAccessor;
        _userRoleService = userRoleService;
        _cacheVersionService = cacheVersionService;
        _logger = logger;
    }

    public async Task<Unit> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var userId = _userAccessor.UserId;

        _logger.LogInformation(
            "Deleting category with id {Id} requested by UserId {UserId}",
            request.Id,
            userId
        );

        var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (category is null)
            throw new NotFoundException(nameof(Category), request.Id);
        
        var isAdmin = await _userRoleService.IsAdminAsync(userId);

        // BUISNESS RULES: 
        // admins can only delete category with null userId
        // only specific user can delete category with the specific userId
        if (category.UserId is null)
        {
            // System category → admin only
            if (!isAdmin)
                throw new ForbiddenException("Only admins can delete system categories.");
        }
        else
        {
            // User category → owner only (admins are NOT allowed)
            if (category.UserId != userId)
                throw new ForbiddenException($"You don't have access to delete category with id '{request.Id}'.");
        }

         // Clear all expense references to this category.
        //
        // We do this before soft-deleting the category so that after deletion:
        //
        // Expense.CategoryId   = null
        // Expense.CategoryName = null
        //
        // For a system category, this must affect expenses belonging
        // to all users that reference the category.
        await _expenseRepository.ClearCategoryReferencesAsync(
            category.Id,
            cancellationToken);

        // Clear all budget references to this category as well.
        await _budgetRepository.ClearCategoryReferencesAsync(
            category.Id,
            cancellationToken);



        await _categoryRepository.DeleteAsync(category, cancellationToken);

        // Invalidate the cache once a new category is deleted for the user, so that the deleted
        // query will fetch fresh data
        _cacheVersionService.IncrementVersion(CacheGroups.Categories, userId);

        // hook the business metric
        CategoryMetrics.CategoryDeleted();
        
        _logger.LogInformation(
            "Category deleted successfully with id {CategoryId} by UserId {UserId}",
            request.Id,
            userId
        );
        return Unit.Value;
    }
}

