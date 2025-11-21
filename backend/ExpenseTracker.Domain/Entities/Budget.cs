namespace ExpenseTracker.Domain.Entities;

public class Budget
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public string? UserId { get; private set; }
    public Guid? CategoryId { get; private set; }
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