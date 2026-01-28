using AutoMapper;
using ExpenseTracker.API.Contracts.V1.Category;
using ExpenseTracker.API.Contracts.V1.Common.Pagination;
using ExpenseTracker.API.Contracts.V1.Expense;
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
using Microsoft.AspNetCore.RateLimiting;

namespace ExpenseTracker.API.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ExpensesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public ExpensesController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    // GET: api/v1/expenses
    [Authorize(Policy = ExpensePermission.ViewAll)]
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] PagedResultRequestV1 pagedRequest,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllExpensesQuery(new PagedQuery(
            pagedRequest.page,
            pagedRequest.pageSize,
            pagedRequest.sortBy,
            pagedRequest.sortDesc));
        var expenses = await _mediator.Send(query, cancellationToken);

        var response = new PagedResultResponseV1<ExpenseResponseV1>
        {
            Items = _mapper.Map<List<ExpenseResponseV1>>(expenses.Items),
            
            TotalCount = expenses.TotalCount,
            Page = expenses.Page,
            PageSize = expenses.PageSize,
            TotalPages = expenses.TotalPages,
            HasNext = expenses.HasNext,
            HasPrevious = expenses.HasPrevious
        };
        return Ok(response);
    }

    // GET: api/v1/expenses/my
    [Authorize(Policy = ExpensePermission.View)]
    [HttpGet("my")]
    public async Task<IActionResult> GetExpensesByEmail(
        [FromQuery] PagedResultRequestV1 pagedRequest,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllExpensesByEmailQuery(new PagedQuery(
            pagedRequest.page,
            pagedRequest.pageSize,
            pagedRequest.sortBy,
            pagedRequest.sortDesc));
        var expensesByEmail = await _mediator.Send(query, cancellationToken);

        var response = new PagedResultResponseV1<ExpenseResponseV1>
        {
            Items = _mapper.Map<List<ExpenseResponseV1>>(expensesByEmail.Items),

            TotalCount = expensesByEmail.TotalCount,
            Page = expensesByEmail.Page,
            PageSize = expensesByEmail.PageSize,
            TotalPages = expensesByEmail.TotalPages,
            HasNext = expensesByEmail.HasNext,
            HasPrevious = expensesByEmail.HasPrevious
        };
        return Ok(response);
    }

    // GET: api/v1/expenses/budget-expenses/my?budgetId={budgetId}
    [Authorize(Policy = ExpensePermission.View)]
    [HttpGet("budget-expenses/my")]
    public async Task<IActionResult> GetExpensesForABudgetByEmail(
        [FromQuery] Guid budgetId,
        [FromQuery] PagedResultRequestV1 pagedRequest,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllExpensesForABudgetByEmailQuery(budgetId, new PagedQuery(
            pagedRequest.page,
            pagedRequest.pageSize,
            pagedRequest.sortBy,
            pagedRequest.sortDesc));
        var expensesForBudget = await _mediator.Send(query, cancellationToken);

        var response = new PagedResultResponseV1<ExpenseSummaryForBudgetResponseV1>
        {
            Items = _mapper.Map<List<ExpenseSummaryForBudgetResponseV1>>(expensesForBudget.Items),

            TotalCount = expensesForBudget.TotalCount,
            Page = expensesForBudget.Page,
            PageSize = expensesForBudget.PageSize,
            TotalPages = expensesForBudget.TotalPages,
            HasNext = expensesForBudget.HasNext,
            HasPrevious = expensesForBudget.HasPrevious
        };
        return Ok(response);
    }

    // GET: api/v1/expenses/category-expenses/my?categoryId={categoryId}
    [Authorize(Policy = ExpensePermission.View)]
    [HttpGet("category-expenses/my")]
    public async Task<IActionResult> GetExpensesForCategoryByEmail(
        [FromQuery] Guid categoryId,
        [FromQuery] PagedResultRequestV1 pagedRequest   ,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllExpensesForCategoryByEmailQuery(categoryId, new PagedQuery(
            pagedRequest.page,
            pagedRequest.pageSize,
            pagedRequest.sortBy,
            pagedRequest.sortDesc));
        var expensesForCategory = await _mediator.Send(query, cancellationToken);

        var response = new PagedResultResponseV1<ExpenseSummaryForCategoryResponseV1>
        {
            Items = _mapper.Map<List<ExpenseSummaryForCategoryResponseV1>>(expensesForCategory.Items),

            TotalCount = expensesForCategory.TotalCount,
            Page = expensesForCategory.Page,
            PageSize = expensesForCategory.PageSize,
            TotalPages = expensesForCategory.TotalPages,
            HasNext = expensesForCategory.HasNext,
            HasPrevious = expensesForCategory.HasPrevious
        };
        return Ok(response);
    }

    // GET: api/v1/expenses/category-summary
    [Authorize(Policy = ExpensePermission.ViewAll)]
    [HttpGet("category-summary")]
    public async Task<IActionResult> GetCategorySummary(
        [FromQuery] PagedResultRequestV1 pagedRequest,
        CancellationToken cancellationToken = default)
    {
        var query = new GetCategorySummaryQuery(new PagedQuery(
            pagedRequest.page,
            pagedRequest.pageSize,
            pagedRequest.sortBy,
            pagedRequest.sortDesc));
        var categorySummary = await _mediator.Send(query, cancellationToken);
        
        var response = new PagedResultResponseV1<CategorySummaryResponseV1>
        {
            Items = _mapper.Map<List<CategorySummaryResponseV1>>(categorySummary.Items),

            TotalCount = categorySummary.TotalCount,
            Page = categorySummary.Page,
            PageSize = categorySummary.PageSize,
            TotalPages = categorySummary.TotalPages,
            HasNext = categorySummary.HasNext,
            HasPrevious = categorySummary.HasPrevious
        };
        return Ok(response);
    }

    // GET: api/v1/expenses/category-summary/my
    [Authorize(Policy = ExpensePermission.View)]
    [HttpGet("category-summary/my")]
    public async Task<IActionResult> GetCategorySummaryByEmail(
        [FromQuery] PagedResultRequestV1 pagedRequest,
        CancellationToken cancellationToken = default)
    {
        var query = new GetCategorySummaryByEmailQuery(new PagedQuery(
            pagedRequest.page,
            pagedRequest.pageSize,
            pagedRequest.sortBy,
            pagedRequest.sortDesc));
        var categorySummaryByEmail = await _mediator.Send(query, cancellationToken);
        
        var response = new PagedResultResponseV1<CategorySummaryResponseV1>
        {
            Items = _mapper.Map<List<CategorySummaryResponseV1>>(categorySummaryByEmail.Items),

            TotalCount = categorySummaryByEmail.TotalCount,
            Page = categorySummaryByEmail.Page,
            PageSize = categorySummaryByEmail.PageSize,
            TotalPages = categorySummaryByEmail.TotalPages,
            HasNext = categorySummaryByEmail.HasNext,
            HasPrevious = categorySummaryByEmail.HasPrevious
        };
        return Ok(response);
    }

    // GET: api/v1/expenses/{id}
    [Authorize(Policy = ExpensePermission.View)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var query = new GetExpenseByIdQuery(id);
        var expense = await _mediator.Send(query, cancellationToken);

        var response = _mapper.Map<ExpenseResponseV1>(expense);
        return Ok(response);
    }

    // GET: api/v1/expenses/total
    [Authorize(Policy = ExpensePermission.ViewAll)]
    [HttpGet("total")]
    public async Task<IActionResult> GetTotalExpense(CancellationToken cancellationToken = default)
    {
        var query = new GetTotalExpensesQuery();
        var totalExpenses = await _mediator.Send(query, cancellationToken);

        var response = new TotalExpenseResponseV1
        {
            TotalAmount = totalExpenses.TotalAmount
        };
        return Ok(response);
    }

    // GET: api/v1/expenses/total-expense/my
    [Authorize(Policy = ExpensePermission.View)]
    [HttpGet("total-expense/my")]
    public async Task<IActionResult> GetTotalExpenseByEmail(CancellationToken cancellationToken = default)
    {
        var query = new GetTotalExpenseByEmailQuery();
        var totalExpensesByEmail = await _mediator.Send(query, cancellationToken);

        var response = new TotalExpenseResponseV1
        {
            TotalAmount = totalExpensesByEmail.TotalAmount
        };
        return Ok(response);
    }

    // GET: api/v1/expenses/filter?startDate=&endDate=&minAmount=&maxAmount=&categoryId=&userId=
    [Authorize(Policy = "Expense.Filter")]
    [HttpGet("filter")]
    public async Task<IActionResult> FilterExpenses(
        [FromQuery] ExpenseFilterRequestV1 filterRequest,
        [FromQuery] PagedResultRequestV1 pagedRequest,

        CancellationToken cancellationToken = default)
    {
        var query = new FilterExpensesQuery(
            new ExpenseFilter(
                filterRequest.startDate,
                filterRequest.endDate,
                filterRequest.minAmount,
                filterRequest.maxAmount,
                filterRequest.categoryId,
                filterRequest.userId),
            new PagedQuery(
                pagedRequest.page,
                pagedRequest.pageSize,
                pagedRequest.sortBy,
                pagedRequest.sortDesc));

        var result = await _mediator.Send(query, cancellationToken);
        // return Ok(result);

        var response = new FilteredExpensesResultResponseV1
        {
            TotalAmount = result.TotalAmount,
            Expenses = new PagedResultResponseV1<ExpenseResponseV1>
            {
                Items = _mapper.Map<List<ExpenseResponseV1>>(result.Expenses.Items),

                TotalCount = result.Expenses.TotalCount,
                Page = result.Expenses.Page,
                PageSize = result.Expenses.PageSize,
                TotalPages = result.Expenses.TotalPages,
                HasNext = result.Expenses.HasNext,
                HasPrevious = result.Expenses.HasPrevious
            }
        };
        return Ok(response);
    }


    // POST: api/v1/expenses
    [Authorize(Policy = ExpensePermission.Create)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateExpenseRequestV1 createRequest, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var mappedRequest = _mapper.Map<CreateExpenseDto>(createRequest);

        var command = new CreateExpenseCommand(mappedRequest);

        var newExpense = await _mediator.Send(command, cancellationToken);

        var mappedNewExpense = _mapper.Map<ExpenseResponseV1>(newExpense);
        return CreatedAtAction(nameof(GetById), new { id = mappedNewExpense.Id }, mappedNewExpense);
    }

    // PUT: api/v1/expenses/{id}
    [Authorize(Policy = ExpensePermission.Update)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateExpenseRequestV1 updateRequest, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        // not using automapper here because we used primitive properties in UpdateExpenseCommand
        var command = new UpdateExpenseCommand(
            updateRequest.Title,
            updateRequest.Description,
            updateRequest.Amount,
            updateRequest.Date,
            updateRequest.CategoryId,
            updateRequest.BudgetId
        )
        {
            Id = id
        };
        
        await _mediator.Send(command, cancellationToken);
        return Ok(new {Success = true, Message = "Expense updated successfully" }); 
    }

    // DELETE: api/v1/expenses/{id}
    [Authorize(Policy = ExpensePermission.Delete)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        var command = new DeleteExpenseCommand(id);
        await _mediator.Send(command, cancellationToken);
        return Ok(new {Success = true, Message = "Expense deleted successfully" });    
    }

    // GET: api/v1/expenses/export?format={format}
    [EnableRateLimiting("Heavy")] // overrides Global
    [Authorize(Policy = "Expense.Filter")]
    [HttpGet("export")]
    public async Task<IActionResult> ExportExpenses(
        [FromQuery] ExportExpensesRequestV1 request,
        CancellationToken cancellationToken = default)
    {
        var query = new ExportExpensesQuery(
            request.format,
            new ExpenseFilter(
                request.startDate,
                request.endDate,
                request.minAmount,
                request.maxAmount,
                request.categoryId,
                request.userId));
        var exportResult = await _mediator.Send(query, cancellationToken);

        // return Ok(exportResult); // doesn't download the file

        return File(
            exportResult.Content,
            exportResult.ContentType,
            exportResult.FileName);
    }


