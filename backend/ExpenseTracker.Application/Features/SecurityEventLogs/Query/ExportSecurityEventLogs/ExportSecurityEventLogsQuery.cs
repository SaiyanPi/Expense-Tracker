using ExpenseTracker.Application.Common.Pagination;
using ExpenseTracker.Application.DTOS.Expense;
using MediatR;

namespace ExpenseTracker.Application.Features.SecurityEventLogs.Query.ExportSecurityEventLogs;

public record ExportSecurityEventLogsQuery(
    string Format, SecurityEventLogFilter Filter) : IRequest<ExportFileResultDto>;