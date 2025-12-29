namespace ExpenseTracker.Application.DTOs.Auth;

public class PhoneConfirmationDto
{
    public string UserEmail { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;

}