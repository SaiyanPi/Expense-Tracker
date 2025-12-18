using ExpenseTracker.Application.Common.Pagination;
using ExpenseTracker.Application.DTOs.Expense;
using ExpenseTracker.Application.Features.Expenses.Commands.CreateExpense;
using ExpenseTracker.Application.Features.Expenses.Commands.DeleteExpense;
using ExpenseTracker.Application.Features.Expenses.Commands.UpdateExpense;
using ExpenseTracker.Application.Features.Expenses.GetTotalExpenses;
using ExpenseTracker.Application.Features.Expenses.Queries.ExportExpenses;
using ExpenseTracker.Application.Features.Expenses.Queries.FilterExpenses;
using ExpenseTracker.Application.Features.Expenses.Queries.GetAllExpenses;
using ExpenseTracker.Application.Features.Expenses.Queries.GetAllExpensesByEmail;
using ExpenseTracker.Application.Features.Expenses.Queries.GetAllExpensesForABudgetByEmail;
using ExpenseTracker.Application.Features.Expenses.Queries.GetAllExpensesForCategoryByEmail;
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
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 5,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDesc = false,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllExpensesQuery(new PagedQuery(page, pageSize, sortBy, sortDesc));
        var expenses = await _mediator.Send(query, cancellationToken);
        return Ok(expenses);
    }

    // GET: api/expense/email?email={email}
    [HttpGet("email")]
    public async Task<IActionResult> GetExpensesByEmail(
        [FromQuery] string email,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 5,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDesc = false,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllExpensesByEmailQuery(email, new PagedQuery(page, pageSize, sortBy, sortDesc));
        var expensesByEmail = await _mediator.Send(query, cancellationToken);
        return Ok(expensesByEmail);
    }

    // GET: api/expense/budget-expenses?budgetId={budgetId}&email={email}
    [HttpGet("budget-expenses")]
    public async Task<IActionResult> GetExpensesForABudgetByEmail(
        [FromQuery] Guid budgetId,
        [FromQuery] string email,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 5, 
        [FromQuery] string? sortBy = null, 
        [FromQuery] bool sortDesc = false,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllExpensesForABudgetByEmailQuery(budgetId, email, new PagedQuery(page, pageSize, sortBy, sortDesc));
        var expensesForBudget = await _mediator.Send(query, cancellationToken);
        return Ok(expensesForBudget);
    }

    // GET: api/expense/category-expenses?categoryId={categoryId}&email={email}
    [HttpGet("category-expenses")]
    public async Task<IActionResult> GetExpensesForCategoryByEmail(
        [FromQuery] Guid categoryId,
        [FromQuery] string email,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 5, 
        [FromQuery] string? sortBy = null, 
        [FromQuery] bool sortDesc = false,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllExpensesForCategoryByEmailQuery(categoryId, email, new PagedQuery(page, pageSize, sortBy, sortDesc));
        var expensesForCategory = await _mediator.Send(query, cancellationToken);
        return Ok(expensesForCategory);
    }

    // GET: api/expense/category-summary
    [HttpGet("category-summary")]
    public async Task<IActionResult> GetCategorySummary(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 5,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDesc = false,
        CancellationToken cancellationToken = default)
    {
        var query = new GetCategorySummaryQuery(new PagedQuery(page, pageSize, sortBy, sortDesc));
        var categorySummary = await _mediator.Send(query, cancellationToken);
        return Ok(categorySummary);
    }

    // GET: api/expense/category-summary/email?email={email}
    [HttpGet("category-summary/email")]
    public async Task<IActionResult> GetCategorySummaryByEmail(
        [FromQuery] string email,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 5,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDesc = false,
        CancellationToken cancellationToken = default)
    {
        var query = new GetCategorySummaryByEmailQuery(email, new PagedQuery(page, pageSize, sortBy, sortDesc));
        var categorySummaryByEmail = await _mediator.Send(query, cancellationToken);
        return Ok(categorySummaryByEmail);
    }

    // GET: api/expense/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var query = new GetExpenseByIdQuery(id);
        var expense = await _mediator.Send(query, cancellationToken);
        return Ok(expense);
    }

    // GET: api/expense/total
    [HttpGet("total")]
    public async Task<IActionResult> GetTotalExpense(CancellationToken cancellationToken = default)
    {
        var query = new GetTotalExpensesQuery();
        var totalExpenses = await _mediator.Send(query, cancellationToken);
        return Ok(totalExpenses);
    }

    // GET: api/expense/total-expense/email?email=email
    [HttpGet("total-expense/email")]
    public async Task<IActionResult> GetTotalExpenseByEmail(string email, CancellationToken cancellationToken = default)
    {
        var query = new GetTotalExpenseByEmailQuery(email);
        var totalExpensesByEmail = await _mediator.Send(query, cancellationToken);
        return Ok(totalExpensesByEmail);
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

        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 5,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDesc = false,
        CancellationToken cancellationToken = default)
    {
        var query = new FilterExpensesQuery(
            startDate,
            endDate,
            minAmount,
            maxAmount,
            categoryId,
            userId,

            new PagedQuery(page, pageSize, sortBy, sortDesc));

        var filteredExpenses = await _mediator.Send(query, cancellationToken);
        return Ok(filteredExpenses);
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
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateExpenseDto dto, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var command = new UpdateExpenseCommand(
            dto.Title,
            dto.Description,
            dto.Amount,
            dto.Date,
            dto.CategoryId,
            dto.BudgetId
        )
        {
            Id = id
        };
        
        await _mediator.Send(command, cancellationToken);
        return Ok(new {Success = true, Message = "Expense updated successfully" }); 
    }

    // DELETE: api/expense/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        var command = new DeleteExpenseCommand(id);
        await _mediator.Send(command, cancellationToken);
        return Ok(new {Success = true, Message = "Expense deleted successfully" });    
    }

    // GET: api/expense/export?userId={userId}&format={format}
    [HttpGet("export")]
    public async Task<IActionResult> ExportExpenses(
        [FromQuery] string userId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        [FromQuery] string format,
        CancellationToken cancellationToken = default)
    {
        var query = new ExportExpensesQuery(userId, startDate, endDate, format);

        var exportResult = await _mediator.Send(query, cancellationToken);

        // return Ok(exportResult); // doesn't download the file

        return File(
            exportResult.Content,
            exportResult.ContentType,
            exportResult.FileName);
    }
}