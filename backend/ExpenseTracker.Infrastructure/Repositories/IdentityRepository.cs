using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using ExpenseTracker.Application.Common.Authorization;
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



    // User Registration
    //---------------------------
    public async Task<(bool Succeeded, IEnumerable<string>? Errors, User? User)> RegisterAsync
        (User user, string password, string role, CancellationToken cancellationToken = default)
    {
        var appUser = _mapper.Map<ApplicationUser>(user);

        var result = await _userManager.CreateAsync(appUser, password);
        if (!result.Succeeded)
            return (false, result.Errors.Select(e => e.Description), null);

        await _userManager.AddToRoleAsync(appUser, role ?? "User");
      

        // Convert ApplicationUser → Domain User
        var domainUser = new User
        {
            Id = appUser.Id,
            FullName = appUser.FullName,
            Email = appUser.Email!,
        };

        return (true, null, domainUser);
    }


    // Check Password
    //---------------------------
    public async Task<bool> CheckPasswordAsync(string email, string password, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) return false;

        return await _userManager.CheckPasswordAsync(user, password);
    }



    // JWT Generation
    //---------------------------
    public async Task<string> GenerateJwtTokenAsync(User user, CancellationToken cancellationToken)
    {
        var key = Encoding.UTF8.GetBytes(_config["JwtConfig:Secret"]!);

        // Fetch user from UserManager
        var appUser = await _userManager.FindByIdAsync(user.Id);
        if (appUser == null) 
            throw new Exception("User not found.");

        // Build claims 
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.FullName),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? "")
        };

        // Add roles
        var roles = await _userManager.GetRolesAsync(appUser);
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        // Add role based claims
        foreach (var role in roles)
        {
            switch (role)
            {
                case AppRoles.Admin:
                    claims.Add(new Claim(AppClaimTypes.can_view_all_users, "true"));
                    claims.Add(new Claim(AppClaimTypes.can_view_all_users_category, "true"));
                    claims.Add(new Claim(AppClaimTypes.can_view_all_users_expense, "true"));
                    break;
                case "User":
                    claims.Add(new Claim("CanViewOwnCategory", "true"));
                    break;
            }
        }

        // Signing
        var signingKey = new SymmetricSecurityKey(key);
        var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["JwtConfig:Issuer"],
            audience: _config["JwtConfig:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds   // <-- Use the SAME credentials object
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

 


    // Refresh Token Generation
    //---------------------------
    public Task<string> GenerateRefreshTokenAsync(User user, CancellationToken cancellationToken)
    {
        var refresh = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        return Task.FromResult(refresh);
    }



    // Store Refresh Token (Identity Tokens table)
    //--------------------------------------------
    public async Task<bool> StoreRefreshTokenAsync(string userId, string refreshToken, CancellationToken cancellationToken)
    {
        var appUser = await _userManager.FindByIdAsync(userId);
        if (appUser == null) return false;

        await _userManager.SetAuthenticationTokenAsync(appUser, "ExpenseTracker", "RefreshToken", refreshToken);

        return true;
    }



    // Validate Refresh Token
    //---------------------------
    public async Task<bool> ValidateRefreshTokenAsync(string userId, string refreshToken, CancellationToken cancellationToken)
    {
        var appUser = await _userManager.FindByIdAsync(userId);
        if (appUser == null) return false;

        var stored = await _userManager.GetAuthenticationTokenAsync(appUser, "ExpenseTracker", "RefreshToken");

        return stored == refreshToken;
    }



    // Revoke Refresh Token (Logout)
    //---------------------------------
    public async Task<bool> RevokeRefreshTokenAsync(string userId, string refreshToken, CancellationToken cancellationToken)
    {
        var appUser = await _userManager.FindByIdAsync(userId);
        if (appUser == null) return false;

        await _userManager.RemoveAuthenticationTokenAsync(appUser, "ExpenseTracker", "RefreshToken");

        return true;
    }



    // Get Refresh Token (for Logout)
    //---------------------------------
    public async Task<string?> GetRefreshTokenAsync(string userId, CancellationToken cancellationToken)
    {
        var appUser = await _userManager.FindByIdAsync(userId);
        if (appUser == null) return null;

        var stored = await _userManager.GetAuthenticationTokenAsync(appUser, "ExpenseTracker", "RefreshToken");

        return stored;
    }


    // Change Password
    //---------------------------
    public async Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword, CancellationToken cancellationToken)
    {
        var appUser = await _userManager.FindByIdAsync(userId);
        if (appUser == null) return false;

        var result = await _userManager.ChangePasswordAsync(appUser, currentPassword, newPassword);
        return result.Succeeded;
    }


    // generate email confirmation token
    //------------------------------------
    public async Task<string?> GenerateEmailConfirmationTokenAsync(string userId, CancellationToken cancellationToken)
    {
        var appUser = await _userManager.FindByIdAsync(userId);
        if (appUser == null) return null;

        return await _userManager.GenerateEmailConfirmationTokenAsync(appUser);
    }


    // confirm email
    //------------------------------------
    public async Task<bool> ConfirmEmailAsync(string userId, string token, CancellationToken cancellationToken)
    {
        var appUser = await _userManager.FindByIdAsync(userId);
        if (appUser == null) return false;

        var result = await _userManager.ConfirmEmailAsync(appUser, token);
        return result.Succeeded;
    }


    // generate password reset token
    //------------------------------------
    public async Task<string?> GeneratePasswordResetTokenAsync(string userId, CancellationToken cancellationToken)
    {
        var appUser = await _userManager.FindByIdAsync(userId);
        if (appUser == null) return null;

        return await _userManager.GeneratePasswordResetTokenAsync(appUser);
    }


    // reset password
    //------------------------------------
    public async Task<bool> ResetPasswordAsync(string userId, string token, string newPassword, CancellationToken cancellationToken)
    {
        var appUser = await _userManager.FindByIdAsync(userId);
        if (appUser == null) return false;

        // decode token because it is URL encoded
        var decodedToken = Uri.UnescapeDataString(token);

        var result = await _userManager.ResetPasswordAsync(appUser, decodedToken, newPassword);
        return result.Succeeded;
    }
}