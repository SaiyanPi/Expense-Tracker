using ExpenseTracker.Application.Common.Authorization;
using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.DTOs.Auth;
using ExpenseTracker.Application.Features.Identity.Commands.ChangePassword;
using ExpenseTracker.Application.Features.Identity.Commands.ConfirmChangeEmail;
using ExpenseTracker.Application.Features.Identity.Commands.ConfirmPhone;
using ExpenseTracker.Application.Features.Identity.Commands.EmailConfirmation;
using ExpenseTracker.Application.Features.Identity.Commands.ForgotPassword;
using ExpenseTracker.Application.Features.Identity.Commands.Login;
using ExpenseTracker.Application.Features.Identity.Commands.Logout;
using ExpenseTracker.Application.Features.Identity.Commands.RefreshToken;
using ExpenseTracker.Application.Features.Identity.Commands.Register;
using ExpenseTracker.Application.Features.Identity.Commands.RequestChangeEmail;
using ExpenseTracker.Application.Features.Identity.Commands.ResetPassword;
using ExpenseTracker.Application.Features.Identity.Commands.SendPhoneConfirmationCode;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.API.Controllers.V1;

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
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDto dto, CancellationToken cancellationToken = default)
    {
        return await Register(dto, AppRoles.User, cancellationToken);
    }

    [HttpPost("register-admin")]
    public async Task<IActionResult> RegisterAdmin([FromBody] RegisterUserDto dto, CancellationToken cancellationToken = default)
    {
        return await Register(dto, AppRoles.Admin, cancellationToken);
    }

    
    

    // POST: api/auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto dto, string role, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var command = new RegisterUserCommand(dto, role);
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(Register), new { id = result.Token }, result);
    }

    // POST: api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserDto dto, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var command = new LoginUserCommand(dto);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result); // AuthResultDto with token + refresh
    }

    // POST: api/auth/refresh
    // [Authorize]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto dto, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState); 
        
         try
        {
            var command = new RefreshTokenCommand(dto);
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result); // AuthResultDto
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { Message = "Invalid or expired refresh token." });
        }
        catch (IdentityOperationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    // POST: api/auth/logout
    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var command = new LogoutUserCommand();
        await _mediator.Send(command, cancellationToken);
        return Ok(new { Success = true, Message = "Logged out successfully" });
    }

    // POST: api/auth/change-password
    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var command = new ChangePasswordCommand(dto);
        await _mediator.Send(command, cancellationToken);
        return Ok(new { Success = true, Message = "Password changed successfully" });
    }

    // Confirm Email
    // GET: api/auth/confirm-email?userId={userId}&token={token}
    [AllowAnonymous]
    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] VerifyEmailDto dto, CancellationToken cancellationToken = default)
    {
        var command = new EmailConfirmationCommand(dto);
        await _mediator.Send(command, cancellationToken);
        return Ok(new { Success = true, Message = "Email confirmed successfully" });
    }

    // Forgot Password - Request Reset Token
    // POST: api/auth/forgot-password
    [AllowAnonymous]
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var command = new ForgotPasswordCommand(dto);
        await _mediator.Send(command, cancellationToken);
        return Ok(new { Success = true, Message = "Password reset token sent to email if it exists" });
    }

    // Reset Password
    // POST: api/auth/reset-password?userId={userId}&token={token}
    [AllowAnonymous]
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(
        [FromQuery] string userId,
        [FromQuery] string token,
        [FromBody] ResetPasswordDto dto, 
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var command = new ResetPasswordCommand(userId, token, dto);
        await _mediator.Send(command, cancellationToken);
        return Ok(new { Success = true, Message = "Password has been reset successfully" });
    }

    // POST: api/auth/send-phone-otp
    [AllowAnonymous]
    [HttpPost("send-phone-otp")]
    public async Task<IActionResult> SendPhoneOtp([FromBody] PhoneConfirmationDto dto, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var command = new SendPhoneConfirmationCodeCommand(dto);
        await _mediator.Send(command, cancellationToken);
        return Ok(new { Success = true, Message = "OTP sent (check your verified phone)." });
    }

    // POST: api/auth/send-phone-otp
    [AllowAnonymous]
    [HttpPost("confirm-phone-otp")]
    public async Task<IActionResult> ConfirmPhoneOtp([FromBody] VerifyPhoneDto dto, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var command = new ConfirmPhoneCommand(dto);
        var success =await _mediator.Send(command, cancellationToken);
        return Ok(new { Success = true, Message = "Phone confirmed." });
    }

    //POST: api/auth/change-email
    [HttpPost("change-email")]
    public async Task<IActionResult> RequestChangeEmail([FromBody] ChangeEmailRequestDto dto, CancellationToken cancellationToken = default)
    {
        if(!ModelState.IsValid)
            return BadRequest(ModelState);
        
        var command = new RequestChangeEmailCommand(dto);
        await _mediator.Send(command, cancellationToken);
        return Ok(new {Success = true, Message = "Email change confirmation link sent to your new email"});
    }


    // GET: api/auth/confirm-change-email?userId={UserId}&newEmail={newEmail}&token={token}
    [AllowAnonymous]
    [HttpGet("confirm-change-email")]
    public async Task<IActionResult> ConfirmChangeEmail([FromQuery] ConfirmChangeEmailDto dto, CancellationToken cancellationToken = default)
    {
        if(!ModelState.IsValid)
            return BadRequest(ModelState);
        
        var command = new ConfirmChangeEmailCommand(dto);
        await _mediator.Send(command, cancellationToken);
        return Ok(new {Success = true, Message = "Email has been changed successfully"});
    }
}

