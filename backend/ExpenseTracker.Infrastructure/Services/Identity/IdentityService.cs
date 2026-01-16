using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Application.DTOs.Auth;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using ExpenseTracker.Persistence.Identity;
using Microsoft.AspNetCore.Identity;

namespace ExpenseTracker.Infrastructure.Services.Identity;

public class IdentityService : IIdentityService
{
    //private readonly IJwtTokenService _jwtTokenService;
    private readonly IIdentityRepository _identityRepository;
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;
    private readonly ISmsSenderService _smsSenderService;

    private readonly IMapper _mapper;

    public IdentityService(
        IIdentityRepository identityRepository,
        IUserRepository userRepository,
        IEmailService emailService,
        ISmsSenderService smsSenderService,
        IMapper mapper)
    {
        _identityRepository = identityRepository;
        _userRepository = userRepository;
        _emailService = emailService;
        _smsSenderService = smsSenderService;
        _mapper = mapper;
    }

    public async Task<AuthResultDto> RegisterUserAsync(RegisterUserDto dto, string role, CancellationToken cancellationToken = default)
    {
        // check if user with email already exists
        if (await _userRepository.GetByEmailAsync(dto.Email, cancellationToken) != null)
            throw new ConflictException($"User with email {dto.Email} already exists.");
            
        var domainUser = _mapper.Map<User>(dto);

        // create user in identity
        var (Succeeded, UserId, Errors) = await _identityRepository.RegisterAsync(domainUser, dto.Password, role, cancellationToken);
        if (!Succeeded)
            throw new IdentityOperationException("User registration failed.");

        var user = await _userRepository.GetByEmailAsync(dto.Email, cancellationToken)
            ?? throw new Exception("User not found after registration.");
        try
        {
            // 1. automatically generate email confirmation token
            var emailConfirmationToken = await _identityRepository.GenerateEmailConfirmationTokenAsync(user.Id, cancellationToken);
            if (emailConfirmationToken == null)
                throw new IdentityOperationException("Failed to generate email confirmation token.");

            // 2. build confirmation link (to be sent via email)
            var confirmationLink = $"http://localhost:5167/api/auth/confirm-email?userId={user.Id}&token={Uri.EscapeDataString(emailConfirmationToken)}";

            // 3. send confirmation email
            await _emailService.SendEmailAsync(
                to: user.Email,
                subject: "Email Confirmation",
                body: $"Please confirm your email by clicking this link: {confirmationLink}", 
                cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            // rollback
            await _identityRepository.DeleteAsync(user, cancellationToken);
            throw new EmailSendingException("Failed to send email confirmation link. Did you forget to run the local email server?", ex);
        }
      
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
        var result = new AuthResultDto(); // start as success = false

        var domainUser = await _userRepository.GetByEmailAsync(dto.Email);
        if (domainUser is null)
        {
            result.Errors = new[] { "Invalid credentials." };
            return result;
        }

        var valid = await _identityRepository.CheckPasswordAsync(dto.Email, dto.Password, cancellationToken);
        if (!valid)
        {
            result.Errors = new[] { "Invalid credentials." };
            return result;
        }
        
        var accessToken = await _identityRepository.GenerateJwtTokenAsync(domainUser);
        var refreshToken = await _identityRepository.GenerateRefreshTokenAsync(domainUser);

        // store refresh token
        await _identityRepository.StoreRefreshTokenAsync(domainUser.Id, refreshToken, cancellationToken);

        // return new AuthResultDto
        // {
        //     Success = true,
        //     Token = accessToken,
        //     RefreshToken = refreshToken
        // };
        result.Success = true;
        result.Token = accessToken;
        result.RefreshToken = refreshToken;

        return result;
    }

    public async Task UpdateAsync(string userId, UpdateUserDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if(user is null)
            throw new NotFoundException(nameof(User), userId);
        
        _mapper.Map(dto, user);

        var updated = await _identityRepository.UpdateAsync(user, cancellationToken);
        if(!updated)
            throw new IdentityOperationException("User update failed.");
    }

    public async Task DeleteAsync( string userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if(user is null)
            throw new NotFoundException(nameof(User), userId);
        
        var delete = await _identityRepository.DeleteAsync(user, cancellationToken);
        if(!delete)
            throw new IdentityOperationException("Profile deletion failed.");
    }

    public async Task LogoutAsync(string userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if( user is null)
            throw new NotFoundException(nameof(User), userId);

        var storedRefresh = await _identityRepository.GetRefreshTokenAsync(user.Id, cancellationToken);
        if (storedRefresh == null)
            throw new IdentityOperationException("Logout failed. No refresh token found.");
        
        var revoked = await _identityRepository.RevokeRefreshTokenAsync(user.Id, storedRefresh, cancellationToken);
        if (!revoked)
            throw new IdentityOperationException("No active session, User already logged out.");
    }

    public async Task<AuthResultDto> RefreshTokenAsync(RefreshTokenDto dto,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dto.RefreshToken))
            throw new Application.Common.Exceptions.InvalidOperationException("Refresh token must be provided.");

