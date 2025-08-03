namespace ExpenseTracker.Domain.Entities;

public class Category
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;

    // Multi-user support
    public string? UserId { get; set; }     // foreign key

    // Navigation
    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();       // collectional navigation 
}
