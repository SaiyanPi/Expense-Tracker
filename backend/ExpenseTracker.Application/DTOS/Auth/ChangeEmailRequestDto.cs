namespace ExpenseTracker.Application.DTOs.Auth;

public class ChangeEmailRequestDto
{
    public string UserId { get; set; } = default!;
    public string NewEmail { get; set; } = default!;
}