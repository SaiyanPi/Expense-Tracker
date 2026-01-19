namespace ExpenseTracker.API.Contracts.V1.Category;

public class CreateCategoryRequestV1
{
    public string Name { get; set; } = default!;
    public string? UserId { get; set; }     // foreign key
 
   
}