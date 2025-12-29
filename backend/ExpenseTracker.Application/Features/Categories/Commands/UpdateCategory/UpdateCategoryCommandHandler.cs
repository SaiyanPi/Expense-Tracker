using AutoMapper;
using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;

namespace ExpenseTracker.Application.Features.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, Unit>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserAccessor _userAccessor;
    private readonly IUserRoleService _userRoleService;
    private readonly IMapper _mapper;

    public UpdateCategoryCommandHandler(
        ICategoryRepository categoryRepository,
        IUserRepository userRepository,
        IUserAccessor userAccessor,
        IUserRoleService userRoleService,
        IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _userRepository = userRepository;
        _userAccessor = userAccessor;
        _userRoleService = userRoleService;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (category == null)
            throw new NotFoundException(nameof(Category), request.Id);

        var userId = _userAccessor.UserId;
        var isAdmin = await _userRoleService.IsAdminAsync(userId);

        // BUISNESS RULES: 
        // admins can only update category with null userId
        // only specific user can update category with the specific userId
        if (category.UserId is null)
        {
            // System category → admin only
            if (!isAdmin)
                throw new ForbiddenException("Only admins can update system categories.");
        }
        else
        {
            // User category → owner only (admins are NOT allowed)
            if (category.UserId != userId)
                throw new ForbiddenException("You cannot update this category.");
        }

        // Check for uniqueness of category name
        if (!string.Equals(category.Name, request.Name, StringComparison.OrdinalIgnoreCase))
        {
            var userIdToCheck = category.UserId ?? string.Empty; // null or specific user

            var nameExists = await _categoryRepository.ExistsByNameAndUserIdAsync(
                request.Name,
                userIdToCheck,
                category.Id,
                cancellationToken);

            if (nameExists)
                throw new ValidationException($"Category with name '{request.Name}' already exists.");
        }

        _mapper.Map(request, category);
        await _categoryRepository.UpdateAsync(category, cancellationToken);
        return Unit.Value;
    }
}