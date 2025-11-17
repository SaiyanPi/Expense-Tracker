namespace ExpenseTracker.Application.DTOs.User;

public class ChangePasswordDto
{
    public string Email { get; set; } = default!;
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}