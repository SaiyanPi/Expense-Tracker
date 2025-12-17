using ExpenseTracker.Domain.Entities;

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

    Task<IReadOnlyList<(string Category, decimal TotalAmount)>> GetExpensesByCategoryForMonthAsync(
        string userId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);
    
    Task<IReadOnlyList<(DateOnly Date, decimal TotalAmount)>> GetDailyExpensesForMonthAsync(
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
