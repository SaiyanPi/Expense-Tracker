using ExpenseTracker.Application.Interfaces.Services;
using ExpenseTrackler.Application.DTOs.Expense;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExpenseController : ControllerBase
{
    private readonly IExpenseService _expenseService;

    public ExpenseController(IExpenseService expenseService)
    {
        _expenseService = expenseService;
    }

    // GET: api/expense
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        var expenses = await _expenseService.GetAllAsync(cancellationToken);
        return Ok(expenses);
    }

    // GET: api/expense/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var expense = await _expenseService.GetByIdAsync(id, cancellationToken);
        if (expense == null)
        {
            return NotFound();
        }
        return Ok(expense);
    }

    // POST: api/expense
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateExpenseDto dto, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var newExpenseId = await _expenseService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = newExpenseId }, null);
    }

    // PUT: api/expense/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateExpenseDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _expenseService.UpdateAsync(id, dto, cancellationToken);
        return NoContent();
    }

    // DELETE: api/expense/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _expenseService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}