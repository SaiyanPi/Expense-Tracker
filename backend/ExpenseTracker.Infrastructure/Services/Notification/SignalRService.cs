using ExpenseTracker.Application.Common.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace ExpenseTracker.Infrastructure.Services.Notification;

public class SignalRService : INotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public SignalRService(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task BudgetExceededAsync(
        Guid budgetId,
        string budgetName,
        decimal totalSpent,
        decimal budgetAmount,
        string userId,
        CancellationToken cancellationToken = default)
    {
        await _hubContext
            .Clients
            .User(userId)
            .SendAsync("BudgetExceeded", new
            {
                budgetId,
                budgetName,
                totalSpent,
                budgetAmount,
                exceededAt = DateTime.UtcNow
            }, cancellationToken);
    }
}