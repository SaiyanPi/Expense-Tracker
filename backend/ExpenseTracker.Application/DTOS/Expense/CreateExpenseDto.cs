namespace ExpenseTrackler.Application.DTOs.Expense;

public class CreateExpenseDto
{
    public string Title { get; set; } = default!;
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public Guid CategoryId { get; set; }    // foreign key
    public string? UserId { get; set; }     // foreign key
   
}