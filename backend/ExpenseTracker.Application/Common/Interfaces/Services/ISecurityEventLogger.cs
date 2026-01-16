using ExpenseTracker.Domain.Entities;

namespace ExpenseTracker.Application.Common.Interfaces.Services;

public interface ISecurityEventLogger
{
    Task LogSecurityEventAsync(SecurityEventLog securityEvent);
}