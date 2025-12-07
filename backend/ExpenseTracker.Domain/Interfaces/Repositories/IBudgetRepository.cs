using ExpenseTracker.Domain.Entities;
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

    Task<IReadOnlyList<BudgetSummary>> GetBudgetSummaryAsync(string userId, CancellationToken cancellationToken = default);

}