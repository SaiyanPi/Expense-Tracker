using ExpenseTracker.Application.DTOs.SecurityEventLog;
using MediatR;

namespace ExpenseTracker.Application.Features.SecurityEventLogs.Query.GetSecurityEventLogById;

public record GetSecurityEventLogByIdQuery(Guid Id) : IRequest<SecurityEventLogDto>;