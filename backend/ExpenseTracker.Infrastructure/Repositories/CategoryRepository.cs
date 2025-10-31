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

    public async Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _dbContext.Categories.ToListAsync();
        return categories;
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
        return await _dbContext.Categories.AnyAsync(c => c.Name == name && c.UserId == userId, cancellationToken);
    }

}

// This is the implementation of repositopry interface in domain layer