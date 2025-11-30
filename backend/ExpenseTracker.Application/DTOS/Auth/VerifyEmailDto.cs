namespace ExpenseTracker.Application.DTOs.Auth;

public class VerifyEmailDto
{
    public string UserId { get; set; } = default!;
    public string Token { get; set; } = default!;
}