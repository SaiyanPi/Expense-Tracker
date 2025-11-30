namespace ExpenseTracker.Application.DTOs.Auth;

public class AuthResultDto
{
    public bool Success { get; set; }
    public string Token { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;
    public DateTime ExpiresAt { get; set; }
    public IEnumerable<string> Errors { get; set; } = Enumerable.Empty<string>();
}

