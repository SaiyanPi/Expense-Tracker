using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Models;
using ExpenseTracker.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Domain.Interfaces.Repositories;

public class DashBoardRepository : IDashboardRepository
{
    private readonly ExpenseTrackerDbContext _dbContext;
    public DashBoardRepository(ExpenseTrackerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<decimal> GetTotalExpensesAsync(
        string userId,
        DateTime startDate,
        DateTime endDateExclusive,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Expenses
            .Where(e => e.UserId == userId && e.Date >= startDate && e.Date < endDateExclusive)
            .SumAsync(e => e.Amount, cancellationToken);
    }

    public async Task<decimal> GetTotalBudgetAsync(
        string userId,
        DateTime startDate,
        DateTime endDateExclusive,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Budgets
            // .Where(b => b.UserId == userId && b.StartDate >= startDate && b.EndDate <= endDate)  // Budget is entirely contained within the specified range.
            .Where(b => b.UserId == userId && b.StartDate < endDateExclusive && b.EndDate >= startDate) // Any budget that touches the range, even partially, will be included.
            .SumAsync(b => b.Amount, cancellationToken);
    }

    public async Task<IReadOnlyList<DashboardCategoryExpenseSummary>> GetExpensesByCategoryAsync(
        string userId,
        DateTime startDate,
        DateTime endDateExclusive,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Expenses
            .Where(e => e.UserId == userId && e.Date >= startDate && e.Date < endDateExclusive)
            .GroupBy(e => e.Category)
            .Select(g => new DashboardCategoryExpenseSummary
            {
                Category = g.Key.Name,
                TotalAmount = g.Sum(x => x.Amount)
            })
        .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<DashboardBudgetUtilizationSummary>> GetBudgetUtilizationAsync(
        string userId,
        DateTime startDate,
        DateTime endDateExclusive,
        CancellationToken cancellationToken = default)
    {
        var expenseTotals = _dbContext.Expenses
            .AsNoTracking()
            .Where(e =>
                e.UserId == userId &&
                e.BudgetId != null &&
                e.CreatedAt >= startDate &&
                e.CreatedAt < endDateExclusive)
            .GroupBy(e => e.BudgetId!.Value)
            .Select(g => new
            {
                BudgetId = g.Key,
                ActualSpent = g.Sum(e => e.Amount)
            });

        return await _dbContext.Budgets
            .AsNoTracking()
            .Where(b =>
                b.UserId == userId &&
                b.StartDate < endDateExclusive &&
                b.EndDate >= startDate)
            .GroupJoin(
                expenseTotals,
                budget => budget.Id,
                expense => expense.BudgetId,
                (budget, expenses) => new
                {
                    budget.Name,
                    BudgetTarget = budget.Amount,
                    ActualSpent = expenses
                        .Select(e => (decimal?)e.ActualSpent)
                        .FirstOrDefault()
                })
            .Select(x => new DashboardBudgetUtilizationSummary
            {
                BudgetName = x.Name,
                BudgetTarget = x.BudgetTarget,
                ActualSpent = x.ActualSpent
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<DashboardDailyExpenseSummary>> GetDailyExpensesAsync(
        string userId,
        DateTime startDate,
        DateTime endDateExclusive,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Expenses
            .Where(e => e.UserId == userId && e.Date >= startDate && e.Date < endDateExclusive)
            .GroupBy(e => DateOnly.FromDateTime(e.Date))
            .Select(g => new DashboardDailyExpenseSummary
            {
                Date = g.Key,
                TotalAmount = g.Sum(x => x.Amount)
            })
        .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Expense>> GetRecentExpensesAsync(
        string userId,
        DateTime startDate,
        DateTime endDateExclusive,
        int take,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Expenses
            .Where(e => e.UserId == userId && e.Date >= startDate && e.Date < endDateExclusive)
            .OrderByDescending(e => e.Date)
            .Take(take)
            .ToListAsync(cancellationToken);
    }
}