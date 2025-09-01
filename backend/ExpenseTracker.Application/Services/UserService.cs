using System.Security.Claims;
using AutoMapper;
using ExpenseTracker.Application.DTOs.User;
using ExpenseTracker.Application.Interfaces.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using ExpenseTrackler.Application.DTOs.User;

namespace ExpenseTracker.Application.Services;

// FOR USER MANAGEMENT USE CASE
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IMapper _mapper;
    public UserService(IUserRepository userRepository, IJwtTokenService jwtTokenService, IMapper mapper)
    {
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
        _mapper = mapper;
    }


    public async Task<IReadOnlyList<UserDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<UserDto>>(users);
    }

    public async Task<UserDto?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        return _mapper.Map<UserDto?>(user);
    }

    public async Task<UserDto?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);
        return _mapper.Map<UserDto?>(user);
    }

    public async Task<string> RegisterAsync(RegisterUserDto dto, CancellationToken cancellationToken = default)
    {
        var user = _mapper.Map<User>(dto);
        await _userRepository.RegisterAsync(user, dto.Password, cancellationToken);
        return user.Id;
    }

    public async Task UpdateAsync(string id, UpdateUserDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (user is null)
        {
            throw new KeyNotFoundException($"User with id {id} not found");
        }

        _mapper.Map(dto, user);
        await _userRepository.UpdateAsync(user, cancellationToken);
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (user is null)
        {
            throw new KeyNotFoundException($"User with id {id} not found");
        }

        await _userRepository.DeleteAsync(user, cancellationToken);
    }


    // --- Authentication ---

    public async Task<AuthResultDto> LoginAsync(LoginUserDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAsync(dto.Email, cancellationToken);
        if (user == null || !await _userRepository.CheckPasswordAsync(dto.Email, dto.Password))
        {
            return new AuthResultDto
            {
                Success = false,
                Errors = new[] { "Invalid email or password" }
            };
        }

        var roles = await _userRepository.GetRolesAsync(dto.Email);

        // Generate new access token
        var (token, expiresAt) = _jwtTokenService.GenerateToken(user, roles);

        // Generate new refresh token
        var (refreshToken, refreshExpires) = _jwtTokenService.GenerateRefreshToken();

        // Save refresh token in DB
        await _userRepository.SetRefreshTokenAsync(dto.Email, refreshToken, refreshExpires);

        return new AuthResultDto
        {
            Success = true,
            Token = token,
            ExpiresAt = expiresAt,
            RefreshToken = refreshToken
        };
    }

    public async Task<AuthResultDto> RefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        // parse the token
        var principal = _jwtTokenService.GetPrincipalFromExpiredToken(token);
        if (principal == null) throw new InvalidOperationException("Invalid token.");

        // extracting email claim from the token
        var email = principal.FindFirst(ClaimTypes.Email)?.Value;
        if (email == null) throw new InvalidOperationException("Invalid token.");

        // fetch the user with email from db
        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);
        if (user == null) throw new KeyNotFoundException("User not found.");

        // fetch the stored refresh token and its expiry time from the db
        var storedRefresh = await _userRepository.GetRefreshTokenAsync(email);
        if (storedRefresh.refreshToken != refreshToken || storedRefresh.expiryTime < DateTime.UtcNow)
            throw new InvalidOperationException("Invalid refresh token.");

        // Generate new tokens
        var roles = await _userRepository.GetRolesAsync(email);
        var (newToken, expiresAt) = _jwtTokenService.GenerateToken(user, roles);
        var (newRefresh, newRefreshExpires) = _jwtTokenService.GenerateRefreshToken();

        // Save new refresh token
        await _userRepository.SetRefreshTokenAsync(email, newRefresh, newRefreshExpires);

        // Return both tokens to client
        return new AuthResultDto
        {
            Success = true,
            Token = newToken,
            ExpiresAt = expiresAt,
            RefreshToken = newRefresh
        };
    }

    public async Task LogoutAsync(string email, CancellationToken cancellationToken = default)
    {
        await _userRepository.SetRefreshTokenAsync(email, null!, DateTime.MinValue);
    }
}   
