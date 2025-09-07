using ExpenseTracker.Domain.Entities;

namespace ExpenseTracker.Domain.Interfaces.Repositories;

public interface IExpenseRepository
{
    Task<IReadOnlyList<Expense>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Expense?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Expense expense, CancellationToken cancellationToken = default);
    Task UpdateAsync(Expense expense, CancellationToken cancellationToken = default);
    Task DeleteAsync(Expense expense, CancellationToken cancellationToken = default);

    // Additional method to check for existing title for validation in service in Application layer
    Task<bool> ExistsByTitleAsync(string title, CancellationToken cancellationToken = default);
}
// 📝
// 1. Repositories represent the data access layer:
// Repositories work directly with your domain entities(not with DTOs and raw data like Guid IDs) but this
// don’t mean that repositories can’t accept Guid values at all.
// REPOSITORIES SHOULD NOT ACCEPT RAW DATA TO PERFORM BEHAVIOR OR MAKE CHANGES TO ENTITIES. like 'Task DeleteAsync(Guid id);'
// — these are the classes that map closely to your database tables.
// They provide CRUD operations on entities, typically returning entities or collections of entities.
// The repository’s job is to abstract data storage details and keep persistence concerns inside the infrastructure/data access layer.

// 2. Repositories should not be concerned with DTOs because:
// DTOs are designed for communication with external layers (UI, API clients).
// Introducing DTOs in repositories mixes persistence logic with presentation or API concerns.
// It would tightly couple repositories to external data contract changes.

// 3. Repositories usually focus on persistence operations — not on operation results.
// So no result type  for AddAsync, UpdateAsync, and DeleteAsync (like Task<bool>)
