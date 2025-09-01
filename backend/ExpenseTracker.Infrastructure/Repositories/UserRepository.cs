using AutoMapper;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using ExpenseTracker.Persistence;
using ExpenseTracker.Persistence.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;
    public UserRepository(UserManager<ApplicationUser> userManager, IMapper mapper)
    {
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var appUsers = await _userManager.Users.ToListAsync();
        return _mapper.Map<IReadOnlyList<User>>(appUsers);
    }

    public async Task<User?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var appUser = await _userManager.FindByIdAsync(id);
        return appUser is null ? null : _mapper.Map<User>(appUser);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var appUser = await _userManager.FindByEmailAsync(email);
        return appUser is null ? null : _mapper.Map<User>(appUser);
    }

    public async Task RegisterAsync(User user, string password, CancellationToken cancellationToken = default)
    {
        var appUser = _mapper.Map<ApplicationUser>(user);
        var result = await _userManager.CreateAsync(appUser, password);

        if (!result.Succeeded)
        {
            throw new InvalidOperationException($"Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        var appUser = await _userManager.FindByIdAsync(user.Id);
        if (appUser is null) return;

        _mapper.Map(user, appUser);
        var result = await _userManager.UpdateAsync(appUser);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException($"Failed to update user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
    }

    public async Task DeleteAsync(User user, CancellationToken cancellationToken = default)
    {
        var appUser = await _userManager.FindByIdAsync(user.Id);
        if (appUser is null) return;

        var result = await _userManager.DeleteAsync(appUser);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException($"Failed to delete user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
    }

    // --- Authentication ---
    public async Task<bool> CheckPasswordAsync(string email, string password)
    {
        var appUser = await _userManager.FindByEmailAsync(email);
        if (appUser == null) return false;

        return await _userManager.CheckPasswordAsync(appUser, password);
    }

    public async Task<IList<string>> GetRolesAsync(string email)
    {
        var appUser = await _userManager.FindByEmailAsync(email);
        if (appUser == null) return new List<string>();

        return await _userManager.GetRolesAsync(appUser);
    }
    
    // --- Refresh Tokens ---
    public async Task SetRefreshTokenAsync(string email, string refreshToken, DateTime expiryTime)
    {
        var appUser = await _userManager.FindByEmailAsync(email);
        if (appUser == null) return;
        appUser.RefreshToken = refreshToken;
        appUser.RefreshTokenExpiryTime = expiryTime;
        await _userManager.UpdateAsync(appUser);
    }

    public async Task<(string? refreshToken, DateTime? expiryTime)> GetRefreshTokenAsync(string email)
    {
        var appUser = await _userManager.FindByEmailAsync(email);
        return (appUser?.RefreshToken, appUser?.RefreshTokenExpiryTime);
    }

}

// This is the implementation of repositopry interface in domain layer