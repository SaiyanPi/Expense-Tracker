using ExpenseTracker.Application.Common.Authorization.Permissions;
using ExpenseTracker.Application.Common.Pagination;
using ExpenseTracker.Application.Features.AuditLogs.Query;
using ExpenseTracker.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.API.Controllers;

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
    //          &entityName=Expense&userId={userId}&action=1&from={date}&to={date}
    [Authorize(Policy = AuditLogPermission.View)]
    [HttpGet]
    public async Task<IActionResult> GetAuditLogs(
        [FromQuery] string? entityName = null,
        [FromQuery] string? userId = null,
        [FromQuery] AuditAction? action = null,
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

}