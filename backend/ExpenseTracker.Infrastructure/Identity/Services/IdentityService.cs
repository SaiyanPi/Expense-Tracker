using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Application.DTOs.User;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using ExpenseTracker.Persistence.Identity;
using Microsoft.AspNetCore.Identity;

namespace ExpenseTracker.Infrastructure.Identity.Services;

public class IdentityService : IIdentityService
{
    //private readonly IJwtTokenService _jwtTokenService;
    private readonly IIdentityRepository _identityRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public IdentityService(
        IIdentityRepository identityRepository,
        IUserRepository userRepository,
        IMapper mapper)
    {
        _identityRepository = identityRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<AuthResultDto> RegisterUserAsync(RegisterUserDto dto, string role, CancellationToken cancellationToken = default)
    {
        // check if user with email already exists
        if (await _userRepository.GetByEmailAsync(dto.Email, cancellationToken) != null)
            throw new ConflictException($"User with email {dto.Email} already exists.");
            
        var domainUser = _mapper.Map<User>(dto);
        var (Succeeded, UserId, Errors) = await _identityRepository.RegisterAsync(domainUser, dto.Password, role, cancellationToken);
        if (!Succeeded)
            throw new IdentityOperationException("User registration failed.");

        var user = await _userRepository.GetByEmailAsync(dto.Email, cancellationToken)
            ?? throw new Exception("User not found after registration.");

        var accessToken = await _identityRepository.GenerateJwtTokenAsync(user);
        var refreshToken = await _identityRepository.GenerateRefreshTokenAsync(user);
        return new AuthResultDto
        {
            Success = true,
            Token = accessToken,
            RefreshToken = refreshToken
        };
    }

    public async Task<AuthResultDto> LoginAsync(LoginUserDto dto, CancellationToken cancellationToken = default)
    {
        var domainUser = await _userRepository.GetByEmailAsync(dto.Email);
        if (domainUser is null)
            throw new UnauthorizedAccessException("Invalid credentials.");
        
        // load the ApplicationUser instance for ASP.NET Identity operations
        // var appUser = await _userManager.FindByEmailAsync(dto.Email);
        // if (appUser is null)
        //     throw new UnauthorizedAccessException("Invalid credentials. {errors}");

        var valid = await _identityRepository.CheckPasswordAsync(dto.Email, dto.Password, cancellationToken);
        if (!valid)
            throw new InvalidCredentialsException("Invalid credentials. {errors}");

        var accessToken = await _identityRepository.GenerateJwtTokenAsync(domainUser);
        var refreshToken = await _identityRepository.GenerateRefreshTokenAsync(domainUser);

        // store refresh token
        await _identityRepository.StoreRefreshTokenAsync(domainUser.Id, refreshToken, cancellationToken);

        return new AuthResultDto
        {
            Success = true,
            Token = accessToken,
            RefreshToken = refreshToken
        };
    }

    public async Task LogoutAsync(LogoutUserDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAsync(dto.Email, cancellationToken);
        if( user is null)
            throw new NotFoundException(nameof(User), dto.Email);

        var userId = user.Id;

        var storedRefresh = await _identityRepository.GetRefreshTokenAsync(userId, cancellationToken);
        if (storedRefresh == null)
        {
            throw new IdentityOperationException("Logout failed. No refresh token found.");
        }
        
        var revoked = await _identityRepository.RevokeRefreshTokenAsync(userId, storedRefresh, cancellationToken);
        if (!revoked)
        {
            throw new IdentityOperationException("No active session, User already logged out.");
        }
    }

    public async Task<AuthResultDto> RefreshTokenAsync( RefreshTokenDto dto,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dto.Token))
            throw new Application.Common.Exceptions.InvalidOperationException("Access token must be provided.");

        if (string.IsNullOrWhiteSpace(dto.RefreshToken))
            throw new Application.Common.Exceptions.InvalidOperationException("Refresh token must be provided.");

        // ---- STEP 1: Decode access token safely ----
        var handler = new JwtSecurityTokenHandler();
        JwtSecurityToken jwtToken;

        try
        {
            jwtToken = handler.ReadJwtToken(dto.Token);
        }
        catch (Exception)
        {
            throw new Application.Common.Exceptions.InvalidOperationException("Invalid access token format.");
        }

        // ---- STEP 2: Extract email claim ----
        // Use standard JWT claim name
        var email = jwtToken.Claims.FirstOrDefault(c =>
            c.Type == JwtRegisteredClaimNames.Email ||
            c.Type == "email" ||
            c.Type == ClaimTypes.Email
        )?.Value;

        if (string.IsNullOrWhiteSpace(email))
            throw new Application.Common.Exceptions.InvalidOperationException("Email claim not found in access token.");

        Console.WriteLine("EMAIL FROM TOKEN: " + email);

        // ---- STEP 3: Retrieve user by email ----
        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);
        if (user == null)
            throw new NotFoundException(nameof(User), email);

        // ---- STEP 4: Validate refresh token ----
        var isValid = await _identityRepository.ValidateRefreshTokenAsync(
            user.Id, dto.RefreshToken, cancellationToken);

        if (!isValid)
            throw new IdentityOperationException("Invalid or expired refresh token.");

        // ---- STEP 5: Generate new tokens ----
        var newAccessToken = await _identityRepository.GenerateJwtTokenAsync(user, cancellationToken);
        var newRefreshToken = await _identityRepository.GenerateRefreshTokenAsync(user, cancellationToken);

        // ---- STEP 6: Store new refresh token ----
        var stored = await _identityRepository.StoreRefreshTokenAsync(user.Id, newRefreshToken, cancellationToken);
        if (!stored)
            throw new IdentityOperationException("Unable to store new refresh token.");

        // ---- STEP 7: Return AuthResultDto ----
        return new AuthResultDto
        {
            Success = true,
            Token = newAccessToken,
            RefreshToken = newRefreshToken
        };
    }


    // Change Password
    //-------------------
    public async Task ChangePasswordAsync(ChangePasswordDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAsync(dto.Email, cancellationToken);
        if (user == null)
            throw new NotFoundException(nameof(User), dto.Email);
        
        // check if the current password is correct
        var isCurrentPasswordValid = await _identityRepository.CheckPasswordAsync(dto.Email, dto.CurrentPassword, cancellationToken);
        if (!isCurrentPasswordValid)
            throw new IdentityOperationException("Your Current password is incorrect.");

        var changed = await _identityRepository.ChangePasswordAsync(user.Id, dto.CurrentPassword, dto.NewPassword, cancellationToken);
        if (!changed)
            throw new IdentityOperationException("Password change failed. Current password may be incorrect.");
    }

}