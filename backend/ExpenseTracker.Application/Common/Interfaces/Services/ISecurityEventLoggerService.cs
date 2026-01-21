using ExpenseTracker.Domain.Entities;

namespace ExpenseTracker.Application.Common.Interfaces.Services;

public interface ISecurityEventLoggerService
{
    Task LogSecurityEventAsync(SecurityEventLog securityEvent);
}