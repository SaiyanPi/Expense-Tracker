using ExpenseTracker.Application.Common.Pagination;
using ExpenseTracker.Application.DTOS.Expense;
using MediatR;

namespace ExpenseTracker.Application.Features.AuditLogs.Query.ExportAuditLogs;

public record ExportAuditLogsQuery(
    string Format, AuditLogFilter Filter) : IRequest<ExportFileResultDto>;