using AutoMapper;
using ExpenseTracker.Application.DTOs.AuditLog;
using ExpenseTracker.Application.DTOs.SecurityEventLog;
using ExpenseTracker.Domain.Entities;

namespace ExpenseTracker.Application.Common.Mappings;

public class SecurityEventLogMappingProfile : Profile
{
    public SecurityEventLogMappingProfile()
    {
        CreateMap<SecurityEventLog, SecurityEventLogDto>();
        
        CreateMap<SecurityEventLog, SecurityLogsExportDto>();
    }
    
}