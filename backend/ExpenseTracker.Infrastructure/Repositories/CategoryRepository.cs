using ExpenseTracker.Application.Common.Pagination;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using ExpenseTracker.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly ExpenseTrackerDbContext _dbContext;
    public CategoryRepository(ExpenseTrackerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<(IReadOnlyList<Category> Categories, int TotalCount)> GetAllAsync(
        int skip,
        int take,
        string? sortBy = null,
        bool sortDesc = false,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Categories
            .AsNoTracking()
            .AsQueryable();

        var totalCount = await query
            .CountAsync(cancellationToken);
        
        // apply sorting
        query = query.ApplySorting(sortBy, sortDesc);

        var categories = await query
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        return (categories, totalCount);
    }

    public async Task<(IReadOnlyList<Category> Categories, int TotalCount)> GetAllCategoriesByEmailAsync(
        string userId,
        int skip,
        int take,
        string? sortBy = null,
        bool sortDesc = false,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Categories
            .Where(c => c.UserId == userId)
            .AsNoTracking()
            .AsQueryable();

        var totalCount = await query
            .CountAsync(cancellationToken);
        
        query = query.ApplySorting(sortBy, sortDesc);

        var categories = await query
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        return (categories, totalCount);
    }

    public async Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var category = await _dbContext.Categories.FindAsync(id);
        return category;
    }

    public async Task AddAsync(Category category, CancellationToken cancellationToken = default)
    {
        await _dbContext.Categories.AddAsync(category);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Category category, CancellationToken cancellationToken = default)
    {
        _dbContext.Categories.Update(category);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Category category, CancellationToken cancellationToken = default)
    {
        _dbContext.Categories.Remove(category);
        await _dbContext.SaveChangesAsync();
    }

    // Additional method to check for existing name for validation in service in Application layer
    // public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
    // {
    //     return await _dbContext.Categories.AnyAsync(c => c.Name == name, cancellationToken);
    // }

    public async Task<bool> ExistsByNameAndUserIdAsync(string name, string userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Categories
            .AnyAsync(c => c.Name == name && c.UserId == userId, cancellationToken);
    }

    public async Task<bool> UserOwnsCategoryAsync(Guid categoryId, string userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Categories
            .AnyAsync(c => c.Id == categoryId && c.UserId == userId, cancellationToken);
    }

}

// This is the implementation of repositopry interface in domain layer