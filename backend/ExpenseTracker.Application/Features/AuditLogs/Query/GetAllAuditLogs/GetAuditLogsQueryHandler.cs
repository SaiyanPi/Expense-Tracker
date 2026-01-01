using AutoMapper;
using ExpenseTracker.Application.Common.Pagination;
using ExpenseTracker.Application.DTOs.AuditLog;
using ExpenseTracker.Application.Features.AuditLogs.Query.GetAllAuditLogs;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Application.Features.AuditLogs.QueryGetAllAuditLogs;

public class GetAuditLogsQueryHandler
    : IRequestHandler<GetAuditLogsQuery, PagedResult<AuditLogDto>>
{
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IMapper _mapper;

    public GetAuditLogsQueryHandler(
        IAuditLogRepository auditLogRepository,
        IMapper mapper)
    {
        _auditLogRepository = auditLogRepository;
        _mapper = mapper;
    }

    public async Task<PagedResult<AuditLogDto>> Handle(
        GetAuditLogsQuery request,
        CancellationToken cancellationToken)
    {
        var filter = request.Filter;
        var page = request.Paging;

        IQueryable<AuditLog> query = _auditLogRepository.GetAuditLogsQueryable();


        // Filtering
        if (!string.IsNullOrWhiteSpace(filter.EntityName))
            query = query.Where(x => x.EntityName == filter.EntityName);

        if (!string.IsNullOrWhiteSpace(filter.UserId))
            query = query.Where(x => x.UserId == filter.UserId);

        if (filter.Action.HasValue)
            query = query.Where(x => x.Action == filter.Action.Value);

        if (filter.StartDate.HasValue)
            query = query.Where(x => x.CreatedAt >= filter.StartDate.Value);

        if (filter.EndDate.HasValue)
            query = query.Where(x => x.CreatedAt <= filter.EndDate.Value);

        // Sorting
        query = query.ApplySorting(page.SortBy, page.SortDesc);

        // Pagination
        var totalCount = await query.CountAsync(cancellationToken);

        var auditLogs = await query
            .Skip(page.Skip)
            .Take(page.EffectivePageSize)
            .ToListAsync(cancellationToken);
        
        var mappedAuditLogs = _mapper.Map<IReadOnlyList<AuditLogDto>>(auditLogs);
        
        return new PagedResult<AuditLogDto>(
            mappedAuditLogs,
            totalCount,
            page.EffectivePage,
            page.EffectivePageSize);
    
    }
}