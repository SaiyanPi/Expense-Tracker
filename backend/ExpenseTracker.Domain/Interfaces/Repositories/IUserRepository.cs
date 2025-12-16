using ExpenseTracker.Domain.Entities;

namespace ExpenseTracker.Domain.Interfaces.Repositories;

public interface IUserRepository
{
    // USER MANAGEMENT ONLY
    Task<(IReadOnlyList<User> Users, int TotalCount)> GetAllAsync(
        int skip,
        int take,
        string? sortBy = null,
        bool sortDesc = false,
        CancellationToken cancellationToken = default);
    Task<User?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(User user, CancellationToken cancellationToken = default);
    

}