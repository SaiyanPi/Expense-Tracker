using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Models;

namespace ExpenseTracker.Domain.Interfaces.Repositories;

public interface IAuditLogRepository
{
    IQueryable<AuditLog> GetAuditLogsQueryable();
    Task<AuditLog?> GetById(Guid id, CancellationToken cancellationToken = default);
    Task<int> DeleteOlderThanAsync(DateTime cutOffDate, CancellationToken cancellationToken = default);
    IQueryable<AuditLog> GetAuditTimelineQueryable();
}