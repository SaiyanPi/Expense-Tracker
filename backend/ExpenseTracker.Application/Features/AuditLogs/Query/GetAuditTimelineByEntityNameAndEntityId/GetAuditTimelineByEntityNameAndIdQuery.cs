using ExpenseTracker.Application.Common.Pagination;
using ExpenseTracker.Application.DTOs.AuditLog;
using MediatR;

namespace ExpenseTracker.Application.Features.AuditLogs.Query.GetAuditTimelineByEntityNameAndEntityId;

public record GetAuditTimelineByEntityNameAndIdQuery(
    string EntityName,
    string EntityId,
    PagedQuery Paging)
: IRequest<PagedResult<AuditTimelineItemDto>>;