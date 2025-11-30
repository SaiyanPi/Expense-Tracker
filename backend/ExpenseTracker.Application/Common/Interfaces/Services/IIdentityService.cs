using ExpenseTracker.Application.DTOs.Auth;

namespace ExpenseTracker.Application.Common.Interfaces.Services;

public interface IIdentityService
{
    Task<AuthResultDto> RegisterUserAsync(RegisterUserDto dto, string role, CancellationToken cancellationToken = default);
    Task<AuthResultDto> LoginAsync(LoginUserDto dto, CancellationToken cancellationToken = default);
    Task LogoutAsync(LogoutUserDto dto,CancellationToken cancellationToken = default);
    Task<AuthResultDto> RefreshTokenAsync(RefreshTokenDto dto, CancellationToken cancellationToken = default);
    Task ChangePasswordAsync(ChangePasswordDto dto, CancellationToken cancellationToken = default);

    // email confirmation
    Task RequestEmailConfirmationTokenAsync(RequestEmailConfirmationDto dto, CancellationToken cancellationToken = default);
    Task ConfirmEmailAsync(VerifyEmailDto dto, CancellationToken cancellationToken = default);

    // password reset
    Task ForgotPasswordResetTokenAsync(ForgotPasswordDto dto, CancellationToken cancellationToken = default);
    Task ResetPasswordAsync(ResetPasswordDto dto, CancellationToken cancellationToken = default);
}