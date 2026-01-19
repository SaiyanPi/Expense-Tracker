namespace ExpenseTracker.API.Contracts.V1.Category;

public class CategoryResponseV1
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? UserId { get; set; }     // foreign key 
}