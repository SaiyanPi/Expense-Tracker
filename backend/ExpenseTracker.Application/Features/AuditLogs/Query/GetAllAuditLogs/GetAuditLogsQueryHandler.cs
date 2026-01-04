using AutoMapper;
using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.Common.Pagination;
using ExpenseTracker.Application.DTOs.AuditLog;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Application.Features.AuditLogs.Query.GetAllAuditLogs;

public class GetAuditLogsQueryHandler
    : IRequestHandler<GetAuditLogsQuery, PagedResult<AuditLogDto>>
{
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetAuditLogsQueryHandler(
        IAuditLogRepository auditLogRepository,
        IUserRepository userRepository,
        IMapper mapper)
    {
        _auditLogRepository = auditLogRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<PagedResult<AuditLogDto>> Handle(
        GetAuditLogsQuery request,
        CancellationToken cancellationToken)
    {   
        // Validate userId if provided
        if (!string.IsNullOrWhiteSpace(request.Filter.UserId))
        {
            var user = await _userRepository.GetByIdAsync(request.Filter.UserId);
            if(user is null)
                throw new NotFoundException(nameof(User), request.Filter.UserId);
        }

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