namespace ExpenseTracker.Application.DTOs.Auth;

public class PhoneConfirmationDto
{
    public string UserId { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;

}