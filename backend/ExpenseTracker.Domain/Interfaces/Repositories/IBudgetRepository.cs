using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Models;
using ExpenseTrackerDomain.Models;

namespace ExpenseTracker.Domain.Interfaces.Repositories;

public interface IBudgetRepository
{
    Task<(IEnumerable<Budget> Budgets, int totalCount)> GetAllAsync(
        int skip,
        int take,
        string? sortBy = null,
        bool sortDesc = false,
        CancellationToken cancellationToken = default);

    Task<(IEnumerable<Budget> Budgets, int totalCount)> GetAllBudgetsByEmailAsync(
        string userId,
        int skip,
        int take,
        string? sortBy = null,
        bool sortDesc = false,
        CancellationToken cancellationToken = default);

    Task <Budget?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task <bool> GetBudgetStatusByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<BudgetDetailWithExpensesSummary> GetBudgetDetailWithExpensesByEmailAsync(
        Guid budgetId,
        string userId,

        int skip,
        int take,
        string? sortBy = null,
        bool sortDesc = false,
        CancellationToken cancellationToken = default);
    
    Task<BudgetsSummary> GetBudgetsSummaryByEmailAsync(
        string userId,

        int skip,
        int take,
        string? sortBy = null,
        bool sortDesc = false,
        CancellationToken cancellationToken = default);
        
    Task AddAsync(Budget budget, CancellationToken cancellationToken = default);
    Task UpdateAsync(Budget budget, CancellationToken cancellationToken = default);
    Task DeleteAsync(Budget budget, CancellationToken cancellationToken = default);

    Task<bool> UserOwnsBudgetAsync(Guid budgetId, string userId, CancellationToken cancellationToken = default);

}