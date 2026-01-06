using ExpenseTracker.Domain.SharedKernel;

namespace ExpenseTracker.Domain.Entities;
public class Budget : BaseSoftDeletableEntity
{
    public string Name { get; set; }
    public decimal Amount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? UserId { get; set; }
    public Guid? CategoryId { get; set; }
    public ICollection<Expense> Expenses { get; set; } = new List<Expense>(); 
    public bool IsActive => DateTime.UtcNow >= StartDate && DateTime.UtcNow <= EndDate;

    public Budget(string name,
        decimal amount,
        DateTime startDate,
        DateTime endDate,
        string? userId = default!, 
        Guid? categoryId = null)
    {
        Id = Guid.NewGuid();
        Name = name;
        Amount = amount;
        StartDate = startDate;
        EndDate = endDate;
        UserId = userId;
        CategoryId = categoryId;
    }


}