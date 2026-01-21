using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Application.Common.Security;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.SharedKernel;

namespace ExpenseTracker.API.Security;  
public class AuthorizationFailureResultHandler
    : IAuthorizationMiddlewareResultHandler
{
    private readonly AuthorizationMiddlewareResultHandler _defaultHandler = new();

    public async Task HandleAsync(
        RequestDelegate next,
        HttpContext context,
        AuthorizationPolicy policy,
        PolicyAuthorizationResult authorizeResult)
    {
        if (authorizeResult.Forbidden &&
            context.User.Identity?.IsAuthenticated == true)
        {
            // Prevent duplicate logs
            if (!context.Items.ContainsKey("403Logged"))
            {
                context.Items["403Logged"] = true;

                using var scope = context.RequestServices.CreateScope();
                var securityLogger = scope.ServiceProvider
                    .GetRequiredService<ISecurityEventLoggerService>();

                await securityLogger.LogSecurityEventAsync(new SecurityEventLog
                {
                    EventType = SecurityEventTypes.AccessDenied,
                    Outcome = SecurityEventOutcome.Denied,
                    UserId = context.User.FindFirstValue(ClaimTypes.NameIdentifier),
                    UserEmail = context.User.FindFirstValue(ClaimTypes.Email),
                    Endpoint = SecurityEventContext.GetEndpoint(context),
                    IpAddress = SecurityEventContext.GetIp(context),
                    UserAgent = SecurityEventContext.GetUserAgent(context),
                    CorrelationId = SecurityEventContext.GetCorrelationId(context),
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        // Continue default framework behavior
        await _defaultHandler.HandleAsync(
            next, context, policy, authorizeResult);
    }
}


// Handle 403 forbidden results to log security events