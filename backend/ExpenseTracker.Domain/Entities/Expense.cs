using ExpenseTracker.Domain.Common;

namespace ExpenseTracker.Domain.Entities;

public class Expense : BaseSoftDeletableEntity
{
    public string Title { get; set; } = default!;
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public Guid? CategoryId { get; set; }    // foreign key
    public Guid? BudgetId { get; set; }     // foreign key
    public string? UserId { get; set; }     // foreign key
    public Category Category { get; private set; } = default!;      // reference navigation to Category
    public Budget Budget { get; private set; } = default!;        // reference navigation to Budget
    
    //  public ApplicationUser User { get; set; }   // ❌ breaks Clean Architecture
    // (domain layer must not depend on persistence layer)
}
