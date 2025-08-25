namespace ExpenseTracker.Domain.Entities;

public class Category
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = default!;
    public string? UserId { get; set; }     // foreign key
    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();       // collectional navigation 




    // public ApplicationUser User { get; set; }  // ❌ breaks Clean Architecture
    // (domain layer must not depend on persistence layer)

}
