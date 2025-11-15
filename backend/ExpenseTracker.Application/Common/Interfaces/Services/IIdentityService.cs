using ExpenseTracker.Application.DTOs.User;

namespace ExpenseTracker.Application.Common.Interfaces.Services;

public interface IIdentityService
{
    Task<AuthResultDto> RegisterUserAsync(RegisterUserDto dto, IEnumerable<string> roles, CancellationToken cancellationToken = default);
    Task<AuthResultDto> LoginAsync(LoginUserDto dto, CancellationToken cancellationToken = default);
    Task LogoutAsync(LogoutUserDto dto,CancellationToken cancellationToken = default);
    Task<AuthResultDto> RefreshTokenAsync(RefreshTokenDto dto, CancellationToken cancellationToken = default);
}