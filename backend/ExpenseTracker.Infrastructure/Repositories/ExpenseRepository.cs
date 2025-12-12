using AutoMapper;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using ExpenseTracker.Domain.Models;
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

    public async Task<IReadOnlyList<Expense>> GetAllExpensesByEmailAsync(string userId, CancellationToken cancellationToken = default)
    {
        var expenses = await _dbContext.Expenses
            .Include(e => e.Category)
            .Where(e => e.UserId == userId)
            .ToListAsync(cancellationToken);
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

    // Additional method to check for existing title for validation in service in Application layer
    public async Task<bool> ExistsByTitleAsync(string title, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Expenses.AnyAsync(e => e.Title == title, cancellationToken);
    }

    public async Task<decimal> GetTotalExpensesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Expenses.SumAsync(e => e.Amount, cancellationToken);
    }

    public async Task<decimal> GetTotalExpensesByEmailAsync(string userId, CancellationToken cancellationToken = default)
    {
        var total = await _dbContext.Expenses
            .Where(e => e.UserId == userId)
            .SumAsync(e => e.Amount, cancellationToken);
        return total;
    }

    public async Task<IReadOnlyList<CategorySummary>> GetCategorySummaryAsync(CancellationToken cancellationToken = default)
    {
        var summaries = await _dbContext.Expenses
            .Include(e => e.Category)
            .GroupBy(e => e.Category.Name)
            .Select(g => new CategorySummary
            {
                CategoryName = g.Key,
                TotalAmount = g.Sum(e => e.Amount)
            })
            .ToListAsync(cancellationToken);

        return summaries;
    }

    public async Task<IReadOnlyList<CategorySummary>> GetCategorySummaryByEmailAsync(string userId, CancellationToken cancellationToken = default)
    {
        var summaries = await _dbContext.Expenses
            .Include(e => e.Category)
            .Where(e => e.UserId == userId)
            .GroupBy(e => e.Category.Name)
            .Select(g => new CategorySummary    
            {
                CategoryName = g.Key,
                TotalAmount = g.Sum(e => e.Amount)
            })
            .ToListAsync(cancellationToken);

        return summaries;  
    }

    public async Task<FilteredExpensesResult> FilterExpensesAsync(
                                                                    DateTime? startDate,
                                                                    DateTime? endDate,
                                                                    decimal? minAmount,
                                                                    decimal? maxAmount,
                                                                    Guid? categoryId,
                                                                    string? userId,
                                                                    CancellationToken cancellationToken = default
                                                                )
    {
        var query = _dbContext.Expenses
            .Include(e => e.Category)
            .AsQueryable();

        query = query.Where(e => e. Date >= startDate && e.Date <= endDate);

        if (minAmount.HasValue)
        {
            query = query.Where(e => e.Amount >= minAmount.Value);
        }

        if (maxAmount.HasValue)
        {
            query = query.Where(e => e.Amount <= maxAmount.Value);
        }

        if (categoryId.HasValue)
        {
            query = query.Where(e => e.CategoryId == categoryId.Value);
        }

        if (!string.IsNullOrEmpty(userId))
        {
            query = query.Where(e => e.UserId == userId);
        }

        
        var expenses = await query.ToListAsync(cancellationToken);

        return new FilteredExpensesResult
        {
            TotalAmount = expenses.Sum(e => e.Amount),
            Expenses = expenses
        };
    }

    public async Task<IReadOnlyList<ExpenseSummaryForBudget>> GetAllExpensesForABudgetByEmailAsync(Guid budgetId, string userId, CancellationToken cancellationToken = default)
    {
        var expensesOfBudget = await _dbContext.Expenses
            .Include(e => e.Category)
            .Where(e => e.BudgetId == budgetId && e.UserId == userId)
            .Select(e => new ExpenseSummaryForBudget
            {
                Title = e.Title,
                Amount = e.Amount,
                Date = e.Date,
                CategoryId = e.Category.Id,
                CategoryName = e.Category.Name,
                BudgetId = e.BudgetId,
                UserId = e.UserId
            })
        .ToListAsync(cancellationToken);
        
        return expensesOfBudget;
        
    }

    public async Task<IReadOnlyList<ExpenseSummaryForCategory>> GetExpensesForACategoryByEmailAsync(Guid categoryId, string userId, CancellationToken cancellationToken = default)
    {
        var expensesOfCategory = await _dbContext.Expenses
            .Include(e => e.Category)
            .Where(e => e.CategoryId == categoryId && e.UserId == userId)
            .Select(e => new ExpenseSummaryForCategory
            {
                Id = e.Id,
                Title = e.Title,
                Amount = e.Amount,
                Date = e.Date,
                CategoryId = e.Category.Id,
                CategoryName = e.Category.Name,
                BudgetId = e.BudgetId,
                UserId = e.UserId!
            })
        .ToListAsync(cancellationToken);
        
        return expensesOfCategory;
        
    }

}

// This is the implementation of repositopry interface in domain layer