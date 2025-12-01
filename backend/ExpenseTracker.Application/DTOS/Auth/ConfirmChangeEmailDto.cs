namespace ExpenseTracker.Application.DTOs.Auth;

public class ConfirmChangeEmailDto
{
    public string UserId { get; set; } = default!;
    public string NewEmail { get; set; } = default!;
    public string Token { get; set; } = default!;
}