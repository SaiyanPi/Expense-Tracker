namespace ExpenseTracker.Application.Common.Interfaces;

public interface INotificationService
{
    Task BudgetExceededAsync(
        Guid budgetId,
        string budgetName,
        decimal totalSpent,
        decimal budgetAmount,
        string userId,
        CancellationToken cancellationToken = default);
}