namespace ExpenseTracker.Domain.Common;

public abstract class BaseSoftDeletableEntity : BaseAuditableEntity
{
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}
