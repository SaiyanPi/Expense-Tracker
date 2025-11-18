using ExpenseTracker.Domain.Entities;

namespace ExpenseTracker.Domain.Interfaces.Repositories;

public interface IUserRepository
{
    Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<User?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(User user, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(User user, CancellationToken cancellationToken = default);

}