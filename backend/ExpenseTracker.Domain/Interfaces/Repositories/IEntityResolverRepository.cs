using ExpenseTracker.Domain.SharedKernel;

namespace ExpenseTracker.Domain.Interfaces.Repositories;

public interface IEntityResolverRepository
{
    Task<BaseEntity?> ExistsAsync(EntityType entityName, Guid entityId, CancellationToken cancellationToken = default);
}
