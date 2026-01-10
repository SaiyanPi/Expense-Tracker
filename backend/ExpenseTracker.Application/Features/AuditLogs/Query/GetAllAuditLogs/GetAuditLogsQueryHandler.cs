using AutoMapper;
using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.Common.Pagination;
using ExpenseTracker.Application.DTOs.AuditLog;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using ExpenseTracker.Domain.SharedKernel;
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
        var filter = request.Filter;
        var page = request.Paging;

        // Validate userId if provided
        if (!string.IsNullOrWhiteSpace(filter.UserId))
        {
            var user = await _userRepository.GetByIdAsync(filter.UserId);
            if(user is null)
                throw new NotFoundException(nameof(User), filter.UserId);
        }

        // parse the enum: 
        // since we've used enum type entityName and auditAction as string type, we need to parse it
        // either on controller or in handler
        EntityType? entityName = null;
        if (!string.IsNullOrWhiteSpace(filter.EntityName))
        {
            entityName = Enum.Parse<EntityType>(
                filter.EntityName,
                ignoreCase: true
            );
        }

        AuditAction? auditAction = null;
        if (!string.IsNullOrWhiteSpace(filter.Action))
        {
            auditAction = Enum.Parse<AuditAction>(
                filter.Action,
                ignoreCase: true
            );
        }

        IQueryable<AuditLog> query = _auditLogRepository.GetAuditLogsQueryable();

        // use the parsed enum in the query
        if (entityName.HasValue)
            query = query.Where(x => x.EntityName == entityName.Value);

        if (auditAction.HasValue)
            query = query.Where(x => x.Action == auditAction.Value);

        if (!string.IsNullOrWhiteSpace(filter.UserId))
            query = query.Where(x => x.UserId == filter.UserId);

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