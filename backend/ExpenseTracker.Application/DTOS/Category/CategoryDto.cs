namespace ExpenseTrackler.Application.DTOs.Expense;

public class CategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? UserId { get; set; }     // foreign key
 
   
}