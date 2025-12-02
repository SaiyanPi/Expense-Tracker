using ExpenseTracker.Domain.Entities;

namespace ExpenseTracker.Domain.Interfaces.Repositories;

public interface IUserRepository
{
    // USER MANAGEMENT ONLY
    Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<User?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(User user, CancellationToken cancellationToken = default);
    

}