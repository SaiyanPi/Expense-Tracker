using ExpenseTracker.Application.Common.Pagination;
using ExpenseTracker.Application.DTOs.AuditLog;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Application.Features.AuditLogs.Query.GetAuditTimelineByUserId;

public sealed class GetAuditTimelineByUserIdQueryHandler
    : IRequestHandler<GetAuditTimelineByUserIdQuery, PagedResult<AuditTimelineItemDto>>
{
    private readonly IAuditLogRepository _auditLogRepository;

    public GetAuditTimelineByUserIdQueryHandler(IAuditLogRepository auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    public async Task<PagedResult<AuditTimelineItemDto>> Handle(
        GetAuditTimelineByUserIdQuery request,
        CancellationToken cancellationToken)
    {
        var paging = request.Paging;

        var baseQuery = _auditLogRepository.GetAuditTimelineQueryable()
            .Where(a => a.UserId == request.UserId);
        
        var totalCount = await baseQuery.CountAsync(cancellationToken);

        var items = await baseQuery
            .OrderByDescending(a => a.CreatedAt)   // index-friendly
            .Skip(paging.Skip)
            .Take(paging.EffectivePageSize)
            .Select(a => new AuditTimelineItemDto(
                a.Id,
                a.Action.ToString(),
                a.OldValues,
                a.NewValues,
                a.CreatedAt,
                a.UserId,
                a.CorrelationId,
                a.HttpMethod,
                a.RequestPath
            ))
            .ToListAsync(cancellationToken);
        
        return new PagedResult<AuditTimelineItemDto>(
            items,
            totalCount,
            paging.EffectivePage,
            paging.EffectivePageSize
        );
    }
}