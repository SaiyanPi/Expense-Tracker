using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ExpenseTracker.Infrastructure.Services.Notification;

[Authorize]
public class NotificationHub : Hub
{
    public const string HubUrl = "/hubs/notifications";
    
    // add methods if clients want to call server-side actions

    public override Task OnConnectedAsync()
    {
        Console.WriteLine($"SignalR connected. UserId = {Context.UserIdentifier}");
        return base.OnConnectedAsync();
    }
}