using ExpenseTracker.Application.DTOs.Expense;
using ExpenseTracker.Application.Features.Expenses.Commands.CreateExpense;
using ExpenseTracker.Application.Features.Expenses.Commands.DeleteExpense;
using ExpenseTracker.Application.Features.Expenses.Commands.UpdateExpense;
using ExpenseTracker.Application.Features.Expenses.GetTotalExpenses;
using ExpenseTracker.Application.Features.Expenses.Queries.FilterExpenses;
using ExpenseTracker.Application.Features.Expenses.Queries.GetAllExpenses;
using ExpenseTracker.Application.Features.Expenses.Queries.GetAllExpensesByEmail;
using ExpenseTracker.Application.Features.Expenses.Queries.GetCategorySummary;
using ExpenseTracker.Application.Features.Expenses.Queries.GetCategorySummaryByEmail;
using ExpenseTracker.Application.Features.Expenses.Queries.GetExpenseById;
using ExpenseTracker.Application.Features.Expenses.Queries.GetTotalExpensesByEmail;
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
        return Ok(new {Success = true, Message = "Expense updated successfully" }); 
    }

    // DELETE: api/expense/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteExpenseCommand(id);
        await _mediator.Send(command, cancellationToken);
        return Ok(new {Success = true, Message = "Expense deleted successfully" });    
    }

    // GET: api/expense/total
    [HttpGet("total")]
    public async Task<IActionResult> GetTotalExpenses(CancellationToken cancellationToken = default)
    {
        var query = new GetTotalExpensesQuery();
        var totalExpenses = await _mediator.Send(query, cancellationToken);
        return Ok(totalExpenses);
    }

    // GET: api/expense/total-expenses/email?email=email
    [HttpGet("total-expenses/email")]
    public async Task<IActionResult> GetTotalExpensesByEmail(string email, CancellationToken cancellationToken = default)
    {
        var query = new GetTotalExpensesByEmailQuery(email);
        var totalExpensesByEmail = await _mediator.Send(query, cancellationToken);
        return Ok(totalExpensesByEmail);
    }

    // GET: api/expense/category-summary
    [HttpGet("category-summary")]
    public async Task<IActionResult> GetCategorySummary(CancellationToken cancellationToken = default)
    {
        var query = new GetCategorySummaryQuery();
        var summary = await _mediator.Send(query, cancellationToken);
        return Ok(summary);
    }

    // GET: api/expense/category-summary/email?email={email}
    [HttpGet("category-summary/email")]
    public async Task<IActionResult> GetCategorySummaryByEmail(string email, CancellationToken cancellationToken = default)
    {
        var query = new GetCategorySummaryByEmailQuery(email);
        var summary = await _mediator.Send(query, cancellationToken);
        return Ok(summary);
    }

    // GET: api/expense/filter?startDate=&endDate=&minAmount=&maxAmount=&categoryId=&userId=
    [HttpGet("filter")]
    public async Task<IActionResult> FilterExpenses(
                                                    [FromQuery] DateTime? startDate,
                                                    [FromQuery] DateTime? endDate,
                                                    [FromQuery] decimal? minAmount,
                                                    [FromQuery] decimal? maxAmount,
                                                    [FromQuery] Guid? categoryId,
                                                    [FromQuery] string? userId,
                                                    CancellationToken cancellationToken = default
                                                    )
    {
        var query = new FilterExpensesQuery(
            startDate,
            endDate,
            minAmount,
            maxAmount,
            categoryId,
            userId
        );

        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}