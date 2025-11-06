using ExpenseTracker.Application.DTOs.User;

namespace ExpenseTracker.Application.Common.Interfaces.Services;

public interface IUserService
{
    // CRUD
    Task<IReadOnlyList<UserDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<UserDto?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<UserDto?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    //Task<string> RegisterAsync(RegisterUserDto dto, CancellationToken cancellationToken = default);
    Task UpdateAsync(string id, UpdateUserDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);

    // Authentication
    //Task<AuthResultDto> LoginAsync(LoginUserDto dto, CancellationToken cancellationToken = default);
    //Task<AuthResultDto> RefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);
    //Task LogoutAsync(string userId, CancellationToken cancellationToken = default);
}
