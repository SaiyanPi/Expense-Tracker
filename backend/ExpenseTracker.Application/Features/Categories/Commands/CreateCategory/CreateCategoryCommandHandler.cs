using AutoMapper;
using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.DTOs.Category;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;

namespace ExpenseTracker.Application.Features.Categories.Commands.CreateCategory;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryDto>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    public CreateCategoryCommandHandler(ICategoryRepository categoryRepository,
        IUserRepository userRepository, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }
    public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        // check if the userId exist
        if (request.CreateCategoryDto.UserId is not null)
        {
            var userExists = await _userRepository.GetByIdAsync(request.CreateCategoryDto.UserId, cancellationToken);
            if (userExists is null)
                throw new NotFoundException(nameof(User), request.CreateCategoryDto.UserId);

            // business rule: category name must be unique per user
            var exists = await _categoryRepository.ExistsByNameAndUserIdAsync(request.CreateCategoryDto.Name, request.CreateCategoryDto.UserId, cancellationToken);
            if (exists)
                throw new ConflictException($"Category with name '{request.CreateCategoryDto.Name}' already exists for user '{request.CreateCategoryDto.UserId}'.");
        }
        
        var category = _mapper.Map<Category>(request.CreateCategoryDto);
        await _categoryRepository.AddAsync(category, cancellationToken);
        return _mapper.Map<CategoryDto>(category);
    }
}