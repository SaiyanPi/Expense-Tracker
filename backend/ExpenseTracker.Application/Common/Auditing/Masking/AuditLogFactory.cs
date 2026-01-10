using ExpenseTracker.Application.Common.Auditing;
using ExpenseTracker.Application.Common.Context;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.SharedKernel;

namespace ExpenseTracker.Application.Common.Auditing.Masking;
public static class AuditLogFactory
{
    public static AuditLog Create(
        EntityType entityName,
        Guid entityId,
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