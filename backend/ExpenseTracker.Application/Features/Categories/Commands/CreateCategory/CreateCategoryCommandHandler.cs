using AutoMapper;
using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Application.Common.Observability.Metrics.BusinessMetrics;
using ExpenseTracker.Application.DTOs.Category;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.Features.Categories.Commands.CreateCategory;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryDto>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserRoleService _userRoleService;
    private readonly IUserAccessor _userAccessor;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateCategoryCommandHandler> _logger;
    public CreateCategoryCommandHandler(ICategoryRepository categoryRepository,
        IUserRepository userRepository,
        IUserRoleService userRoleService,
        IUserAccessor userAccessor,
        IMapper mapper,
        ILogger<CreateCategoryCommandHandler> logger)
    {
        _categoryRepository = categoryRepository;
        _userRepository = userRepository;
        _userRoleService = userRoleService;
        _userAccessor = userAccessor;
        _mapper = mapper;
        _logger = logger;
    }
    public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var userId = _userAccessor.UserId;
        string userIdToUse;

        _logger.LogInformation(
            "Creating category with Name {Name} for user {regularUserId}, initialized by UserId {userId}",
            request.CreateCategoryDto.UserId,
            request.CreateCategoryDto.Name,
            userId
        );

        var isAdmin = await _userRoleService.IsAdminAsync(userId);

        // BUISNESS RULE:
        // Admin can create category with existing UserId, UserId must be provided for admin
        // Regular users cannot have UserId provided in the request body, they can only create for themselves

        if (isAdmin)
        {
            // if userId is not provided in the request body
            if(string.IsNullOrWhiteSpace(request.CreateCategoryDto.UserId))
            {
                throw new BadRequestException("UserId is required for admin users.");
            }
            
            // check if the userId exists
            var userExists = await _userRepository.GetByIdAsync(request.CreateCategoryDto.UserId, cancellationToken);
            if (userExists is null)
                throw new NotFoundException(nameof(User), request.CreateCategoryDto.UserId);  
            
            userIdToUse = request.CreateCategoryDto.UserId;
        }
        else
        {
            // regular user cannot provide UserId in the request body
            if (!string.IsNullOrWhiteSpace(request.CreateCategoryDto.UserId))
            {
                throw new BadRequestException("No permission. Try again without providing UserId field.");
            }

            userIdToUse = userId;
        }

        // category name must be unique per user
        var titleExists = await _categoryRepository.ExistsByNameAndUserIdAsync(request.CreateCategoryDto.Name,
            userIdToUse,
            excludeCategoryId: null,
            cancellationToken);
        if (titleExists)
        {
            var message = isAdmin
            ? $"Category with name '{request.CreateCategoryDto.Name}' already exists for user '{userIdToUse}'."
            : $"Category with name '{request.CreateCategoryDto.Name}' already exists.";

            throw new ConflictException(message);

        }

        var category = _mapper.Map<Category>(request.CreateCategoryDto);
        category.UserId = userIdToUse; // enforce the correct userId
        await _categoryRepository.AddAsync(category, cancellationToken);

        // hook the business metric
        CategoryMetrics.CategoryCreated();
        
        _logger.LogInformation(
            "Category created successfully with id {CategoryId}, name {Name}, and UserId {UserIdToUse} initialized by UserId {UserId}",
            category.Id,
            request.CreateCategoryDto.Name,
            userIdToUse,
            userId
        );

        return _mapper.Map<CategoryDto>(category);        
    }
}