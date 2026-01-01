using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using ExpenseTracker.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Infrastructure.Repositories;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly ExpenseTrackerDbContext _dbContext;

    public AuditLogRepository(ExpenseTrackerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IQueryable<AuditLog> GetAuditLogsQueryable()
    {
        return _dbContext.AuditLogs.AsNoTracking();
    }

    public async Task<AuditLog?> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.AuditLogs
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<int> DeleteOlderThanAsync(DateTime cutOffDate, CancellationToken cancellationToken = default)
    {
        return await _dbContext.AuditLogs
            .Where(a => a.CreatedAt < cutOffDate)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public IQueryable<AuditLog> GetAuditTimelineQueryable()
    {
        return _dbContext.AuditLogs.AsNoTracking();
    }

}