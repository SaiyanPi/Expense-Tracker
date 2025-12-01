using ExpenseTracker.Application.Common.Authorization;
using ExpenseTracker.Application.DTOs.Auth;
using ExpenseTracker.Application.Features.Identity.Commands.ChangePassword;
using ExpenseTracker.Application.Features.Identity.Commands.EmailConfirmation;
using ExpenseTracker.Application.Features.Identity.Commands.ForgotPassword;
using ExpenseTracker.Application.Features.Identity.Commands.Login;
using ExpenseTracker.Application.Features.Identity.Commands.Logout;
using ExpenseTracker.Application.Features.Identity.Commands.RefreshToken;
using ExpenseTracker.Application.Features.Identity.Commands.Register;
using ExpenseTracker.Application.Features.Identity.Commands.ResetPassword;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
// FOR AUTHENTICATION FLOW
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register-user")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDto dto, CancellationToken cancellationToken)
    {
        return await Register(dto, AppRoles.User, cancellationToken);
    }

    [HttpPost("register-admin")]
    public async Task<IActionResult> RegisterAdmin([FromBody] RegisterUserDto dto, CancellationToken cancellationToken)
    {
        return await Register(dto, AppRoles.Admin, cancellationToken);
    }

    
    

    // POST: api/auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto dto, string role, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var command = new RegisterUserCommand(dto, role);
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(Register), new { id = result.Token }, result);
    }

    // POST: api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var command = new LoginUserCommand(dto);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result); // AuthResultDto with token + refresh
    }

    // POST: api/auth/refresh
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState); 
        
        var command = new RefreshTokenCommand(dto);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result); // Same AuthResultDto
    }

    // POST: api/auth/logout
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutUserDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var command = new LogoutUserCommand(dto);
        await _mediator.Send(command, cancellationToken);
        return Ok(new { Success = true, Message = "Logged out successfully" });
    }

    // POST: api/auth/change-password
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var command = new ChangePasswordCommand(dto);
        await _mediator.Send(command, cancellationToken);
        return Ok(new { Success = true, Message = "Password changed successfully" });
    }

    // Confirm Email
    // GET: api/auth/confirm-email?userId={userId}&token={token}
    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] VerifyEmailDto dto, CancellationToken cancellationToken)
    {
        var command = new EmailConfirmationCommand(dto);
        await _mediator.Send(command, cancellationToken);
        return Ok(new { Success = true, Message = "Email confirmed successfully" });
    }

    // Forgot Password - Request Reset Token
    // POST: api/auth/forgot-password
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var command = new ForgotPasswordCommand(dto);
        await _mediator.Send(command, cancellationToken);
        return Ok(new { Success = true, Message = "Password reset token sent to email if it exists" });
    }

    // Reset Password
    // POST: api/auth/reset-password?userId={userId}&token={token}
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var command = new ResetPasswordCommand(dto);
        await _mediator.Send(command, cancellationToken);
        return Ok(new { Success = true, Message = "Password has been reset successfully" });
    }
}

