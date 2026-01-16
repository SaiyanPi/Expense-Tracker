using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using ExpenseTracker.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Infrastructure.Repositories;

public class SecurityEventLogRepository : ISecurityEventLogRepository
{
    private readonly ExpenseTrackerDbContext _dbContext;

    public SecurityEventLogRepository(ExpenseTrackerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IQueryable<SecurityEventLog> GetSecurityEventLogsQueryable()
    {
        return _dbContext.SecurityEventLogs.AsNoTracking();
    }

    public async Task<SecurityEventLog?> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.SecurityEventLogs
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<int> DeleteOlderThanAsync(DateTime cutOffDate, CancellationToken cancellationToken = default)
    {
        return await _dbContext.SecurityEventLogs
            .Where(a => a.Timestamp < cutOffDate)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public IQueryable<SecurityEventLog> GetSecurityEventTimelineQueryable()
    {
        return _dbContext.SecurityEventLogs.AsNoTracking();
    }

}