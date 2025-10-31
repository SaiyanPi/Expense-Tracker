namespace ExpenseTrackler.Application.DTOs.Category;

public class UpdateCategoryDto
{
    public string Name { get; set; } = default!;
    public string UserId { get; set; } = default!;     // foreign key

}