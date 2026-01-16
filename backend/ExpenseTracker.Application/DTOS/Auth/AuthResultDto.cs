namespace ExpenseTracker.Application.DTOs.Auth;

public class AuthResultDto
{
    public AuthResultDto()
    {
        Success = false; // default to false
        Token = string.Empty;
        RefreshToken = string.Empty;
        Errors = Enumerable.Empty<string>();
    }
    public bool Success { get; set; }
    public string Token { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;
    public DateTime ExpiresAt { get; set; }
    public IEnumerable<string> Errors { get; set; }
}

