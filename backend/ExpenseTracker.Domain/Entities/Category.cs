using ExpenseTracker.Domain.Common;

namespace ExpenseTracker.Domain.Entities;

public class Category : BaseSoftDeletableEntity
{    
    public string Name { get; set; } = default!;
    public string? UserId { get; set; }     // foreign key
    public ICollection<Expense> Expenses { get; private set; } = new List<Expense>();       // collectional navigation 




    // public ApplicationUser User { get; set; }  // ❌ breaks Clean Architecture
    // (domain layer must not depend on persistence layer)

}
