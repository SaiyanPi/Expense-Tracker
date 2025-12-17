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

    public async Task<decimal> GetTotalExpensesForMonthAsync(
        string userId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Expenses
            .Where(e => e.UserId == userId && e.Date >= startDate && e.Date <= endDate)
            .SumAsync(e => e.Amount, cancellationToken);
    }

    public async Task<decimal> GetTotalBudgetForMonthAsync(
        string userId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Budgets
            // .Where(b => b.UserId == userId && b.StartDate >= startDate && b.EndDate <= endDate)  // Budget is entirely contained within the specified range.
            .Where(b => b.UserId == userId && b.StartDate <= startDate && b.EndDate >= endDate) // Any budget that touches the range, even partially, will be included.
            .SumAsync(b => b.Amount, cancellationToken);
    }

    public async Task<IReadOnlyList<(string Category, decimal TotalAmount)>> GetExpensesByCategoryForMonthAsync(
        string userId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Expenses
            .Where(e => e.UserId == userId && e.Date >= startDate && e.Date <= endDate)
            .GroupBy(e => e.Category)
            .Select(g => new ValueTuple<string, decimal>(
                g.Key.Name,
                g.Sum(x => x.Amount)))
        .ToListAsync(cancellationToken);
    }
}