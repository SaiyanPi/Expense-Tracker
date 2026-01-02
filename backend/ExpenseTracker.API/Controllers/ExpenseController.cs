using ExpenseTracker.Application.Common.Authorization.Permissions;
using ExpenseTracker.Application.Common.Pagination;
using ExpenseTracker.Application.DTOs.Expense;
using ExpenseTracker.Application.Features.Expenses.Commands.CreateExpense;
using ExpenseTracker.Application.Features.Expenses.Commands.DeleteExpense;
using ExpenseTracker.Application.Features.Expenses.Commands.RestoreDeletedExpenseById;
using ExpenseTracker.Application.Features.Expenses.Commands.UpdateExpense;
using ExpenseTracker.Application.Features.Expenses.GetTotalExpenses;
using ExpenseTracker.Application.Features.Expenses.Queries.ExportExpenses;
using ExpenseTracker.Application.Features.Expenses.Queries.FilterExpenses;
using ExpenseTracker.Application.Features.Expenses.Queries.GetAllDeletedExpensesByEmail;
using ExpenseTracker.Application.Features.Expenses.Queries.GetAllExpenses;
using ExpenseTracker.Application.Features.Expenses.Queries.GetAllExpensesByEmail;
using ExpenseTracker.Application.Features.Expenses.Queries.GetAllExpensesForABudgetByEmail;
using ExpenseTracker.Application.Features.Expenses.Queries.GetAllExpensesForCategoryByEmail;
using ExpenseTracker.Application.Features.Expenses.Queries.GetCategorySummary;
using ExpenseTracker.Application.Features.Expenses.Queries.GetCategorySummaryByEmail;
using ExpenseTracker.Application.Features.Expenses.Queries.GetDeletedExpenseById;
using ExpenseTracker.Application.Features.Expenses.Queries.GetExpenseById;
using ExpenseTracker.Application.Features.Expenses.Queries.GetTotalExpensesByEmail;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Policy = ExpensePermission.ViewAll)]
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

    // GET: api/expense/my
    [Authorize(Policy = ExpensePermission.View)]
    [HttpGet("my")]
    public async Task<IActionResult> GetExpensesByEmail(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 5,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDesc = false,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllExpensesByEmailQuery(new PagedQuery(page, pageSize, sortBy, sortDesc));
        var expensesByEmail = await _mediator.Send(query, cancellationToken);
        return Ok(expensesByEmail);
    }

    // GET: api/expense/budget-expenses/my?budgetId={budgetId}
    [Authorize(Policy = ExpensePermission.View)]
    [HttpGet("budget-expenses/my")]
    public async Task<IActionResult> GetExpensesForABudgetByEmail(
        [FromQuery] Guid budgetId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 5, 
        [FromQuery] string? sortBy = null, 
        [FromQuery] bool sortDesc = false,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllExpensesForABudgetByEmailQuery(budgetId, new PagedQuery(page, pageSize, sortBy, sortDesc));
        var expensesForBudget = await _mediator.Send(query, cancellationToken);
        return Ok(expensesForBudget);
    }

    // GET: api/expense/category-expenses/my?categoryId={categoryId}
    [Authorize(Policy = ExpensePermission.View)]
    [HttpGet("category-expenses/my")]
    public async Task<IActionResult> GetExpensesForCategoryByEmail(
        [FromQuery] Guid categoryId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 5, 
        [FromQuery] string? sortBy = null, 
        [FromQuery] bool sortDesc = false,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllExpensesForCategoryByEmailQuery(categoryId, new PagedQuery(page, pageSize, sortBy, sortDesc));
        var expensesForCategory = await _mediator.Send(query, cancellationToken);
        return Ok(expensesForCategory);
    }

    // GET: api/expense/category-summary
    [Authorize(Policy = ExpensePermission.ViewAll)]
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

    // GET: api/expense/category-summary/my
    [Authorize(Policy = ExpensePermission.View)]
    [HttpGet("category-summary/my")]
    public async Task<IActionResult> GetCategorySummaryByEmail(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 5,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDesc = false,
        CancellationToken cancellationToken = default)
    {
        var query = new GetCategorySummaryByEmailQuery(new PagedQuery(page, pageSize, sortBy, sortDesc));
        var categorySummaryByEmail = await _mediator.Send(query, cancellationToken);
        return Ok(categorySummaryByEmail);
    }

    // GET: api/expense/{id}
    [Authorize(Policy = ExpensePermission.View)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var query = new GetExpenseByIdQuery(id);
        var expense = await _mediator.Send(query, cancellationToken);
        return Ok(expense);
    }

    // GET: api/expense/total
    [Authorize(Policy = ExpensePermission.ViewAll)]
    [HttpGet("total")]
    public async Task<IActionResult> GetTotalExpense(CancellationToken cancellationToken = default)
    {
        var query = new GetTotalExpensesQuery();
        var totalExpenses = await _mediator.Send(query, cancellationToken);
        return Ok(totalExpenses);
    }

    // GET: api/expense/total-expense/my
    [Authorize(Policy = ExpensePermission.View)]
    [HttpGet("total-expense/my")]
    public async Task<IActionResult> GetTotalExpenseByEmail(CancellationToken cancellationToken = default)
    {
        var query = new GetTotalExpenseByEmailQuery();
        var totalExpensesByEmail = await _mediator.Send(query, cancellationToken);
        return Ok(totalExpensesByEmail);
    }

    // // GET: api/expense/filter?startDate=&endDate=&minAmount=&maxAmount=&categoryId=&userId=
    // [Authorize(Policy = "Expense.Filter")]
    // [HttpGet("filter")]
    // public async Task<IActionResult> FilterExpenses(
    //     [FromQuery] DateTime? startDate,
    //     [FromQuery] DateTime? endDate,
    //     [FromQuery] decimal? minAmount,
    //     [FromQuery] decimal? maxAmount,
    //     [FromQuery] Guid? categoryId,
    //     [FromQuery] string? userId,

    //     [FromQuery] int page = 1,
    //     [FromQuery] int pageSize = 5,
    //     [FromQuery] string? sortBy = null,
    //     [FromQuery] bool sortDesc = false,
    //     CancellationToken cancellationToken = default)
    // {
    //     var query = new FilterExpensesQuery(
    //         startDate,
    //         endDate,
    //         minAmount,
    //         maxAmount,
    //         categoryId,
    //         userId,

    //         new PagedQuery(page, pageSize, sortBy, sortDesc));

    //     var filteredExpenses = await _mediator.Send(query, cancellationToken);
    //     return Ok(filteredExpenses);
    // }

    // GET: api/expense/filter?startDate=&endDate=&minAmount=&maxAmount=&categoryId=&userId=
    [Authorize(Policy = "Expense.Filter")]
    [HttpGet("filter")]
    public async Task<IActionResult> FilterExpenses(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] decimal? minAmount = null,
        [FromQuery] decimal? maxAmount = null,
        [FromQuery] Guid? categoryId = null,
        [FromQuery] string? userId = null,

        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 5,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDesc = false,
        CancellationToken cancellationToken = default)
    {
        var query = new FilterExpensesQuery(
            new ExpenseFilter(startDate, endDate, minAmount, maxAmount, categoryId, userId),
            new PagedQuery(page, pageSize, sortBy, sortDesc));

        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }


    // POST: api/expense
    [Authorize(Policy = ExpensePermission.Create)]
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
    [Authorize(Policy = ExpensePermission.Update)]
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
    [Authorize(Policy = ExpensePermission.Delete)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        var command = new DeleteExpenseCommand(id);
        await _mediator.Send(command, cancellationToken);
        return Ok(new {Success = true, Message = "Expense deleted successfully" });    
    }

    // GET: api/expense/export?format={format}
    [Authorize(Policy = ExpensePermission.View)]
    [HttpGet("export")]
    public async Task<IActionResult> ExportExpenses(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        [FromQuery] string format,
        CancellationToken cancellationToken = default)
    {
        var query = new ExportExpensesQuery(startDate, endDate, format);

        var exportResult = await _mediator.Send(query, cancellationToken);

        // return Ok(exportResult); // doesn't download the file

        return File(
            exportResult.Content,
            exportResult.ContentType,
            exportResult.FileName);
    }


