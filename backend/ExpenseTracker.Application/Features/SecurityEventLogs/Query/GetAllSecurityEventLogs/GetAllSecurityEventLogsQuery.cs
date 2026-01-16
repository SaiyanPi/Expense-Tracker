using ExpenseTracker.Application.Common.Pagination;
using ExpenseTracker.Application.DTOs.SecurityEventLog;
using MediatR;

namespace ExpenseTracker.Application.Features.SecurityEventLogs.Query.GetAllSecurityEventLogs;

public record GetAllSecurityEventLogsQuery(SecurityEventLogFilter Filter, PagedQuery Paging) 
    : IRequest<PagedResult<SecurityEventLogDto>>
{
}