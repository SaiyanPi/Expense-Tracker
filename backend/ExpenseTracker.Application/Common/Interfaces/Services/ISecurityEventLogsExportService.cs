using ExpenseTracker.Application.DTOs.AuditLog;
using ExpenseTracker.Application.DTOs.SecurityEventLog;

namespace ExpenseTracker.Application.Common.Interfaces.Services;

public interface ISecurityEventLogsExportService
{
    byte[] ExportToCsv(IReadOnlyList<SecurityLogsExportDto> securityLogs);
    byte[] ExportToExcel(IReadOnlyList<SecurityLogsExportDto> securityLogs);
    byte[] ExportToPdf(IReadOnlyList<SecurityLogsExportDto> securityLogs);
    
}
