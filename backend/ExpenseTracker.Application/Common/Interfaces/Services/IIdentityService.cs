using ExpenseTracker.Application.DTOs.Auth;

namespace ExpenseTracker.Application.Common.Interfaces.Services;

public interface IIdentityService
{
    Task<AuthResultDto> RegisterUserAsync(RegisterUserDto dto, string role, CancellationToken cancellationToken = default);
    Task<AuthResultDto> LoginAsync(LoginUserDto dto, CancellationToken cancellationToken = default);
    Task UpdateAsync(string userId, UpdateUserDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(string userId, CancellationToken cancellationToken = default);
    // GetAll, GetById, GetByEmail methods are not considered identity operation therefore they
    // resides inside user repository
    Task LogoutAsync(string userId, CancellationToken cancellationToken = default);
    Task<AuthResultDto> RefreshTokenAsync(RefreshTokenDto dto, CancellationToken cancellationToken = default);
    Task ChangePasswordAsync(string userId, ChangePasswordDto dto, CancellationToken cancellationToken = default);
    

    // email confirmation
    Task RequestEmailConfirmationTokenAsync(RequestEmailConfirmationDto dto, CancellationToken cancellationToken = default);
    Task ConfirmEmailAsync(VerifyEmailDto dto, CancellationToken cancellationToken = default);

    // password reset
    Task ForgotPasswordResetTokenAsync(ForgotPasswordDto dto, CancellationToken cancellationToken = default);
    Task ResetPasswordAsync(string userId, string token, ResetPasswordDto dto, CancellationToken cancellationToken = default);
    
    // change email
    Task RequestChangeEmailAsync(string userId, ChangeEmailRequestDto dto, CancellationToken cancellationToken = default);
    Task ConfirmChangeEmailAsync(ConfirmChangeEmailDto dto, CancellationToken cancellationToken = default);
    
    // phone confirmation
    Task GeneratePhoneConfirmationTokenAsync(PhoneConfirmationDto dto, CancellationToken cancellationToken = default);
    Task ConfirmPhoneNumberAsync(VerifyPhoneDto dto, CancellationToken cancellationToken = default);
}
