using AutoMapper;
using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Application.DTOs.AuditLog;
using ExpenseTracker.Application.DTOS.FileExport;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Application.Features.AuditLogs.Query.ExportAuditLogs;

public class ExportAuditLogsQueryHandler
    : IRequestHandler<ExportAuditLogsQuery, ExportFileResultDto>
{
    private readonly IAuditLogsExportService _exportService;
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public ExportAuditLogsQueryHandler(
        IAuditLogsExportService exportService,
        IAuditLogRepository auditLogRepository,
        IUserRepository userRepository,
        IMapper mapper)
    {
        _exportService = exportService;
        _auditLogRepository = auditLogRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<ExportFileResultDto> Handle(
        ExportAuditLogsQuery request,
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

        // use the parsed value in the query
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

        var filteredAuditLogs = await query
            .ToListAsync(cancellationToken);
        
        var mappedAuditLogs = _mapper.Map<IReadOnlyList<AuditLogsExportDto>>(filteredAuditLogs);
     
        
         return request.Format.ToLower() switch
        {
            "csv" => new ExportFileResultDto
            {
                Content = _exportService.ExportToCsv(mappedAuditLogs),
                ContentType = "text/csv",
                FileName = $"auditLogs_{DateTime.Now:yyyyMMdd_HHmm}.csv"
            },

            "excel" or "xlsx" => new ExportFileResultDto
            {
                Content = _exportService.ExportToExcel(mappedAuditLogs),
                ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                FileName = $"auditLogs_{DateTime.Now:yyyyMMdd_HHmm}.xlsx"
            },

            "pdf" => new ExportFileResultDto
            {
                Content = _exportService.ExportToPdf(mappedAuditLogs),
                ContentType = "application/pdf",
                FileName = $"auditLogs_{DateTime.Now:yyyyMMdd_HHmm}.pdf"
            },

            _ => throw new ValidationException("Unsupported export format")
        };
    
    }
}