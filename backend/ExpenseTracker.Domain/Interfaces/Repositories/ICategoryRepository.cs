using ExpenseTracker.Domain.Entities;

namespace ExpenseTracker.Domain.Interfaces.Repositories;

public interface ICategoryRepository
{
    Task<(IReadOnlyList<Category> Categories, int TotalCount)> GetAllAsync(
        int skip,
        int take,
        string? sortBy = null,
        bool sortDesc = false,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Category> Categories, int TotalCount)> GetAllCategoriesByEmailAsync(
        string userId,
        int skip,
        int take,
        string? sortBy = null,
        bool sortDesc = false,
        CancellationToken cancellationToken = default);
        
    Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Category category, CancellationToken cancellationToken = default);
    Task UpdateAsync(Category category, CancellationToken cancellationToken = default);
    Task DeleteAsync(Category category, CancellationToken cancellationToken = default);

    // view and restore softdeleted categories
    Task<(IReadOnlyList<Category> Categories, int TotalCount)> GetAllDeletedCategoriesByEmailAsync(
        string userId,
        int skip,
        int take,
        string? sortBy = null,
        bool sortDesc = false,
        CancellationToken cancellationToken = default);
    Task<Category?> GetDeletedCategoryAsync(Guid id, string userId, CancellationToken cancellationToken = default);
    Task<bool> RestoreDeletedCategoryAsync(CancellationToken cancellationToken = default);

    // Additional method to check for existing name for validation in service in Application layer
    //Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAndUserIdAsync(string name, string userId, Guid? excludeCategoryId, CancellationToken cancellationToken = default);
    Task<bool> UserOwnsCategoryAsync(Guid categoryId, string userId, CancellationToken cancellationToken = default);
}