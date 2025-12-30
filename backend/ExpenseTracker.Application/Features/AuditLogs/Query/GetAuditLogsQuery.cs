using ExpenseTracker.Application.Common.Pagination;
using ExpenseTracker.Application.DTOs.AuditLog;
using MediatR;

namespace ExpenseTracker.Application.Features.AuditLogs.Query;

public record GetAuditLogsQuery(AuditLogFilter Filter, PagedQuery Paging) : IRequest<PagedResult<AuditLogDto>>;