         // ---- STEP 1: Find userId by refresh token
        var userId = await _userRepository.GetByRefreshTokenAsync(dto.RefreshToken, cancellationToken);
        if (userId == null)
            throw new UnauthorizedAccessException("Invalid refresh token.");

        // ---- STEP 2: Load ApplicationUser
        var appUser = await _userRepository.GetByIdAsync(userId);
        if (appUser == null)
             throw new NotFoundException(nameof(User), userId);

        // ---- STEP 3: Validate refresh token ----
        var isValid = await _identityRepository.ValidateRefreshTokenAsync(
            userId, dto.RefreshToken, cancellationToken);

        if (!isValid)
            throw new IdentityOperationException("Invalid or expired refresh token.");

        // ---- STEP 4: Generate new tokens ----
        var newAccessToken = await _identityRepository.GenerateJwtTokenAsync(appUser, cancellationToken);
        var newRefreshToken = await _identityRepository.GenerateRefreshTokenAsync(appUser, cancellationToken);
        
        // ---- STEP 5: Store new refresh token ----
        var stored = await _identityRepository.StoreRefreshTokenAsync(userId, newRefreshToken, cancellationToken);
        if (!stored)
            throw new IdentityOperationException("Unable to store new refresh token.");

        // ---- STEP 6: Return AuthResultDto ----
        return new AuthResultDto
        {
            Success = true,
            Token = newAccessToken,
            RefreshToken = newRefreshToken
        };
    }


    // Change Password
    //-------------------
    public async Task ChangePasswordAsync(string userId, ChangePasswordDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
            throw new NotFoundException(nameof(User), userId);
        
        // check if the current password is correct
        var isCurrentPasswordValid = await _identityRepository.CheckPasswordAsync(user.Email, dto.CurrentPassword, cancellationToken);
        if (!isCurrentPasswordValid)
            throw new IdentityOperationException("Your Current password is incorrect.");

        var changed = await _identityRepository.ChangePasswordAsync(user.Id, dto.CurrentPassword, dto.NewPassword, cancellationToken);
        if (!changed)
            throw new IdentityOperationException("Password change failed. Current password may be incorrect.");
    }

    // Email Confirmation
    //-----------------------
    public async Task RequestEmailConfirmationTokenAsync(RequestEmailConfirmationDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(dto.UserId, cancellationToken);
        if (user == null)
            throw new NotFoundException(nameof(User), dto.UserId);

        var token = await _identityRepository.GenerateEmailConfirmationTokenAsync(dto.UserId, cancellationToken);
        if (token == null)
            throw new IdentityOperationException("Failed to generate email confirmation token.");
    }

    public async Task ConfirmEmailAsync(VerifyEmailDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(dto.UserId, cancellationToken);
        if (user == null)
            throw new NotFoundException(nameof(User), dto.UserId);
        
        var confirm = await _identityRepository.ConfirmEmailAsync(dto.UserId, dto.Token, cancellationToken);
        if (!confirm)
            throw new IdentityOperationException("Email confirmation failed.");
    }

    // Password Reset
    //-----------------------
    public async Task ForgotPasswordResetTokenAsync(ForgotPasswordDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAsync(dto.UserEmail, cancellationToken);
        if (user == null)
            throw new NotFoundException(nameof(User), dto.UserEmail);

        var token = await _identityRepository.GeneratePasswordResetTokenAsync(user.Id, cancellationToken);
        if (token == null)
            throw new IdentityOperationException("Failed to generate password reset token.");

        // build reset link (to be sent via email)
        var resetLink = $"http://localhost:5167/api/auth/reset-password?userId={user.Id}&token={Uri.EscapeDataString(token)}";

        // send resetLink via email (implementation omitted)
        await _emailService.SendEmailAsync(
            to: user.Email,
            subject: "Password Reset",
            body: $"You can reset your password by clicking this link: {resetLink}",
            cancellationToken: cancellationToken);

    }

    public async Task ResetPasswordAsync(string userId, string token, ResetPasswordDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
            throw new NotFoundException(nameof(User), userId);

        var reset = await _identityRepository.ResetPasswordAsync(userId, token, dto.NewPassword, cancellationToken);
        if (!reset)
            throw new IdentityOperationException("Password reset failed.");
    }

    // Change email
    // ---------------------
    public async Task RequestChangeEmailAsync(string userId, ChangeEmailRequestDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
            throw new NotFoundException(nameof(User), userId);
        
        var emailTaken = await _identityRepository.IsEmailTakenAsync(dto.NewEmail);
        if(emailTaken is true)
            throw new ConflictException($"User with email {dto.NewEmail} already exists.");

        var token = await _identityRepository.GenerateChangeEmailTokenAsync(user.Id, dto.NewEmail, cancellationToken);
        if (token == null)
            throw new IdentityOperationException("Failed to generate change email token.");

        var confirmationLink = $"http://localhost:5167/api/auth/confirm-change-email?userId={user.Id}&newEmail={dto.NewEmail}&token={Uri.EscapeDataString(token)}";

        await _emailService.SendEmailAsync(
            to: dto.NewEmail,
            subject: "Change Email",
            body: $"Hello {user.FullName}, please confirm your email change by clicking this link: {confirmationLink}",
            cancellationToken: cancellationToken);
    }

    public async Task ConfirmChangeEmailAsync(ConfirmChangeEmailDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(dto.UserId, cancellationToken);
        if (user == null)
            throw new NotFoundException(nameof(User), dto.UserId);

        var changed = await _identityRepository.ChangeEmailAsync(dto.UserId, dto.NewEmail, dto.Token, cancellationToken);
        if (!changed)
            throw new IdentityOperationException("Change email confirmation failed.");
    }

    // phone confirmation
    // --------------------
    public async Task GeneratePhoneConfirmationTokenAsync(PhoneConfirmationDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAsync(dto.UserEmail, cancellationToken);
        if (user == null)
            throw new NotFoundException(nameof(User), dto.UserEmail);

        var token = await _identityRepository.GeneratePhoneConfirmationTokenAsync(user.Id, dto.PhoneNumber);
        await _smsSenderService.SendOtpAsync(dto.PhoneNumber, $"Your confirmation code is: {token}", cancellationToken);
    }

    public async Task ConfirmPhoneNumberAsync(VerifyPhoneDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAsync(dto.UserEmail, cancellationToken);
        if (user == null)
            throw new NotFoundException(nameof(User), dto.UserEmail);

        var success = await _identityRepository.ConfirmPhoneNumberAsync(user.Id, dto.PhoneNumber, dto.Token);
        if (!success)
            throw new Application.Common.Exceptions.InvalidOperationException("Invalid phone confirmation code.");
    }

}
