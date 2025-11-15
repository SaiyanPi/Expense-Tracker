using ExpenseTracker.Domain.Entities;

namespace ExpenseTracker.Domain.Interfaces.Repositories;

public interface IIdentityRepository
{
    Task<(bool Succeeded, IEnumerable<string>? Errors, User? User)> RegisterAsync( User user, string password, 
        IEnumerable<string> roles, CancellationToken cancellationToken = default);
    Task<bool> CheckPasswordAsync(string email, string password, CancellationToken cancellationToken = default);

    Task<string> GenerateJwtTokenAsync(User user, CancellationToken cancellationToken = default);
    Task<string> GenerateRefreshTokenAsync(User user, CancellationToken cancellationToken = default);
    Task<bool> StoreRefreshTokenAsync(string userId, string refreshToken, CancellationToken cancellationToken = default);
    Task<bool> ValidateRefreshTokenAsync(string userId, string refreshToken, CancellationToken cancellationToken = default);
    Task<bool> RevokeRefreshTokenAsync(string userId, string refreshToken, CancellationToken cancellationToken = default);
}
