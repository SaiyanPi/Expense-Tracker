using ExpenseTracker.Domain.Entities;

namespace ExpenseTracker.Domain.Interfaces.Repositories;

public interface IUserRepository
{
    Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<User?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task RegisterAsync(User user, CancellationToken cancellationToken = default);
    Task UpdateAsync(User user, CancellationToken cancellationToken = default);
    Task DeleteAsync(User user, CancellationToken cancellationToken = default);

    // Authentication related
    Task<bool> CheckPasswordAsync(string email, string password);
    Task<IList<string>> GetRolesAsync(string email);

    // Refresh token management
    Task SetRefreshTokenAsync(string email, string refreshToken, DateTime expiryTime);
    Task<(string? refreshToken, DateTime? expiryTime)> GetRefreshTokenAsync(string email);
}