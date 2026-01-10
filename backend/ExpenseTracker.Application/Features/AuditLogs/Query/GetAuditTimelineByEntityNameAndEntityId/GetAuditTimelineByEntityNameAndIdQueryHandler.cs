using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.Common.Pagination;
using ExpenseTracker.Application.DTOs.AuditLog;
using ExpenseTracker.Domain.Interfaces.Repositories;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Application.Features.AuditLogs.Query.GetAuditTimelineByEntityNameAndEntityId;

public sealed class GetAuditTimelineByEntityNameAndIdQueryHandler
    : IRequestHandler<GetAuditTimelineByEntityNameAndIdQuery, PagedResult<AuditTimelineItemDto>>
{
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IEntityResolverRepository _resolverRepository;

    public GetAuditTimelineByEntityNameAndIdQueryHandler(
        IAuditLogRepository auditLogRepository,
        IEntityResolverRepository resolverRepository)
    {
        _auditLogRepository = auditLogRepository;
        _resolverRepository = resolverRepository;

    }

    public async Task<PagedResult<AuditTimelineItemDto>> Handle(
        GetAuditTimelineByEntityNameAndIdQuery request,
        CancellationToken cancellationToken)
    {
        // parse the enum: 
        // since we've used enum type entityName as string type, we need to parse it
        // either on controller or in handler
        EntityType? entityName = null;
        if (!string.IsNullOrWhiteSpace(request.EntityName))
        {
            entityName = Enum.Parse<EntityType>(
                request.EntityName,
                ignoreCase: true
            );
        }

        // EntityName is validated in the fluent
        var entityWithIdExist = await _resolverRepository.ExistsAsync(entityName!.Value, request.EntityId, cancellationToken);
        if (entityWithIdExist is null)
            throw new NotFoundException($"{request.EntityName} with id {request.EntityId} not found");

        var paging = request.Paging;

        var baseQuery = _auditLogRepository.GetAuditTimelineQueryable()
            .Where(a => a.EntityName == entityName && a.EntityId == request.EntityId);

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
