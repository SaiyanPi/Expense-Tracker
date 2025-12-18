using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Models;

namespace ExpenseTracker.Domain.Interfaces.Repositories;

public interface IDashboardRepository

{
    Task<decimal> GetTotalExpensesForMonthAsync(
        string userId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);

    Task<decimal> GetTotalBudgetForMonthAsync(
        string userId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DashboardCategoryExpenseSummary>> GetExpensesByCategoryForMonthAsync(
        string userId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);
    
    Task<IReadOnlyList<DashboardDailyExpenseSummary>> GetDailyExpensesForMonthAsync(
        string userId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Expense>> GetRecentExpensesForMonthAsync(
        string userId,
        DateTime startDate,
        DateTime endDate,
        int take,
        CancellationToken cancellationToken = default);
}
