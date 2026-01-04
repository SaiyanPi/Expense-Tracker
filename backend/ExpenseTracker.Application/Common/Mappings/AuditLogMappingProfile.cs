using AutoMapper;
using ExpenseTracker.Application.DTOs.AuditLog;
using ExpenseTracker.Domain.Entities;

namespace ExpenseTracker.Application.Common.Mappings;

public class AuditLogMappingProfile : Profile
{
    public AuditLogMappingProfile()
    {
        CreateMap<AuditLog, AuditLogDto>();
        
        CreateMap<AuditLog, AuditLogsExportDto>();
    }
    
}