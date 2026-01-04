using ExpenseTracker.Application.DTOs.AuditLog;

namespace ExpenseTracker.Application.Common.Interfaces.Services;

public interface IAuditLogsExportService
{
    byte[] ExportToCsv(IReadOnlyList<AuditLogsExportDto> auditLogs);
    byte[] ExportToExcel(IReadOnlyList<AuditLogsExportDto> auditLogs);
    byte[] ExportToPdf(IReadOnlyList<AuditLogsExportDto> auditLogs);
    
}
