namespace ExpenseTrackler.Application.DTOs.User;

public class RegisterUserDto
{
    public string FullName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
   
}