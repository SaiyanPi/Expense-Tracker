using MediatR;
using Microsoft.AspNetCore.Mvc;
using ExpenseTracker.Application.Features.Budgets.Commands.CreateBudget;
using ExpenseTracker.Application.DTOs.Budget;
using ExpenseTracker.Application.Features.Budgets.Queries.GetBudgetById;
using ExpenseTracker.Application.Features.Budgets.Queries.GetAllBudgets;
using ExpenseTracker.Application.Features.Budgets.Commands.UpdateBudget;
using ExpenseTracker.Application.Features.Budgets.Commands.DeleteBudget;
using ExpenseTracker.Application.Features.Budgets.Queries.GetAllBudgetsByEmail;
using ExpenseTracker.Application.Features.Budgets.Queries.GetBudgetSummaryByEmail;
using ExpenseTracker.Application.Features.Budgets.Queries.GetBudgetDetailWithExpensesByEmail;

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
    [HttpGet]
    public async Task<IActionResult> GetAllBudgets(CancellationToken cancellationToken)
    {
        var query = new GetAllBudgetQuery();
        var budgets = await _mediator.Send(query, cancellationToken);
        return Ok(budgets);
    }

    // GET: api/Budget/email?email={email}
    [HttpGet("email")]
    public async Task<IActionResult> GetAllBudgetsByEmail(string email, CancellationToken cancellationToken)
    {
        var query = new GetAllBudgetsByEmailQuery(email);
        var budgets = await _mediator.Send(query, cancellationToken);
        return Ok(budgets);
    }
    
    // GET: api/Budget/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetBudgetById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetBudgetByIdQuery(id);
        var budget = await _mediator.Send(query, cancellationToken);
        return Ok(budget);
    }

    // POST: api/Budget
    [HttpPost]
    public async Task<IActionResult> CreateBudget([FromBody] CreateBudgetDto createBudgetDto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
            
        var command = new CreateBudgetCommand(createBudgetDto);
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetBudgetById), new { id = result.Id }, result);
    }

    
    // PUT: api/budget/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateBudget(Guid id, [FromBody] UpdateBudgetDto dto, CancellationToken cancellationToken)
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
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteBudget(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteBudgetCommand(id);
        await _mediator.Send(command, cancellationToken);
        return Ok(new {Success = true, Message = "Budget deleted successfully" }); 
    }

    // GET: api/budget/budget-summary/email?email={email}
    [HttpGet("budget-summary/email")]
    public async Task<IActionResult> GetBudgetSummaryByEmail([FromQuery] string email, CancellationToken cancellationToken = default)
    {
        var query = new GetBudgetSummaryByEmailQuery(email);
        var summary = await _mediator.Send(query, cancellationToken);
        return Ok(summary);
    }

    // GET: api/budget/budget-detail-with-expenses?budgetId={budgetId}&email={email}
    [HttpGet("budget-detail-with-expenses")]
    public async Task<IActionResult> GetBudgetDetailWithExpensesByEmail([FromQuery] Guid budgetId, [FromQuery] string email, CancellationToken cancellationToken = default)
    {
        var query = new GetBudgetDetailWithExpensesByEmailQuery(budgetId, email);
        var budgetDetailWithExpensesByEmail = await _mediator.Send(query, cancellationToken);
        return Ok(budgetDetailWithExpensesByEmail);
    }

    
}