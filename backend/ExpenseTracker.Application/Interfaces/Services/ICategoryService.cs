using ExpenseTrackler.Application.DTOs.Category;

namespace ExpenseTracker.Application.Interfaces.Services;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>> GetAllAsync();
    Task<CategoryDto?> GetByIdAsync(Guid id);
    Task<Guid> CreateAsync(CreateCategoryDto dto);
    Task UpdateAsync(UpdateCategoryDto dto);
    Task DeleteAsync(Guid id);
}
