using ExpenseTracker.Domain.Entities;

namespace ExpenseTracker.Domain.Interfaces.Repositories;

public interface IIdentityRepository
{
    Task<(bool Succeeded, IEnumerable<string>? Errors, User? User)> RegisterAsync( User user, string password, 
        string role, CancellationToken cancellationToken = default);
    Task<bool> CheckPasswordAsync(string email, string password, CancellationToken cancellationToken = default);

    Task<string> GenerateJwtTokenAsync(User user, CancellationToken cancellationToken = default);
    Task<string> GenerateRefreshTokenAsync(User user, CancellationToken cancellationToken = default);
    Task<bool> StoreRefreshTokenAsync(string userId, string refreshToken, CancellationToken cancellationToken = default);
    Task<bool> ValidateRefreshTokenAsync(string userId, string refreshToken, CancellationToken cancellationToken = default);
    Task<bool> RevokeRefreshTokenAsync(string userId, string refreshToken, CancellationToken cancellationToken = default);
    Task<string?> GetRefreshTokenAsync(string userId, CancellationToken cancellationToken = default);
    Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default);

    // email confirmation
    Task<string?> GenerateEmailConfirmationTokenAsync(string userId, CancellationToken cancellationToken = default);
    Task<bool> ConfirmEmailAsync(string userId, string token, CancellationToken cancellationToken = default);

    // password reset
    Task<string?> GeneratePasswordResetTokenAsync(string userId, CancellationToken cancellationToken = default);
    Task<bool> ResetPasswordAsync(string userId, string token, string newPassword, CancellationToken cancellationToken = default);
}
