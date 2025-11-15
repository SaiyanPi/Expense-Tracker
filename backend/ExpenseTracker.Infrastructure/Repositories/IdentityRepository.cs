using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using ExpenseTracker.Application.DTOs.User;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using ExpenseTracker.Persistence.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ExpenseTracker.Infrastructure.Repositories;
public class IdentityRepository : IIdentityRepository
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;
    private readonly IConfiguration _config;

    public IdentityRepository(
        UserManager<ApplicationUser> userManager,
        IMapper mapper,
        IConfiguration config)
    {
        _userManager = userManager;
        _mapper = mapper;
        _config = config;
    }

    // ------------------------------
    // User Registration
    // ------------------------------
    public async Task<(bool Succeeded, IEnumerable<string>? Errors, User? User)> RegisterAsync
        (User user, string password, IEnumerable<string> roles, CancellationToken cancellationToken = default)
    {
        var appUser = _mapper.Map<ApplicationUser>(user);

        var result = await _userManager.CreateAsync(appUser, password);
        if (!result.Succeeded)
            return (false, result.Errors.Select(e => e.Description), null);

        foreach (var role in roles)
        {
            await _userManager.AddToRoleAsync(appUser, role ?? "User");
        }
        // Convert ApplicationUser → Domain User
        var domainUser = new User
        {
            Id = appUser.Id,
            FullName = appUser.FullName,
            Email = appUser.Email!,
        };

        return (true, null, domainUser);
    }


    // ------------------------------
    // Check Password
    // ------------------------------
    public async Task<bool> CheckPasswordAsync(string email, string password, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) return false;

        return await _userManager.CheckPasswordAsync(user, password);
    }

    // ------------------------------
    // JWT Generation
    // ------------------------------
    public async Task<string> GenerateJwtTokenAsync(User user, CancellationToken cancellationToken)
    {
        var key = Encoding.UTF8.GetBytes(_config["JwtConfig:Secret"]!);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName)
        };

        //var appUser = _mapper.Map<ApplicationUser>(user);
        var appUser = await _userManager.FindByIdAsync(user.Id);
        if (appUser == null) 
            throw new Exception("User not found.");
            
        var userRoles = await _userManager.GetRolesAsync(appUser);

        // Console.WriteLine("ROLES: " + string.Join(",", userRoles));

        claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));
        
        var signingKey = new SymmetricSecurityKey(key);
        var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
        var expiresAt = DateTime.UtcNow.AddHours(2);
        
        var token = new JwtSecurityToken(
            issuer: _config["JwtConfig:Issuer"],
            audience: _config["JwtConfig:Audience"],
            expires: expiresAt,
            claims: claims,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // ------------------------------
    // Refresh Token Generation
    // ------------------------------
    public Task<string> GenerateRefreshTokenAsync(User user, CancellationToken cancellationToken)
    {
        var refresh = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        return Task.FromResult(refresh);
    }

    // ------------------------------
    // Store Refresh Token (Identity Tokens table)
    // ------------------------------
    public async Task<bool> StoreRefreshTokenAsync(string userId, string refreshToken, CancellationToken cancellationToken)
    {
        var appUser = await _userManager.FindByIdAsync(userId);
        if (appUser == null) return false;

        await _userManager.SetAuthenticationTokenAsync(appUser, "ExpenseTracker", "RefreshToken", refreshToken);

        return true;
    }

    // ------------------------------
    // Validate Refresh Token
    // ------------------------------
    public async Task<bool> ValidateRefreshTokenAsync(string userId, string refreshToken, CancellationToken cancellationToken)
    {
        var appUser = await _userManager.FindByIdAsync(userId);
        if (appUser == null) return false;

        var stored = await _userManager.GetAuthenticationTokenAsync(appUser, "ExpenseTracker", "RefreshToken");

        return stored == refreshToken;
    }

    // ------------------------------
    // Revoke Refresh Token (Logout)
    // ------------------------------
    public async Task<bool> RevokeRefreshTokenAsync(string userId, string refreshToken, CancellationToken cancellationToken)
    {
        var appUser = await _userManager.FindByIdAsync(userId);
        if (appUser == null) return false;

        await _userManager.RemoveAuthenticationTokenAsync(appUser, "ExpenseTracker", "RefreshToken");

        return true;
    }

    
}