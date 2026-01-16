using ExpenseTracker.Application.Common.Authorization.Permissions;
using ExpenseTracker.Application.Common.Pagination;
using ExpenseTracker.Application.Features.SecurityEventLogs.Query.ExportSecurityEventLogs;
using ExpenseTracker.Application.Features.SecurityEventLogs.Query.GetAllSecurityEventLogs;
using ExpenseTracker.Application.Features.SecurityEventLogs.Query.GetSecurityEventLogById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.API.Controllers;

[Authorize(Policy = AuditLogPermission.View)]
[ApiController]
[Route("api/[controller]")]
public class SecurityEventLogController : ControllerBase
{
    private readonly IMediator _mediator;

    public SecurityEventLogController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET: api/securityEventLog?page=1&pageSize=20&sortBy=Timestamp&sortDesc=true&
    // userId=&userEmail=&outcome=&eventType=&startDate=&endDate=
    [HttpGet]
    public async Task<IActionResult> GetSecurityEventLogs(
        [FromQuery] string? eventType = null,
        [FromQuery] string? outcome = null,
        [FromQuery] string? userId = null,
        [FromQuery] string? userEmail = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 5,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDesc = false,
        CancellationToken cancellationToken = default)
    {

        var query = new GetAllSecurityEventLogsQuery(
            new SecurityEventLogFilter(eventType, outcome, userId, userEmail, startDate, endDate),
            new PagedQuery(page, pageSize, sortBy, sortDesc));

        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    // GET: api/securityEventLog/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetSecurityEventLogById(Guid id, CancellationToken cancellationToken = default)
    {
        var query = new GetSecurityEventLogByIdQuery(id);

        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    // GET: api/securityEventLog/export?format=entityName={enum or string}&userId={userId}&action={enum or string}
    // &from={date}&to={date}
    [HttpGet("export")]
    public async Task<IActionResult> ExportAuditLogs(
        [FromQuery] string format,
        [FromQuery] string? eventType = null,
        [FromQuery] string? outcome = null,
        [FromQuery] string? userId = null,
        [FromQuery] string? userEmail = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        CancellationToken cancellationToken = default)
    {
        var query = new ExportSecurityEventLogsQuery(
            format,
            new SecurityEventLogFilter(eventType, outcome, userId, userEmail, startDate, endDate));

        var exportResult = await _mediator.Send(query, cancellationToken);

        return File(
            exportResult.Content,
            exportResult.ContentType,
            exportResult.FileName);
    }
}