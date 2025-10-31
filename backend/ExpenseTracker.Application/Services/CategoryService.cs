
using AutoMapper;
using ExpenseTracker.Application.Exceptions;
using ExpenseTracker.Application.Interfaces.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using ExpenseTrackler.Application.DTOs.Category;
namespace ExpenseTracker.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;
    public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }


    public async Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _categoryRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<CategoryDto>>(categories);
    }

    public async Task<CategoryDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
        if (category is null)
            throw new NotFoundException(nameof(Category), id);

        return _mapper.Map<CategoryDto?>(category);
    }

    public async Task<Guid> CreateAsync(CreateCategoryDto dto, CancellationToken cancellationToken = default)
    {
        // business rule: category name must be unique per user
        var exists = await _categoryRepository.ExistsByNameAndUserIdAsync(dto.Name, dto.UserId, cancellationToken);
        if (exists)
            throw new ConflictException($"Category with name '{dto.Name}' already exists for user '{dto.UserId}'.");

        var category = _mapper.Map<Category>(dto);
        await _categoryRepository.AddAsync(category, cancellationToken);
        return category.Id;
    }

    public async Task UpdateAsync(Guid id, UpdateCategoryDto dto, CancellationToken cancellationToken = default)
    {
        // check if category exists
        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
        if (category is null)
            throw new NotFoundException(nameof(Category), id);

        // business rule: category name must be unique per user
        var exists = await _categoryRepository.ExistsByNameAndUserIdAsync(dto.Name, dto.UserId, cancellationToken);
        if (exists && !string.Equals(category.Name, dto.Name, StringComparison.OrdinalIgnoreCase))
            throw new ValidationException($"Category with name '{dto.Name}' already exists for user '{dto.UserId}'.");
        
        // business rule: user must own the category
        if (category.UserId != dto.UserId)
            throw new UnauthorizedException("You are not authorized to update this category.");

        _mapper.Map(dto, category);

        // save changes
        try
        {
            await _categoryRepository.UpdateAsync(category, cancellationToken);
        }
        catch (UnauthorizedAccessException)
        {
            throw new UnauthorizedException("You are not authorized to update this category.");
        }
        catch (Exception ex)
        {
            throw new ValidationException($"Failed to update category: {ex.Message}");
        }
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
        if (category is null)
            throw new NotFoundException(nameof(Category), id);

        await _categoryRepository.DeleteAsync(category, cancellationToken);
    }
}   
