using MediatR;
using Microsoft.AspNetCore.Mvc;
using ExpenseTracker.Application.Features.Dashboard.Query;
using Microsoft.AspNetCore.Authorization;
using ExpenseTracker.Application.Common.Authorization.Permissions;
using ExpenseTracker.API.Contracts.V1.Dashboard;
using Microsoft.AspNetCore.RateLimiting;

namespace ExpenseTracker.API.Controllers.V1;

[EnableRateLimiting("Heavy")]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
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
        // return Ok(dashboardSummary);
        
        var response = new DashboardSummaryResponseV1
        {
            TotalExpenses = dashboardSummary.TotalExpenses,
            TotalBudgets = dashboardSummary.TotalBudgets,
            TopCategory = dashboardSummary.TopCategory != null
                ? new CategoryExpenseResponseV1
                {
                    Category = dashboardSummary.TopCategory.Category,
                    TotalAmount = dashboardSummary.TopCategory.TotalAmount
                }
                : null,
            RemainingBudget = dashboardSummary.RemainingBudget,
            ExpenseByCategory = dashboardSummary.ExpenseByCategory
                .Select(x => new CategoryExpenseResponseV1
                {
                    Category = x.Category,
                    TotalAmount = x.TotalAmount
                }).ToList(),
            DailyExpenses = dashboardSummary.DailyExpenses
                .Select(x => new DailyExpenseResponseV1
                {
                    Date = x.Date, 
                    TotalAmount = x.TotalAmount
                }).ToList(),
            RecentExpenses = dashboardSummary.RecentExpenses
                .Select(x => new RecentExpenseResponseV1
                {
                    Id = x.Id,
                    Title = x.Title,
                    Amount = x.Amount,
                    Date = x.Date
                }).ToList()
        };
    
        return Ok(response);
    }
    
}