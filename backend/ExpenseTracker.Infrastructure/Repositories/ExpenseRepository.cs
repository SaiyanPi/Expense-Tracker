using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using ExpenseTracker.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Infrastructure.Repositories;

public class ExpenseRepository : IExpenseRepository
{
    private readonly ExpenseTrackerDbContext _dbContext;
    public ExpenseRepository(ExpenseTrackerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Expense>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var expenses = await _dbContext.Expenses
            .Include(e => e.Category) // to display the Category name
            .ToListAsync();
        return expenses;
    }

    public async Task<Expense?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var expense = await _dbContext.Expenses.FindAsync(id);
        return expense;
    }

    public async Task AddAsync(Expense expense, CancellationToken cancellationToken = default)
    {
        await _dbContext.Expenses.AddAsync(expense);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Expense expense, CancellationToken cancellationToken = default)
    {
        _dbContext.Expenses.Update(expense);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Expense expense, CancellationToken cancellationToken = default)
    {
        _dbContext.Expenses.Remove(expense);
        await _dbContext.SaveChangesAsync();
    }

}

// This is the implementation of repositopry interface in domain layer