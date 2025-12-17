using AutoMapper;
using ExpenseTracker.Application.Common.Pagination;
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

    public async Task<(IReadOnlyList<Expense> Expenses, int TotalCount)> GetAllAsync(
        int skip,
        int take,
        string? sortBy = null,
        bool sortDesc = false,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Expenses
            .AsQueryable();
        
        var totalCount = await _dbContext.Expenses
            .CountAsync(cancellationToken);

        // // dynamic sorting
        // if (!string.IsNullOrWhiteSpace(sortBy))
        // {
        //     query = sortDesc 
        //         ? query.OrderByDescending(e => EF.Property<object>(e, sortBy))
        //         : query.OrderBy(e => EF.Property<object>(e, sortBy));
        // }
        query = query.ApplySorting(sortBy, sortDesc);

        var expenses = await query
            .Include(e => e.Category) // to display the Category name
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        return (expenses, totalCount);
    }

    public async Task<(IReadOnlyList<Expense> Expenses, int TotalCount)> GetExpensesByEmailAsync(
        string userId,
        int skip,
        int take,
        string? sortBy = null,
        bool sortDesc = false,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Expenses
            .Where(e => e.UserId == userId)
            .AsQueryable();
        
        var totalCount = await query
            .CountAsync(cancellationToken);

        query = query.ApplySorting(sortBy, sortDesc);

        var expenses = await query
            .Include(e => e.Category)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        return (expenses, totalCount);
    }

    public async Task<(IReadOnlyList<ExpenseSummaryForBudget> Expenses, int TotalCount)> GetAllExpensesForABudgetByEmailAsync(
        Guid budgetId,
        string userId,
        int skip,
        int take,
        string? sortBy = null,
        bool sortDesc = false,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Expenses
            .Include(e => e.Category)
            .Where(e => e.BudgetId == budgetId && e.UserId == userId)
            .AsQueryable();
        
        var totalCount = await query
            .CountAsync(cancellationToken);

        query = query.ApplySorting(sortBy, sortDesc);

        var expensesOfBudget = await query
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
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
        
        return (expensesOfBudget, totalCount);
        
    }

    public async Task<(IReadOnlyList<ExpenseSummaryForCategory> Expenses, int TotalCount)> GetExpensesForACategoryByEmailAsync(
        Guid categoryId,
        string userId,
        int skip,
        int take,
        string? sortBy = null,
        bool sortDesc = false,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Expenses
            .Include(e => e.Category)
            .Where(e => e.CategoryId == categoryId && e.UserId == userId)
            .AsQueryable();
        
        var totalCount = await query
            .CountAsync(cancellationToken);

        query = query.ApplySorting(sortBy, sortDesc);

        var expensesOfCategory = await query
            .Select(e => new ExpenseSummaryForCategory
            {
                Id =  e.Id,
                Title = e.Title,
                Amount = e.Amount,
                Date = e.Date,
                CategoryId = e.Category.Id,
                CategoryName = e.Category.Name,
                BudgetId = e.BudgetId,
                UserId = e.UserId!
            })
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
        
        return (expensesOfCategory, totalCount);
        
    }

    public async Task<(IReadOnlyList<CategorySummary> CategorySummary, int TotalCount)> GetCategorySummaryAsync(
        int skip,
        int take,
        string? sortBy = null,
        bool sortDesc = false,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Expenses
            .Include(e => e.Category)
            .AsQueryable()
            .GroupBy(e => e.Category.Name)
            .Select(g => new CategorySummary
            {
                CategoryName = g.Key,
                TotalAmount = g.Sum(e => e.Amount)
            });
        
        var totalCount = await query
            .CountAsync(cancellationToken);

        // query = query.ApplySorting(sortBy, sortDesc);    // this sorts Expenses, not the grouped result(CategorySummary)

        // Apply sorting on CategorySummary 
        query = sortBy?.ToLower() switch
        {
            "categoryname" => sortDesc 
                ? query.OrderByDescending(c => c.CategoryName)
                : query.OrderBy(c => c.CategoryName),

            "totalamount" => sortDesc
                ? query.OrderByDescending(c => c.TotalAmount)
                : query.OrderBy(c => c.TotalAmount),

            _ => query.OrderBy(c => c.CategoryName) // default
        };

        var categorySummary = await query
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        return (categorySummary, totalCount);
    }

    public async Task<(IReadOnlyList<CategorySummary> CategorySummaryByEmail, int TotalCount)> GetCategorySummaryByEmailAsync(
        string userId,
        int skip,
        int take,
        string? sortBy = null,
        bool sortDesc = false,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Expenses
            .Include(e => e.Category)
            .Where(e => e.UserId == userId)
            .AsQueryable()
            .GroupBy(e => e.Category.Name)
            .Select(g => new CategorySummary
            {
                CategoryName = g.Key,
                TotalAmount = g.Sum(e => e.Amount)
            });
        
        var totalCount = await query
            .CountAsync(cancellationToken);

        query = sortBy?.ToLower() switch
        {
            "categoryname" => sortDesc 
                ? query.OrderByDescending(c => c.CategoryName)
                : query.OrderBy(c => c.CategoryName),

            "totalamount" => sortDesc
                ? query.OrderByDescending(c => c.TotalAmount)
                : query.OrderBy(c => c.TotalAmount),

            _ => query.OrderBy(c => c.CategoryName) // default
        };

        var categorySummaryByEmail = await query
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        return (categorySummaryByEmail, totalCount);
    }

  

    public async Task<Expense?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var expense = await _dbContext.Expenses.FindAsync(id);
        return expense;
    }

      public async Task<decimal> GetTotalExpenseAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Expenses.SumAsync(e => e.Amount, cancellationToken);
    }

    public async Task<decimal> GetTotalExpenseByEmailAsync(string userId, CancellationToken cancellationToken = default)
    {
        var total = await _dbContext.Expenses
            .Where(e => e.UserId == userId)
            .SumAsync(e => e.Amount, cancellationToken);
        return total;
    }

    public async Task<FilteredExpensesResult> GetFilterExpensesAsync(
        DateTime? startDate,
        DateTime? endDate,
        decimal? minAmount,
        decimal? maxAmount,
        Guid? categoryId,
        string? userId,

        int skip,
        int take,
        string? sortBy = null,
        bool sortDesc = false,
        CancellationToken cancellationToken = default)
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

        var totalCount = await query.CountAsync(cancellationToken);
        
        // apply sorting after filtering query
        query = query.ApplySorting(sortBy, sortDesc);

        var expenses = await query
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        return new FilteredExpensesResult
        {
            TotalAmount = expenses.Sum(e => e.Amount),
            Expenses = expenses,
            TotalCount = totalCount
        };
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

    public async Task<IReadOnlyList<Expense>> GetExpensesForExportAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        var expenses = await _dbContext.Expenses
            .Include(e => e.Category)
            .Include(e => e.Budget)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return expenses;
    }

    // Additional method to check for existing title for validation in service in Application layer
    public async Task<bool> ExistsByTitleAsync(string title, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Expenses.AnyAsync(e => e.Title == title, cancellationToken);
    }
    

}

// This is the implementation of repositopry interface in domain layer