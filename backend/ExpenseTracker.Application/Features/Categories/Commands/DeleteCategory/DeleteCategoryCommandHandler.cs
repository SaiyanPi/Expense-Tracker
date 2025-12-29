using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;

namespace ExpenseTracker.Application.Features.Categories.Commands.DeleteCategory;

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Unit>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUserAccessor _userAccessor;
    private readonly IUserRoleService _userRoleService;

    public DeleteCategoryCommandHandler(
        ICategoryRepository categoryRepository,
        IUserAccessor userAccessor,
        IUserRoleService userRoleService)
    {
        _categoryRepository = categoryRepository;
        _userAccessor = userAccessor;
        _userRoleService = userRoleService;
    }

    public async Task<Unit> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (category is null)
            throw new NotFoundException(nameof(Category), request.Id);
        
        var userId = _userAccessor.UserId;
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

        await _categoryRepository.DeleteAsync(category, cancellationToken);
        return Unit.Value;
    }
}

