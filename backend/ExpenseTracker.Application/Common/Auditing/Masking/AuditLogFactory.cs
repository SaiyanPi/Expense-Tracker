using ExpenseTracker.Application.Common.Auditing;
using ExpenseTracker.Application.Common.Context;
using ExpenseTracker.Domain.Common;
using ExpenseTracker.Domain.Entities;

namespace ExpenseTracker.Application.Common.Auditing.Masking;
public static class AuditLogFactory
{
    public static AuditLog Create(
        string entityName,
        string entityId,
        AuditAction action,
        string? oldValues,
        string? newValues,
        string? userId,
        
        IRequestContext requestContext)
    {
        return new AuditLog
        {
            EntityName = entityName,
            EntityId = entityId,
            Action = action,
            OldValues = AuditValueMasker.MaskSensitiveJson(oldValues),
            NewValues = AuditValueMasker.MaskSensitiveJson(newValues),
            UserId = userId,
            CreatedAt = DateTime.UtcNow,

            // correlation + request meta data
            CorrelationId = requestContext.CorrelationId,
            HttpMethod = requestContext.HttpMethod,
            RequestPath = requestContext.RequestPath,
            ClientIp = requestContext.ClientIp,
            UserAgent = requestContext.UserAgent
        };
    }
}