namespace ExpenseTracker.Application.Common.Interfaces;

public interface INotificationService
{
    Task BudgetExceededAsync(
        Guid budgetId,
        string budgetName,
        // decimal totalSpent,
        // decimal budgetAmount,
        decimal percentageUsed,
        decimal remainingAmount,
        string userId,
        CancellationToken cancellationToken = default);
}