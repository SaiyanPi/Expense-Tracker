using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Security;

namespace ExpenseTracker.Domain.Interfaces.Repositories;

public interface IIdentityRepository
{
    Task<(bool Succeeded, IEnumerable<string>? Errors, User? User)> RegisterAsync( User user, string password, 
        string? role);
    
    Task<bool> UpdateAsync(User user);
    Task<bool> DeleteAsync(User user);
    Task<bool> CheckPasswordAsync(string email, string password);

    Task<JwtToken> GenerateJwtTokenAsync(User user);
    string GenerateRefreshToken();
    Task<bool> StoreRefreshTokenAsync(string userId, string refreshToken);
    Task<bool> ValidateRefreshTokenAsync(string userId, string refreshToken);
    Task<bool> RevokeRefreshTokenAsync(string userId, string refreshToken);
    Task<string?> GetRefreshTokenAsync(string userId);
    Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword);

    // email confirmation
    Task<string?> GenerateEmailConfirmationTokenAsync(string userId);
    Task<bool> ConfirmEmailAsync(string userId, string token);

    // password reset
    Task<string?> GeneratePasswordResetTokenAsync(string userId);
    Task<bool> ResetPasswordAsync(string userId, string token, string newPassword);

    // change email
    Task<bool> IsEmailTakenAsync(string email);
    Task<string?> GenerateChangeEmailTokenAsync(string userId, string newEmail);
    Task<bool> ChangeEmailAsync(string userId, string newEmail, string token);

    // phone confirmation
    Task<string> GeneratePhoneConfirmationTokenAsync(string userId, string phoneNumber);
    Task<bool> ConfirmPhoneNumberAsync(string userId, string phoneNumber, string token);

}
