namespace ExpenseTracker.Application.DTOs.Auth;

public class VerifyPhoneDto
{
    public string UserId { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public string Token { get; set; } = default!;

}