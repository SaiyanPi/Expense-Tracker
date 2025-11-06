using AutoMapper;
using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Application.DTOs.User;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Identity;

namespace ExpenseTracker.Persistence.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtTokenService jwtTokenService,
        IUserRepository userRepository,
        IMapper mapper)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtTokenService = jwtTokenService;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<AuthResultDto> RegisterUserAsync(RegisterUserDto dto, CancellationToken cancellationToken = default)
    {
        var domainUser = _mapper.Map<User>(dto);
        var created = await _userRepository.RegisterAsync(domainUser, dto.Password, cancellationToken);
        if (!created)
            throw new IdentityOperationException("User registration failed. {errors}");

        // get roles
        var roles = await _userRepository.GetRolesAsync(dto.Email);
        // generate new access token
        var (token, expiresAt) = _jwtTokenService.GenerateToken(domainUser, roles);
        // Generate new refresh token
        var (refreshToken, refreshExpires) = _jwtTokenService.GenerateRefreshToken();
        // Save refresh token in DB
        await _userRepository.SetRefreshTokenAsync(dto.Email, refreshToken, refreshExpires);
        return new AuthResultDto
        {
            Success = true,
            Token = token,
            RefreshToken = refreshToken,
            ExpiresAt = expiresAt
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

        var result = await _signInManager.CheckPasswordSignInAsync(_mapper.Map<ApplicationUser>(domainUser), dto.Password, false);
        if (!result.Succeeded)
            throw new InvalidCredentialsException("Invalid credentials. {errors}");

        var roles = await _userRepository.GetRolesAsync(dto.Email);
        // generate new access token
        var (token, expiresAt) = _jwtTokenService.GenerateToken(domainUser, roles);
        // Generate new refresh token
        var (refreshToken, refreshExpires) = _jwtTokenService.GenerateRefreshToken();
        // Save refresh token in DB
        await _userRepository.SetRefreshTokenAsync(dto.Email, refreshToken, refreshExpires);
        return new AuthResultDto
        {
            Success = true,
            Token = token,
            RefreshToken = refreshToken,
            ExpiresAt = expiresAt
        };
    }

    public async Task LogoutAsync(LogoutUserDto dto, CancellationToken cancellationToken = default)
    {
        var domainUser = await _userRepository.GetByEmailAsync(dto.Email);
        if (domainUser is null)
            throw new NotFoundException(nameof(User), dto.Email);

        _mapper.Map<ApplicationUser>(domainUser).RefreshToken = null;
        _mapper.Map<ApplicationUser>(domainUser).RefreshTokenExpiryTime = null;

        await _userManager.UpdateAsync(_mapper.Map<ApplicationUser>(domainUser));
        await _signInManager.SignOutAsync();
    }

    public async Task<AuthResultDto> RefreshTokenAsync(RefreshTokenDto dto, CancellationToken cancellationToken = default)
    {
        return await _jwtTokenService.RefreshTokenAsync(dto.Token, dto.RefreshToken);
    }
}