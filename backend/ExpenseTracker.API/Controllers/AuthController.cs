using ExpenseTracker.Application.DTOs.User;
using ExpenseTracker.Application.Interfaces.Services;
using ExpenseTrackler.Application.DTOs.User;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
// FOR AUTHENTICATION FLOW
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    // POST: api/auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto dto, CancellationToken cancellationToken)
    {
        var result = await _userService.RegisterAsync(dto, cancellationToken);
        return Ok(result); // Could return AuthResultDto right away after registration
    }

    // POST: api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserDto dto, CancellationToken cancellationToken)
    {
        var result = await _userService.LoginAsync(dto, cancellationToken);
        return Ok(result); // AuthResultDto with token + refresh
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserDto dto, CancellationToken cancellationToken)
    {
        await _userService.UpdateAsync(id, dto, cancellationToken);
        return NoContent();
    }

    // POST: api/auth/refresh
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto dto, CancellationToken cancellationToken)
    {
        var result = await _userService.RefreshTokenAsync(dto.Token, dto.RefreshToken, cancellationToken);
        return Ok(result); // Same AuthResultDto
    }

    // POST: api/auth/logout
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutUserDto dto, CancellationToken cancellationToken)
    {
        await _userService.LogoutAsync(dto.Email, cancellationToken);
        return Ok(new { Success = true, Message = "Logged out successfully" });
    }
}

