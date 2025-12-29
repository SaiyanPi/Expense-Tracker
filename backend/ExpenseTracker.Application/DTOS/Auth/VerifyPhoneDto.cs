namespace ExpenseTracker.Application.DTOs.Auth;

public class VerifyPhoneDto
{
    public string UserEmail { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public string Token { get; set; } = default!;

}