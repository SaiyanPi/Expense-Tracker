using ExpenseTracker.Application.Common.Pagination;
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

    public async Task<(IEnumerable<Budget> Budgets, int totalCount)> GetAllAsync(
        int skip,
        int take,
        string? sortBy = null,
        bool sortDesc = false,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Budgets
            .AsNoTracking()
            .AsQueryable();

        var totalCount = await query
            .CountAsync(cancellationToken);
        
        // apply sorting
        query = query.ApplySorting(sortBy, sortDesc);

        var budgets = await query
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        
        return (budgets, totalCount);
    }
   

    public async Task<(IEnumerable<Budget> Budgets, int totalCount)> GetAllBudgetsByEmailAsync(
        string userId,
        int skip,
        int take,
        string? sortBy = null,
        bool sortDesc = false,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Budgets
            .Where(b => b.UserId == userId)
            .AsNoTracking()
            .AsQueryable();

        var totalCount = await query
            .CountAsync(cancellationToken);

        query = query.ApplySorting(sortBy, sortDesc);

        var budgets = await query
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        return (budgets, totalCount);
    }

    public async Task<Budget?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var budget = await _dbContext.Budgets
            .Include(b => b.Expenses)   // this is required for Deletion to check if the budget has any expense(s)
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
            
        return budget;
    }

    public async Task<bool> GetBudgetStatusByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Budgets
            .Where(b => b.Id == id && b.StartDate <= DateTime.Now && b.EndDate >= DateTime.Now)
            .FirstOrDefaultAsync(cancellationToken) != null;
    }

    public async Task<BudgetDetailWithExpensesSummary> GetBudgetDetailWithExpensesByEmailAsync(
        Guid budgetId,
        string userId,
        
        int skip,
        int take,
        string? sortBy = null,
        bool sortDesc = false,
        CancellationToken cancellationToken = default)
    {
        // 1. Load budget
        var budget = await _dbContext.Budgets
            .Where(b => b.Id == budgetId && b.UserId == userId)
            .Select(b => new BudgetDetailWithExpensesSummary
            {
                Id = b.Id,
                Name = b.Name,
                Limit = b.Amount,
                IsActive = b.IsActive

            })
            .FirstOrDefaultAsync(cancellationToken);

        if (budget == null)
        {
            return new BudgetDetailWithExpensesSummary();
        }
        var query = _dbContext.Expenses
            .Where(e => e.BudgetId == budgetId && e.UserId == userId)
            .Select(e => new ExpenseSummary
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description,
                Amount = e.Amount,
                Date = e.Date,
                CategoryId = e.Category != null ? e.Category.Id : null,
                CategoryName = e.Category != null ? e.Category.Name : null,  
                BudgetId = e.BudgetId,
                UserId = e.UserId
            })
            .AsQueryable();
        // Calculate value
        var totalSpent = query.Sum(expenses => expenses.Amount);

        var totalCount = await query.CountAsync(cancellationToken);

        // apply sorting
        query = query.ApplySorting(sortBy, sortDesc);

        // load related expenses
        var expenses = await query
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
        
        // Build and return domain model
        return new BudgetDetailWithExpensesSummary
        {
            Id = budget.Id,
            Name = budget.Name,
            Limit = budget.Limit,
            TotalSpent = totalSpent,
            IsActive = budget.IsActive,
            Expenses = expenses,
            TotalCount = totalCount
        };
    }           

    public async Task<BudgetsSummary> GetBudgetsSummaryByEmailAsync(
        string userId, 
        
        int skip,
        int take,
        string? sortBy = null,
        bool sortDesc = false,
        CancellationToken cancellationToken = default)
    {
        // 1. get all budgets for the user
        var budgets = await _dbContext.Budgets
            .Where(b => b.UserId == userId)
            .ToListAsync(cancellationToken);
        
        if(!budgets.Any())
        {
            return new BudgetsSummary();
        }
        
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
        var summary = new BudgetsSummary
        {
            TotalBudget = budgets.Sum(b => b.Amount),
            TotalExpenses = expenses.Sum(e => e.Amount)
        };

        // 6. category-wise summary
        var query = budgets
            .GroupBy(b => b.CategoryId)
            .Select(group =>
            {
                var categoryId = group.Key;
                var categoryBudget = group.Sum(b => b.Amount);

                var categorySpent = expenses.Where(e => e.CategoryId == categoryId).Sum(e => e.Amount);
                var categoryEntity = categories.FirstOrDefault(c => c.Id == categoryId);
                
                return new BudgetCategorySummary
                {
                    BudgetAmount = categoryBudget,
                    ExpensesAmount = categorySpent,
                    CategoryId = categoryId ?? null,
                    CategoryName = categoryEntity?.Name ?? null
                };
            })
            .AsQueryable();

        //  var totalCount = await query.CountAsync(cancellationToken);
        // Above line leads to exception because query is not an EF query anymore since it is not fresh from db but from already loaded
        // in memory(step 1). So we use the below line instead.

        //var totalCount = query.Count();

        // apply sorting
        query = query.ApplySorting(sortBy, sortDesc);

        summary.Categories = query
            .Skip(skip)
            .Take(take)
            .ToList();
        
        summary.TotalCount = query.Count();
        
        return summary;

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

    public async Task<bool> UserOwnsBudgetAsync(Guid budgetId, string userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Budgets
            .AnyAsync(b => b.Id == budgetId && b.UserId == userId, cancellationToken);
    }

    public async Task<bool>ExistByNameUserIdAndCategoryIdAsync(string name, string userId, Guid? excludeBudgetId, Guid catId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Budgets.AnyAsync(b =>
            b.Name == name &&
            b.UserId == userId &&
            (!excludeBudgetId.HasValue || b.Id != excludeBudgetId) &&
            b.CategoryId == catId,
            cancellationToken);
    }

    // view and restore soft deleted expenses
    public async Task<(IReadOnlyList<Budget> Budgets, int TotalCount)> GetAllDeletedBudgetsByEmailAsync(
        string userId,
        int skip,
        int take,
        string? sortBy = null,
        bool sortDesc = false,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Budgets
            .IgnoreQueryFilters()
            .Where(e => e.IsDeleted && e.UserId == userId)
            .AsNoTracking()
            .AsQueryable();

        var totalCount = await query
            .CountAsync(cancellationToken);

        query = query.ApplySorting(sortBy, sortDesc);

        var softDeletedBudgets = await query
            .Include(e => e.Expenses)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        return (softDeletedBudgets, totalCount);
    }

    public async Task<Budget?> GetDeletedBudgetAsync(
        Guid id, 
        string userId, 
        CancellationToken cancellationToken = default)
    {
        var softDeletedBudget = await _dbContext.Budgets
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId && e.IsDeleted);
        
        return softDeletedBudget;
    }

    public async Task<bool> RestoreDeletedBudgetAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync();

        return true;
    }

}