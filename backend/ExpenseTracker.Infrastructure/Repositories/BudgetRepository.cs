using ExpenseTracker.Application.DTOs.Budget;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using ExpenseTracker.Domain.Models;
using ExpenseTracker.Persistence;
using ExpenseTrackerDomain.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Infrastructure.Repositories;

public class BudgetRepository : IBudgetRepository
{
    private readonly ExpenseTrackerDbContext _dbContext;

    public BudgetRepository(ExpenseTrackerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Budget>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Budgets
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Budget>> GetAllBudgetsByEmailAsync(string userId, CancellationToken cancellationToken = default)
    {
        var budgets = await _dbContext.Budgets
            .Where(b => b.UserId == userId)
            .ToListAsync(cancellationToken);
        return budgets;
    }

    public async Task<Budget?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var budget = await _dbContext.Budgets.FindAsync(id);
        return budget;
    }

    public async Task AddAsync(Budget budget, CancellationToken cancellationToken = default)
    {
        await _dbContext.Budgets.AddAsync(budget, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Budget budget, CancellationToken cancellationToken = default)
    {
        _dbContext.Budgets.Update(budget);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Budget budget, CancellationToken cancellationToken = default)
    {
        _dbContext.Budgets.Remove(budget);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<BudgetSummary>> GetBudgetSummaryAsync(string userId, CancellationToken cancellationToken = default)
    {
        // 1. get all budgets with categoryId for the user
        var budgets = await _dbContext.Budgets
            .Where(b => b.UserId == userId)
            .ToListAsync(cancellationToken);
        
        if(!budgets.Any())
            return new List<BudgetSummary>();
        
        // 2. get date range based on all budgets
        var minDate = budgets.Min(b => b.StartDate);
        var maxDate = budgets.Max(b => b.EndDate);


        // 3. get all expenses including category relevant to these periods
        var expenses = await _dbContext.Expenses
            .Where(e => e.UserId == userId && e.Date >= minDate && e.Date <= maxDate)
            .Include(b => b.Category)
            .ToListAsync(cancellationToken);

        // 4. Load categories referenced by budgets
        var categoryIds = budgets
            .Where(b => b.CategoryId.HasValue)
            .Select(b => b.CategoryId!.Value)
            .Distinct()
            .ToList();

        var categories = await _dbContext.Categories
            .Where(c => categoryIds.Contains(c.Id))
            .ToListAsync(cancellationToken);

        // 5. Generate summary            
        var summary = new BudgetSummary
        {
            TotalBudget = budgets.Sum(b => b.Amount),
            TotalExpenses = expenses.Sum(e => e.Amount)
        };

        // 6. category-wise summary
        summary.Categories = budgets
            .GroupBy(b => b.CategoryId)
            .Select(group =>
            {
                var categoryId = group.Key;
                var categoryBudget = group.Sum(b => b.Amount);

                var categorySpent = expenses.Where(e => e.CategoryId == categoryId).Sum(e => e.Amount);
                var categoryEntity = categories.FirstOrDefault(c => c.Id == categoryId);
                
                return new BudgetCategorySummary
                {
                    CategoryId = categoryId ?? Guid.Empty,
                    BudgetAmount = categoryBudget,
                    ExpensesAmount = categorySpent,
                    CategoryName = categoryEntity?.Name ?? string.Empty
                };
            })
            .ToList();
        
        return new List<BudgetSummary> { summary };

    }

}