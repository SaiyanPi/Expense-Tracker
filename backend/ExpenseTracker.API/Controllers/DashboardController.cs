using MediatR;
using Microsoft.AspNetCore.Mvc;
using ExpenseTracker.Application.Features.Dashboard.Query;

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

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetMonthlyDashboard(
        [FromQuery]string userId,
        CancellationToken cancellationToken = default)
    {
        var query = new GetMonthlyDashboardQuery(userId);
        var dashboardSummary = await _mediator.Send(query, cancellationToken);
        return Ok(dashboardSummary);
    }
    
}