namespace ExpenseTrackler.Application.DTOs.Expense;

public class UpdateExpenseDto
{
    public string Title { get; set; } = default!;
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public Guid CategoryId { get; set; }    // foreign key
   
}