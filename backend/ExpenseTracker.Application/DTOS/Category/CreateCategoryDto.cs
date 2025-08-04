namespace ExpenseTrackler.Application.DTOs.Expense;

public class CreateCategoryDto
{
    public string Name { get; set; } = default!;
    public string? UserId { get; set; }     // foreign key
 
   
}