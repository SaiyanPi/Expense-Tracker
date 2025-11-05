namespace ExpenseTracker.Application.DTOs.Category;

public class CreateCategoryDto
{
    public string Name { get; set; } = default!;
    public string? UserId { get; set; }     // foreign key
 
   
}