//----  VIEW AND RESTORE DELETED EXPENSES    -----

    // GET: api/v1/expenses/deleted/my
    [Authorize(Policy = ExpensePermission.View)]
    [HttpGet("deleted/my")]
    public async Task<IActionResult> GetAllDeletedExpensesByEmail(
        [FromQuery] PagedResultRequestV1 pagedRequest,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllDeletedExpensesByEmailQuery(new PagedQuery(
            pagedRequest.page,
            pagedRequest.pageSize,
            pagedRequest.sortBy,
            pagedRequest.sortDesc));
        var deletedExpenses = await _mediator.Send(query, cancellationToken);

        var response = new PagedResultResponseV1<ExpenseResponseV1>
        {
            Items = _mapper.Map<List<ExpenseResponseV1>>(deletedExpenses.Items),

            TotalCount = deletedExpenses.TotalCount,
            Page = deletedExpenses.Page,
            PageSize = deletedExpenses.PageSize,
            TotalPages = deletedExpenses.TotalPages,
            HasNext = deletedExpenses.HasNext,
            HasPrevious = deletedExpenses.HasPrevious
        };
        return Ok(response);
    }

    // GET: api/v1/expenses/deleted/my/{id}
    [Authorize(Policy = ExpensePermission.View)]
    [HttpGet("deleted/my/{id:guid}")]
    public async Task<IActionResult> GetDeletedExpenseById(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var query = new GetDeletedExpenseByIdQuery(id);
        var deletedExpense = await _mediator.Send(query, cancellationToken);

        var mappedDeletedExpense = _mapper.Map<ExpenseResponseV1>(deletedExpense);
        return Ok(mappedDeletedExpense);
    }

    // GET: api/v1/expenses/deleted/restore/{id}
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
            return Ok(new {Success = true, Message = "Expense restored successfully" });
        }
        catch (Exception)
        {
            return BadRequest(new { Success = false, Message = "Failed to restore expense" });
        }
    }
}