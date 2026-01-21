using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Application.Common.Observability.Metrics.Security;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Persistence;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Infrastructure.Services.SecurityEventLogger;

public class SecurityEventLoggerService : ISecurityEventLoggerService
{
    private readonly ExpenseTrackerDbContext _dbContext;
    private readonly ILogger<SecurityEventLoggerService> _logger;
    public SecurityEventLoggerService(
        ExpenseTrackerDbContext dbContext,
        ILogger<SecurityEventLoggerService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task LogSecurityEventAsync(SecurityEventLog securityEvent)
    {
        try
        {
            _dbContext.SecurityEventLogs.Add(securityEvent);
            await _dbContext.SaveChangesAsync();
            SecurityEventMetric.RecordEvent(securityEvent.EventType, securityEvent.Outcome.ToString());
        }
        catch(Exception exception)
        {
            _logger.LogError(exception, "Failed to log security event.");
            // Swallow exceptions to avoid impacting main flow
        }
    }
}