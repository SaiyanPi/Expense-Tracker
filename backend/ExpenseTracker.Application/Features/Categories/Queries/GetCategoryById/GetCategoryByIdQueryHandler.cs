using AutoMapper;
using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Application.DTOs.Category;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;

namespace ExpenseTracker.Application.Features.Categories.Queries.GetCategoryById;

public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, CategoryDto>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUserAccessor _userAccessor;
    private readonly IUserRoleService _userRoleService;
    private readonly IMapper _mapper;

    public GetCategoryByIdQueryHandler(
        ICategoryRepository categoryRepository,
        IUserAccessor userAccessor,
        IUserRoleService userRoleService,
        IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _userAccessor = userAccessor;
        _userRoleService = userRoleService;
        _mapper = mapper;
    }

    public async Task<CategoryDto> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (category is null)
            throw new NotFoundException(nameof(Category), request.Id);
        
        var userId = _userAccessor.UserId;
        var isAdmin = await _userRoleService.IsAdminAsync(userId);

        // BUISNESS RULES: 
        // admins can fetch any user's category including null userId
        // specific user can fetch category with the specific userId
        
        // System category → admin only
        if (category.UserId is null && !isAdmin)
            throw new ForbiddenException("Only admins can view system categories.");

        // User category → owner OR admin
        if (category.UserId is not null && category.UserId != userId && !isAdmin)
            throw new ForbiddenException($"You don't have access to category '{request.Id}'.");

        return _mapper.Map<CategoryDto>(category);
    }
}