//----  VIEW AND RESTORE DELETED EXPENSES    -----

    // GET: api/expense/deleted/my
    [Authorize(Policy = ExpensePermission.View)]
    [HttpGet("deleted/my")]
    public async Task<IActionResult> GetAllDeletedExpensesByEmail(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 5, 
        [FromQuery] string? sortBy = null, 
        [FromQuery] bool sortDesc = false,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllDeletedExpensesByEmailQuery(new PagedQuery(page, pageSize, sortBy, sortDesc));
        var deletedExpenses = await _mediator.Send(query, cancellationToken);
        return Ok(deletedExpenses);
    }

    // GET: api/expense/deleted/my/{id}
    [Authorize(Policy = ExpensePermission.View)]
    [HttpGet("deleted/my/{id:guid}")]
    public async Task<IActionResult> GetDeletedExpenseById(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var query = new GetDeletedExpenseByIdQuery(id);
        var deletedExpense = await _mediator.Send(query, cancellationToken);
        return Ok(deletedExpense);
    }

    // GET: api/expense/deleted/restore/{id}
    [Authorize(Policy = ExpensePermission.View)]
    [HttpPost("deleted/restore/{id:guid}")]
    public async Task<IActionResult> RestoreDeletedExpenseById(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var query = new RestoreDeletedExpenseByIdCommand(id);
        try
        {
            await _mediator.Send(query, cancellationToken);
            return Ok(new { message = "Expense restored successfully" });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Failed to restore expense" });
        }
    }
}