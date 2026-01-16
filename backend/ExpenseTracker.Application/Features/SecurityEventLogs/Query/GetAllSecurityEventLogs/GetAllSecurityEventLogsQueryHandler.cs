using AutoMapper;
using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.Common.Pagination;
using ExpenseTracker.Application.DTOs.SecurityEventLog;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Application.Features.SecurityEventLogs.Query.GetAllSecurityEventLogs;

public class GetSecurityEventLogsQueryableHandler 
    : IRequestHandler<GetAllSecurityEventLogsQuery, PagedResult<SecurityEventLogDto>>
{
    private readonly ISecurityEventLogRepository _securityEventLogRepository;
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;

    public GetSecurityEventLogsQueryableHandler(
        ISecurityEventLogRepository securityEventLogRepository,
        IMapper mapper,
        IUserRepository userRepository)
    {
        _securityEventLogRepository = securityEventLogRepository;
        _mapper = mapper;
        _userRepository = userRepository;
    }

    public async Task<PagedResult<SecurityEventLogDto>> Handle(
        GetAllSecurityEventLogsQuery request,
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

        // validate UserEmail if provided
        if (!string.IsNullOrWhiteSpace(filter.UserEmail))
        {
            var user = await _userRepository.GetByEmailAsync(filter.UserEmail);
            if(user is null)
                throw new NotFoundException(nameof(User), filter.UserEmail);
        }

        // PARSE THE ENUMS
        SecurityEventTypes? eventType = null;
        if (!string.IsNullOrWhiteSpace(filter.EventType))
        {
            eventType = Enum.Parse<SecurityEventTypes>(
                filter.EventType,
                ignoreCase: true
            );
        }

        SecurityEventOutcome? outcome = null;
        if (!string.IsNullOrWhiteSpace(filter.Outcome))
        {
            outcome = Enum.Parse<SecurityEventOutcome>(
                filter.Outcome,
                ignoreCase: true
            );
        }

        IQueryable<SecurityEventLog> query = _securityEventLogRepository.GetSecurityEventLogsQueryable();

        // Apply the parsed enums to filter
        if (eventType.HasValue)
            query = query.Where(s => s.EventType == eventType.Value);

        if (outcome.HasValue)
            query = query.Where(s => s.Outcome == outcome.Value);

        if (!string.IsNullOrWhiteSpace(filter.UserId))
            query = query.Where(s => s.UserId == filter.UserId);
        
        if (!string.IsNullOrWhiteSpace(filter.UserEmail))
            query = query.Where(s => s.UserEmail == filter.UserEmail);
        
        if (filter.StartDate.HasValue)
            query = query.Where(s => s.Timestamp >= filter.StartDate.Value);
        
        if (filter.EndDate.HasValue)
            query = query.Where(s => s.Timestamp <= filter.EndDate.Value);

        // sort by timestamp desc by default
        query = query.ApplySorting(page.SortBy, page.SortDesc);

        // pagination
        var totalCount = await query.CountAsync(cancellationToken);

        var securityEventLogs = await query
            .Skip(page.Skip)
            .Take(page.EffectivePageSize)
            .ToListAsync(cancellationToken);
    
        var mappedSecurityEventLogs = _mapper.Map<List<SecurityEventLogDto>>(securityEventLogs);

        return new PagedResult<SecurityEventLogDto>(
            mappedSecurityEventLogs,
            totalCount,
            page.EffectivePage,
            page.EffectivePageSize);
    }
}