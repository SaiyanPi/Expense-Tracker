namespace ExpenseTracker.Domain.Entities;

public class Expense
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = default!;
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public Guid CategoryId { get; set; }    // foreign key
    public string? UserId { get; set; }     // foreign key
    public Category Category { get; set; } = default!;      // reference navigation to Category

    
    //  public ApplicationUser User { get; set; }   // ❌ breaks Clean Architecture
    // (domain layer must not depend on persistence layer)
}
