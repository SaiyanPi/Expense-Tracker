using ExpenseTracker.Application.DTOs.Auth;

namespace ExpenseTracker.Application.Common.Interfaces.Services;

public interface IIdentityService
{
    Task<AuthResultDto> RegisterUserAsync(RegisterUserDto dto, string role, CancellationToken cancellationToken = default);
    Task<AuthResultDto> LoginAsync(LoginUserDto dto, CancellationToken cancellationToken = default);
    Task UpdateAsync(string userId, UpdateUserDto dto, CancellationToken cancellationToken = default);
    Task LogoutAsync(LogoutUserDto dto,CancellationToken cancellationToken = default);
    Task<AuthResultDto> RefreshTokenAsync(RefreshTokenDto dto, CancellationToken cancellationToken = default);
    Task ChangePasswordAsync(ChangePasswordDto dto, CancellationToken cancellationToken = default);

    // email confirmation
    Task RequestEmailConfirmationTokenAsync(RequestEmailConfirmationDto dto, CancellationToken cancellationToken = default);
    Task ConfirmEmailAsync(VerifyEmailDto dto, CancellationToken cancellationToken = default);

    // password reset
    Task ForgotPasswordResetTokenAsync(ForgotPasswordDto dto, CancellationToken cancellationToken = default);
    Task ResetPasswordAsync(ResetPasswordDto dto, CancellationToken cancellationToken = default);
    
    // change email
    Task RequestChangeEmailAsync(ChangeEmailRequestDto dto, CancellationToken cancellationToken = default);
    Task ConfirmChangeEmailAsync(ConfirmChangeEmailDto dto, CancellationToken cancellationToken = default);
    
    // phone confirmation
    Task GeneratePhoneConfirmationTokenAsync(PhoneConfirmationDto dto, CancellationToken cancellationToken = default);
    Task ConfirmPhoneNumberAsync(VerifyPhoneDto dto, CancellationToken cancellationToken = default);
}
