using MediatR;
using Microsoft.AspNetCore.Mvc;
using ExpenseTracker.Application.Features.Dashboard.Query;
using Microsoft.AspNetCore.Authorization;
using ExpenseTracker.Application.Common.Authorization.Permissions;

namespace ExpenseTracker.API.Controllers;   

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IMediator _mediator;

    public DashboardController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // api/dashboard
    [Authorize(Policy = DashboardPermission.View)]
    [HttpGet("dashboard")]
    public async Task<IActionResult> GetMonthlyDashboard(CancellationToken cancellationToken = default)
    {
        var query = new GetMonthlyDashboardQuery();
        var dashboardSummary = await _mediator.Send(query, cancellationToken);
        return Ok(dashboardSummary);
    }
    
}