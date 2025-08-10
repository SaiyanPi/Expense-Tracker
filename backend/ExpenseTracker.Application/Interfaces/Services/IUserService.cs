using ExpenseTrackler.Application.DTOs.User;

namespace ExpenseTracker.Application.Interfaces.Services;

public interface IUserService
{
    Task<IReadOnlyList<UserDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<UserDto?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<UserDto?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<string> RegisterAsync(RegisterUserDto dto, CancellationToken cancellationToken = default);
    Task UpdateAsync(string id, UpdateUserDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
}
