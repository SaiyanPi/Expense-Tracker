using AutoMapper;
using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Application.DTOs.SecurityEventLog;
using ExpenseTracker.Application.DTOS.FileExport;
using ExpenseTracker.Application.Features.SecurityEventLogs.Query.ExportSecurityEventLogs;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Application.Features.AuditLogs.Query.ExportAuditLogs;

public class ExportSecurityEventLogsQueryHandler
    : IRequestHandler<ExportSecurityEventLogsQuery, ExportFileResultDto>
{
    private readonly ISecurityEventLogsExportService _exportService;
    private readonly ISecurityEventLogRepository _securityEventLogRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public ExportSecurityEventLogsQueryHandler(
        ISecurityEventLogsExportService exportService,
        ISecurityEventLogRepository securityEventLogRepository,
        IUserRepository userRepository,
        IMapper mapper)
    {
        _exportService = exportService;
        _securityEventLogRepository = securityEventLogRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<ExportFileResultDto> Handle(
        ExportSecurityEventLogsQuery request,
        CancellationToken cancellationToken)
    {
        var filter = request.Filter;

        // Validate userId if provided
        if (!string.IsNullOrWhiteSpace(filter.UserId))
        {
            var user = await _userRepository.GetByIdAsync(filter.UserId);
            if(user is null)
                throw new NotFoundException(nameof(User), filter.UserId);
        }

        // Validate userEmail if provided
        if (!string.IsNullOrWhiteSpace(filter.UserEmail))
        {
            var user = await _userRepository.GetByEmailAsync(filter.UserEmail);
            if(user is null)
                throw new NotFoundException(nameof(User), filter.UserEmail);
        }

        // parse the enum: 
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

        // use the parsed value in the query
        if (eventType.HasValue)
            query = query.Where(x => x.EventType == eventType.Value);

        if (outcome.HasValue)
            query = query.Where(x => x.Outcome == outcome.Value);
            
        if (!string.IsNullOrWhiteSpace(filter.UserId))
            query = query.Where(x => x.UserId == filter.UserId);

        if (!string.IsNullOrWhiteSpace(filter.UserEmail))
            query = query.Where(s => s.UserEmail == filter.UserEmail);
        
        if (filter.StartDate.HasValue)
            query = query.Where(s => s.Timestamp >= filter.StartDate.Value);
        
        if (filter.EndDate.HasValue)
            query = query.Where(s => s.Timestamp <= filter.EndDate.Value);

        var filteredSecurityLogs = await query
            .ToListAsync(cancellationToken);
        
        var mappedSecurityLogs = _mapper.Map<IReadOnlyList<SecurityLogsExportDto>>(filteredSecurityLogs);
     
        
         return request.Format.ToLower() switch
        {
            "csv" => new ExportFileResultDto
            {
                Content = _exportService.ExportToCsv(mappedSecurityLogs),
                ContentType = "text/csv",
                FileName = $"securityLogs_{DateTime.Now:yyyyMMdd_HHmm}.csv"
            },

            "excel" or "xlsx" => new ExportFileResultDto
            {
                Content = _exportService.ExportToExcel(mappedSecurityLogs),
                ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                FileName = $"securityLogs_{DateTime.Now:yyyyMMdd_HHmm}.xlsx"
            },

            "pdf" => new ExportFileResultDto
            {
                Content = _exportService.ExportToPdf(mappedSecurityLogs),
                ContentType = "application/pdf",
                FileName = $"securityLogs_{DateTime.Now:yyyyMMdd_HHmm}.pdf"
            },

            _ => throw new ValidationException("Unsupported export format")
        };
    
    }
}