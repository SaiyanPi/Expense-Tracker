using ExpenseTracker.Application.DTOs.AuditLog;
using MediatR;

namespace ExpenseTracker.Application.Features.AuditLogs.Query.GetAuditLogById;

public record GetAuditLogByIdQuery(Guid Id) :  IRequest<AuditLogDto>;