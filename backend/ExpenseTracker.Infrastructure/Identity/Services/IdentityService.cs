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
    private readonly SignInManager<ApplicationUser> _signInManager;
    //private readonly IJwtTokenService _jwtTokenService;
    private readonly IIdentityRepository _identityRepository;
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IMapper _mapper;

    public IdentityService(
        SignInManager<ApplicationUser> signInManager,
        IIdentityRepository identityRepository,
        IUserRepository userRepository,
        IJwtTokenService jwtTokenService,
        IMapper mapper)
    {
        _signInManager = signInManager;
        _identityRepository = identityRepository;
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
        _mapper = mapper;
    }

    public async Task<AuthResultDto> RegisterUserAsync(RegisterUserDto dto, IEnumerable<string> roles, CancellationToken cancellationToken = default)
    {
        // check if user with email already exists
        if (await _userRepository.GetByEmailAsync(dto.Email, cancellationToken) != null)
            throw new ConflictException($"User with email {dto.Email} already exists.");
            
        var domainUser = _mapper.Map<User>(dto);
        var (Succeeded, UserId, Errors) = await _identityRepository.RegisterAsync(domainUser, dto.Password, roles, cancellationToken);
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

        var storedRefresh = await _userRepository.GetRefreshTokenAsync(dto.Email);
        if (storedRefresh.refreshToken == null)
        {
            throw new IdentityOperationException("Logout failed. No refresh token found.");
        }
        
        var revoked = await _identityRepository.RevokeRefreshTokenAsync(userId, storedRefresh.refreshToken, cancellationToken);
        if (!revoked)
        {
            throw new IdentityOperationException("Logout failed. Unable to revoke refresh token.");
        }
    }

    public async Task<AuthResultDto> RefreshTokenAsync(RefreshTokenDto dto, CancellationToken cancellationToken = default)
    {
        // parse the token
        var principal = _jwtTokenService.GetPrincipalFromExpiredToken(dto.Token);
        if (principal is null)
            throw new Application.Common.Exceptions.InvalidOperationException("Invalid token.");

        // extracting email claim from the token
        var email = principal.FindFirst(ClaimTypes.Email)?.Value;
        if (email is null)
            throw new Application.Common.Exceptions.InvalidOperationException("Invalid token.");

        // fetch the user with email from db
        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);
        if (user is null)
            throw new NotFoundException(nameof(User), email);
        
        var isValid = await _identityRepository.ValidateRefreshTokenAsync(user.Id, dto.RefreshToken, cancellationToken);
        if (!isValid)
            throw new IdentityOperationException("Invalid or expired refresh token."); 
        
         // Generate new tokens
        var accessToken = await _identityRepository.GenerateJwtTokenAsync(user, cancellationToken);
        var newRefreshToken = await _identityRepository.GenerateRefreshTokenAsync(user, cancellationToken);

        var stored = await _identityRepository.StoreRefreshTokenAsync(user.Id, newRefreshToken, cancellationToken);
        if (!stored)
            throw new IdentityOperationException("Unable to store new refresh token.");

        return new AuthResultDto
        {
            Success = true,
            Token = accessToken,
            RefreshToken = newRefreshToken
        };
    }
}