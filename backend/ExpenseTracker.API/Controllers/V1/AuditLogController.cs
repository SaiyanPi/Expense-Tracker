using ExpenseTracker.Application.Common.Authorization.Permissions;
using ExpenseTracker.Application.Common.Pagination;
using ExpenseTracker.Application.Features.AuditLogs.Query.ExportAuditLogs;
using ExpenseTracker.Application.Features.AuditLogs.Query.GetAllAuditLogs;
using ExpenseTracker.Application.Features.AuditLogs.Query.GetAuditLogById;
using ExpenseTracker.Application.Features.AuditLogs.Query.GetAuditTimelineByEntityNameAndEntityId;
using ExpenseTracker.Application.Features.AuditLogs.Query.GetAuditTimelineByUserId;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.API.Controllers.V1;

[Authorize(Policy = AuditLogPermission.View)]
[ApiController]
[Route("api/[controller]")]
public class AuditLogController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuditLogController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET: api/auditLog?page=1&pageSize=7&sortBy=Date&sortDesc=true
    //          &entityName={enum or string}&userId={userId}&action={enum or string}&from={date}&to={date}
    [HttpGet]
    public async Task<IActionResult> GetAuditLogs(
        [FromQuery] string? entityName = null,
        [FromQuery] string? userId = null,
        [FromQuery] string? action = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 5,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDesc = false,
        CancellationToken cancellationToken = default)
    {

        var query = new GetAuditLogsQuery(
            new AuditLogFilter(entityName, userId, action, startDate, endDate),
            new PagedQuery(page, pageSize, sortBy, sortDesc));

        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    // GET: api/auditLog/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetAuditLogById(Guid id, CancellationToken cancellationToken = default)
    {
        var query = new GetAuditLogByIdQuery(id);

        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    // GET: api/auditLog/timeline/entity/timeline/{enum or string}/{entityId}
    [HttpGet("timeline/entity/{entityName}/{entityId}")]
    public async Task<IActionResult> GetAuditTimelineByEntityNameAndId(
        string entityName,
        Guid entityId, 
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 5,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDesc = false,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAuditTimelineByEntityNameAndIdQuery(entityName, entityId, 
            new PagedQuery(page, pageSize, sortBy, sortDesc));

        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    // GET: api/auditLog/timeline/{userId}
    [HttpGet("timeline/{userId}")]
    public async Task<IActionResult> GetAuditTimelineByUserId(
        string userId,
        
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 5,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDesc = false,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAuditTimelineByUserIdQuery(userId, 
            new PagedQuery(page, pageSize, sortBy, sortDesc));

        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    // GET: api/auditLog/export?format=entityName={enum or string}&userId={userId}&action={enum or string}
    // &from={date}&to={date}
    [HttpGet("export")]
    public async Task<IActionResult> ExportAuditLogs(
        [FromQuery] string format,
        [FromQuery] string? entityName = null,
        [FromQuery] string? userId = null,
        [FromQuery] string? action = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        CancellationToken cancellationToken = default)
    {
        var query = new ExportAuditLogsQuery(
            format,
            new AuditLogFilter(entityName, userId, action, startDate, endDate));

        var exportResult = await _mediator.Send(query, cancellationToken);

        return File(
            exportResult.Content,
            exportResult.ContentType,
            exportResult.FileName);
    }

}
