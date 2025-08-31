namespace ExpenseTracker.Application.DTOs.User;

public class RefreshTokenDto
{
    public string Token { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;
}
