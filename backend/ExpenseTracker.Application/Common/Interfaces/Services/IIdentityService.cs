using ExpenseTracker.Application.DTOs.Auth;

namespace ExpenseTracker.Application.Common.Interfaces.Services;

public interface IIdentityService
{
    Task<AuthResultDto> RegisterUserAsync(RegisterUserDto dto, string role, CancellationToken cancellationToken = default);
    Task<AuthResultDto> LoginAsync(LoginUserDto dto);
    Task UpdateAsync(string userId, UpdateUserDto dto);
    Task DeleteAsync(string userId);
    // GetAll, GetById, GetByEmail methods are not considered identity operation therefore they
    // resides inside user repository
    Task LogoutAsync(string userId);
    Task<AuthResultDto> RefreshTokenAsync(RefreshTokenDto dto, CancellationToken cancellationToken = default);
    Task ChangePasswordAsync(string userId, ChangePasswordDto dto);
    

    // email confirmation
    Task RequestEmailConfirmationTokenAsync(RequestEmailConfirmationDto dto);
    Task ConfirmEmailAsync(VerifyEmailDto dto);

    // password reset
    Task ForgotPasswordResetTokenAsync(ForgotPasswordDto dto, CancellationToken cancellationToken = default);
    Task ResetPasswordAsync(string userId, string token, ResetPasswordDto dto);
    
    // change email
    Task RequestChangeEmailAsync(string userId, ChangeEmailRequestDto dto, CancellationToken cancellationToken = default);
    Task ConfirmChangeEmailAsync(ConfirmChangeEmailDto dto);
    
    // phone confirmation
    Task GeneratePhoneConfirmationTokenAsync(PhoneConfirmationDto dto, CancellationToken cancellationToken = default);
    Task ConfirmPhoneNumberAsync(VerifyPhoneDto dto);
}
