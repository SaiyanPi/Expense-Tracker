using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Models;
using ExpenseTrackerDomain.Models;

namespace ExpenseTracker.Domain.Interfaces.Repositories;

public interface IBudgetRepository
{
    Task<IEnumerable<Budget>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Budget>> GetAllBudgetsByEmailAsync(string userId, CancellationToken cancellationToken = default);
    Task <Budget?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Budget budget, CancellationToken cancellationToken = default);
    Task UpdateAsync(Budget budget, CancellationToken cancellationToken = default);
    Task DeleteAsync(Budget budget, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<BudgetSummary>> GetBudgetSummaryByEmailAsync(string userId, CancellationToken cancellationToken = default);
    Task<BudgetDetailWithExpensesSummary> GetBudgetDetailWithExpensesByEmailAsync(Guid budgetId, string userId, CancellationToken cancellationToken = default);

}