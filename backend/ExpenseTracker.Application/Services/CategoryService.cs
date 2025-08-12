
using AutoMapper;
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
        return _mapper.Map<CategoryDto?>(category);
    }

    public async Task<Guid> CreateAsync(CreateCategoryDto dto, CancellationToken cancellationToken = default)
    {
        var category = _mapper.Map<Category>(dto);
        await _categoryRepository.AddAsync(category, cancellationToken);
        return category.Id;
    }

    public async Task UpdateAsync(Guid id, UpdateCategoryDto dto, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
        if (category is null)
        {
            throw new KeyNotFoundException($"Category with id {id} not found");
        }

        _mapper.Map(dto, category);
        await _categoryRepository.UpdateAsync(category, cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
        if (category is null)
        {
            throw new KeyNotFoundException($"Category with id {id} not found");
        }

        await _categoryRepository.DeleteAsync(category, cancellationToken);
    }
}   
