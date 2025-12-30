using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Models;

namespace ExpenseTracker.Domain.Interfaces.Repositories;

public interface IAuditLogRepository
{
    IQueryable<AuditLog> GetAuditLogsQueryable();

}