using MediatR;
using Microsoft.AspNetCore.Mvc;
using ExpenseTracker.Application.Features.Budgets.Commands.CreateBudget;
using ExpenseTracker.Application.DTOs.Budget;
using ExpenseTracker.Application.Features.Budgets.Queries.GetBudgetById;
using ExpenseTracker.Application.Features.Budgets.Queries.GetAllBudgets;
using ExpenseTracker.Application.Features.Budgets.Commands.UpdateBudget;
using ExpenseTracker.Application.Features.Budgets.Commands.DeleteBudget;
using ExpenseTracker.Application.Features.Budgets.Queries.GetAllBudgetsByEmail;
using ExpenseTracker.Application.Features.Budgets.Queries.GetBudgetsSummaryByEmail;
using ExpenseTracker.Application.Features.Budgets.Queries.GetBudgetDetailWithExpensesByEmail;
using ExpenseTracker.Application.Common.Pagination;
using Microsoft.AspNetCore.Authorization;
using ExpenseTracker.Application.Common.Authorization.Permissions;

namespace ExpenseTracker.API.Controllers;   

[ApiController]
[Route("api/[controller]")]
public class BudgetController : ControllerBase
{
    private readonly IMediator _mediator;

    public BudgetController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    // GET: api/Budget
    [Authorize(Policy = BudgetPermission.ViewAll)]
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 5,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDesc = false,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllBudgetQuery(new PagedQuery(page, pageSize, sortBy, sortDesc));
        var budgets = await _mediator.Send(query, cancellationToken);
        return Ok(budgets);
    }

    // GET: api/Budget/my
    [Authorize(Policy = BudgetPermission.View)]
    [HttpGet("my")]
    public async Task<IActionResult> GetAllBudgetsByEmail(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 5,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDesc = false,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllBudgetsByEmailQuery(new PagedQuery(page, pageSize, sortBy, sortDesc));
        var budgets = await _mediator.Send(query, cancellationToken);
        return Ok(budgets);
    }
    
    // GET: api/Budget/{id}
    [Authorize(Policy = BudgetPermission.View)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetBudgetById(Guid id, CancellationToken cancellationToken = default)
    {
        var query = new GetBudgetByIdQuery(id);
        var budget = await _mediator.Send(query, cancellationToken);
        return Ok(budget);
    }

    // GET: api/budget/budget-detail-with-expenses?budgetId={budgetId}
    [Authorize(Policy = BudgetPermission.View)]
    [HttpGet("budget-detail-with-expenses")]
    public async Task<IActionResult> GetBudgetDetailWithExpensesByEmail(
        [FromQuery] Guid budgetId,

        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 5,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDesc = false,
        CancellationToken cancellationToken = default)
    {
        var query = new GetBudgetDetailWithExpensesByEmailQuery(
            budgetId,
            new PagedQuery(page, pageSize, sortBy, sortDesc));

        var budgetDetailWithExpensesByEmail = await _mediator.Send(query, cancellationToken);
        return Ok(budgetDetailWithExpensesByEmail);
    }

    
    // GET: api/budget/budget-summary/email?email={email}
    [Authorize(Policy = BudgetPermission.View)]
    [HttpGet("budget-summary/email")]
    public async Task<IActionResult> GetBudgetSummaryByEmail(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 5,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDesc = false,
        CancellationToken cancellationToken = default)
    {
        var query = new GetBudgetsSummaryByEmailQuery(new PagedQuery(page, pageSize, sortBy, sortDesc));
        var budgetsSummary = await _mediator.Send(query, cancellationToken);
        return Ok(budgetsSummary);
    }

    // POST: api/Budget
    [Authorize(Policy = BudgetPermission.Create)]
    [HttpPost]
    public async Task<IActionResult> CreateBudget([FromBody] CreateBudgetDto createBudgetDto, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
            
        var command = new CreateBudgetCommand(createBudgetDto);
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetBudgetById), new { id = result.Id }, result);
    }

    
    // PUT: api/budget/{id}
    [Authorize(Policy = BudgetPermission.Update)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateBudget(Guid id, [FromBody] UpdateBudgetDto dto, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var command = new UpdateBudgetCommand(
            dto.Name, 
            dto.Amount, 
            dto.StartDate, 
            dto.EndDate, 
            dto.CategoryId
        )
        {
            Id = id
        };
        
        await _mediator.Send(command, cancellationToken);
        return Ok(new {Success = true, Message = "Budget updated successfully" }); 
    }

    // DELETE: api/budget/{id}
    [Authorize(Policy = BudgetPermission.Delete)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteBudget(Guid id, CancellationToken cancellationToken = default)
    {
        var command = new DeleteBudgetCommand(id);
        await _mediator.Send(command, cancellationToken);
        return Ok(new {Success = true, Message = "Budget deleted successfully" }); 
    }

}