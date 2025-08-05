using ExpenseTrackler.Application.DTOs.User;

namespace ExpenseTracker.Application.Interfaces.Services;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllAsync();
    Task<UserDto?> GetByIdAsync(string id);
    Task<Guid> CreateAsync(RegisterUserDto dto);
    Task UpdateAsync(UpdateUserDto dto);
    Task DeleteAsync(string id);
}
