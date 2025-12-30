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
}