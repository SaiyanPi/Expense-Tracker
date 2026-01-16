using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Persistence;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Infrastructure.Services.SecurityEventLogger;

public class SecurityEventLogger : ISecurityEventLogger
{
    private readonly ExpenseTrackerDbContext _dbContext;
    private readonly ILogger<SecurityEventLogger> _logger;
    public SecurityEventLogger(ExpenseTrackerDbContext dbContext, ILogger<SecurityEventLogger> logger)
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
        }
        catch(Exception exception)
        {
            _logger.LogError(exception, "Failed to log security event.");
            // Swallow exceptions to avoid impacting main flow
        }
    }
}