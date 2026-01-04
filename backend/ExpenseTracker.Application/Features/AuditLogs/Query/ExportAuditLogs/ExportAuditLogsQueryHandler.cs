using AutoMapper;
using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Application.DTOs.AuditLog;
using ExpenseTracker.Application.DTOS.Expense;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
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
        // Validate userId if provided
        if (!string.IsNullOrWhiteSpace(request.Filter.UserId))
        {
            var user = await _userRepository.GetByIdAsync(request.Filter.UserId);
            if(user is null)
                throw new NotFoundException(nameof(User), request.Filter.UserId);
        }

        var filter = request.Filter;

        IQueryable<AuditLog> query = _auditLogRepository.GetAuditLogsQueryable();

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