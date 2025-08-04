namespace ExpenseTrackler.Application.DTOs.Expense;

public class UpdateCategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;

}