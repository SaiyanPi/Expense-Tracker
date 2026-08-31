using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Models;

namespace ExpenseTracker.Domain.Interfaces.Repositories;

public interface IDashboardRepository

{
    Task<decimal> GetTotalExpensesAsync(
        string userId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);

    Task<decimal> GetTotalBudgetAsync(
        string userId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DashboardCategoryExpenseSummary>> GetExpensesByCategoryAsync(
        string userId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DashboardBudgetUtilizationSummary>> GetBudgetUtilizationAsync(
        string userId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);
    
    Task<IReadOnlyList<DashboardDailyExpenseSummary>> GetDailyExpensesAsync(
        string userId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Expense>> GetRecentExpensesAsync(
        string userId,
        DateTime startDate,
        DateTime endDate,
        int take,
        CancellationToken cancellationToken = default);
}
