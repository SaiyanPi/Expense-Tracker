using ExpenseTracker.Application.Common.Pagination;
using ExpenseTracker.Application.DTOs.AuditLog;
using MediatR;

namespace ExpenseTracker.Application.Features.AuditLogs.Query.GetAuditTimelineByUserId;

public record GetAuditTimelineByUserIdQuery(
    string UserId,
    PagedQuery Paging)
: IRequest<PagedResult<AuditTimelineItemDto>>;