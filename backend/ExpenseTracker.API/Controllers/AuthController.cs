
using ExpenseTracker.Application.Common.Constants;
using ExpenseTracker.Application.DTOs.User;
using ExpenseTracker.Application.Features.Identity.Commands.Login;
using ExpenseTracker.Application.Features.Identity.Commands.Logout;
using ExpenseTracker.Application.Features.Identity.Commands.RefreshToken;
using ExpenseTracker.Application.Features.Identity.Commands.Register;
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
}

