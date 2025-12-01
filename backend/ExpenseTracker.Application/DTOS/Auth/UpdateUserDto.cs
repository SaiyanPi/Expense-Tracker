namespace ExpenseTracker.Application.DTOs.Auth;

public class UpdateUserDto
{
    public string FullName { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
   
}