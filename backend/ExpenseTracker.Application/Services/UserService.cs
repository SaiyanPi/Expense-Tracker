using System.Security.Claims;
using AutoMapper;
using ExpenseTracker.Application.DTOs.User;
using ExpenseTracker.Application.Exceptions;
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

    // public async Task<string> RegisterAsync(RegisterUserDto dto, CancellationToken cancellationToken = default)
    // {
    //     if (await _userRepository.GetByEmailAsync(dto.Email, cancellationToken) != null)
    //         throw new ConflictException($"User with email {dto.Email} already exists.");

    //     var user = _mapper.Map<User>(dto);
    //     var registered = await _userRepository.RegisterAsync(user, dto.Password, cancellationToken);

    //     if (!registered)
    //         throw new IdentityOperationException("User registration failed: {errors}");
       
    //     return user.Id;
    // }

    public async Task UpdateAsync(string id, UpdateUserDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (user is null)
            throw new NotFoundException(nameof(User), id);

        _mapper.Map(dto, user);
        var updated = await _userRepository.UpdateAsync(user, cancellationToken);
        if (!updated)
            throw new IdentityOperationException("User update failed: {errors}");
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (user is null)
            throw new NotFoundException(nameof(User), id);
  
        // var deleted = await _userRepository.DeleteAsync(user, cancellationToken);
        // if (!deleted)
        try
        {
            await _userRepository.DeleteAsync(user, cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.InnerException?.Message);
            throw;
        }
    }


    // --- Authentication ---

    // public async Task<AuthResultDto> LoginAsync(LoginUserDto dto, CancellationToken cancellationToken = default)
    // {       
    //     var user = await _userRepository.GetByEmailAsync(dto.Email, cancellationToken);
    //     if (user == null || !await _userRepository.CheckPasswordAsync(dto.Email, dto.Password))
    //     throw new InvalidCredentialsException();

    //     var roles = await _userRepository.GetRolesAsync(dto.Email);

    //     // Generate new access token
    //     var (token, expiresAt) = _jwtTokenService.GenerateToken(user, roles);

    //     // Generate new refresh token
    //     var (refreshToken, refreshExpires) = _jwtTokenService.GenerateRefreshToken();

    //     // Save refresh token in DB
    //     await _userRepository.SetRefreshTokenAsync(dto.Email, refreshToken, refreshExpires);

    //     return new AuthResultDto
    //     {
    //         Success = true,
    //         Token = token,
    //         ExpiresAt = expiresAt,
    //         RefreshToken = refreshToken
    //     };
    // }

    // public async Task<AuthResultDto> RefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
    // {
    //     // parse the token
    //     var principal = _jwtTokenService.GetPrincipalFromExpiredToken(token);
    //     if (principal == null) throw new InvalidOperationException("Invalid token.");

    //     // extracting email claim from the token
    //     var email = principal.FindFirst(ClaimTypes.Email)?.Value;
    //     if (email == null) throw new InvalidOperationException("Invalid token.");

    //     // fetch the user with email from db
    //     var user = await _userRepository.GetByEmailAsync(email, cancellationToken);
    //     if (user == null) throw new KeyNotFoundException("User not found.");

    //     // fetch the stored refresh token and its expiry time from the db
    //     var storedRefresh = await _userRepository.GetRefreshTokenAsync(email);
    //     if (storedRefresh.refreshToken != refreshToken || storedRefresh.expiryTime < DateTime.UtcNow)
    //         throw new InvalidOperationException("Invalid refresh token.");

    //     // Generate new tokens
    //     var roles = await _userRepository.GetRolesAsync(email);
    //     var (newToken, expiresAt) = _jwtTokenService.GenerateToken(user, roles);
    //     var (newRefresh, newRefreshExpires) = _jwtTokenService.GenerateRefreshToken();

    //     // Save new refresh token
    //     await _userRepository.SetRefreshTokenAsync(email, newRefresh, newRefreshExpires);

    //     // Return both tokens to client
    //     return new AuthResultDto
    //     {
    //         Success = true,
    //         Token = newToken,
    //         ExpiresAt = expiresAt,
    //         RefreshToken = newRefresh
    //     };
    // }

    // public async Task LogoutAsync(string email, CancellationToken cancellationToken = default)
    // {
    //     await _userRepository.SetRefreshTokenAsync(email, null!, DateTime.MinValue);
    // }
}   
