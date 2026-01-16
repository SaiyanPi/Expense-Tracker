using ExpenseTracker.Domain.Entities;

namespace ExpenseTracker.Domain.Interfaces.Repositories
{
    public interface ISecurityEventLogRepository
    {
        IQueryable<SecurityEventLog> GetSecurityEventLogsQueryable();
        Task<SecurityEventLog?> GetById(Guid id, CancellationToken cancellationToken = default);
        Task<int> DeleteOlderThanAsync(DateTime cutOffDate, CancellationToken cancellationToken = default);
        IQueryable<SecurityEventLog> GetSecurityEventTimelineQueryable();
    }
}