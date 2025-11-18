using ExpenseTracker.Application.DTOs.Expense;
using ExpenseTracker.Application.Features.Expenses.Commands.CreateExpense;
using ExpenseTracker.Application.Features.Expenses.Commands.DeleteExpense;
using ExpenseTracker.Application.Features.Expenses.Commands.UpdateExpense;
using ExpenseTracker.Application.Features.Expenses.Queries.GetAllExpenses;
using ExpenseTracker.Application.Features.Expenses.Queries.GetAllExpensesByEmail;
using ExpenseTracker.Application.Features.Expenses.Queries.GetExpenseById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExpenseController : ControllerBase
{
    private readonly IMediator _mediator;

    public ExpenseController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET: api/expense
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        var query = new GetAllExpensesQuery();
        var expenses = await _mediator.Send(query, cancellationToken);
        return Ok(expenses);
    }

    // GET: api/expense/email?email={email}
    [HttpGet("email")]
    public async Task<IActionResult> GetAllByEmail(string email, CancellationToken cancellationToken = default)
    {
        var query = new GetAllExpensesByEmailQuery(email);
        var expenses = await _mediator.Send(query, cancellationToken);
        return Ok(expenses);
    }

    // GET: api/expense/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var query = new GetExpenseByIdQuery(id);
        var expense = await _mediator.Send(query, cancellationToken);
        return Ok(expense);
    }

    // POST: api/expense
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateExpenseDto dto, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var command = new CreateExpenseCommand(dto);
        var newExpense = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = newExpense.Id }, newExpense);
    }

    // PUT: api/expense/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateExpenseDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var command = new UpdateExpenseCommand(
            dto.Title,
            dto.Description,
            dto.Amount,
            dto.Date,
            dto.CategoryId
        )
        {
            Id = id
        };
        
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    // DELETE: api/expense/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteExpenseCommand(id);
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }
}