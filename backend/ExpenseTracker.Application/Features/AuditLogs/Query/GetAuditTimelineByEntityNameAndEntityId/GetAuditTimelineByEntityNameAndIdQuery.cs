using ExpenseTracker.Application.Common.Pagination;
using ExpenseTracker.Application.DTOs.AuditLog;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;

namespace ExpenseTracker.Application.Features.AuditLogs.Query.GetAuditTimelineByEntityNameAndEntityId;

public record GetAuditTimelineByEntityNameAndIdQuery(
    string EntityName,
    Guid EntityId,
    PagedQuery Paging)
: IRequest<PagedResult<AuditTimelineItemDto>>;