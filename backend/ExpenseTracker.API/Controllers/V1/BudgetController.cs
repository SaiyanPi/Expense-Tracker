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
using ExpenseTracker.Application.Features.Budgets.Queries.GetAllDeletedBudgetsByEmail;
using ExpenseTracker.Application.Features.Budgets.Queries.GetDeletedBudgetById;
using ExpenseTracker.Application.Features.Budgets.Commands.RestoreDeletedBudgetById;
using AutoMapper;
using ExpenseTracker.API.Contracts.V1.Common.Pagination;
using ExpenseTracker.API.Contracts.V1.Budget;
using ExpenseTracker.API.Contracts.V1.Expense;

namespace ExpenseTracker.API.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class BudgetController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public BudgetController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    // GET: api/v1/Budget
    [Authorize(Policy = BudgetPermission.ViewAll)]
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] PagedResultRequestV1 pagedResultRequest,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllBudgetQuery(new PagedQuery(
            pagedResultRequest.page,
            pagedResultRequest.pageSize,
            pagedResultRequest.sortBy,
            pagedResultRequest.sortDesc));
        var budgets = await _mediator.Send(query, cancellationToken);

        var response = new PagedResultResponseV1<BudgetResponseV1>
        {
            Items = _mapper.Map<List<BudgetResponseV1>>(budgets.Items),

            TotalCount = budgets.TotalCount,
            Page = budgets.Page,
            PageSize = budgets.PageSize,
            TotalPages = budgets.TotalPages,
            HasNext = budgets.HasNext,
            HasPrevious = budgets.HasPrevious
        };
        return Ok(response);
    }

    // GET: api/v1/Budget/my
    [Authorize(Policy = BudgetPermission.View)]
    [HttpGet("my")]
    public async Task<IActionResult> GetAllBudgetsByEmail(
        [FromQuery] PagedResultRequestV1 pagedResultRequest,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllBudgetsByEmailQuery(new PagedQuery(
            pagedResultRequest.page,
            pagedResultRequest.pageSize,
            pagedResultRequest.sortBy,
            pagedResultRequest.sortDesc));
        var budgets = await _mediator.Send(query, cancellationToken);

        var response = new PagedResultResponseV1<BudgetResponseV1>
        {
            Items = _mapper.Map<List<BudgetResponseV1>>(budgets.Items),

            TotalCount = budgets.TotalCount,
            Page = budgets.Page,
            PageSize = budgets.PageSize,
            TotalPages = budgets.TotalPages,
            HasNext = budgets.HasNext,
            HasPrevious = budgets.HasPrevious
        };
        return Ok(response);
    }
    
    // GET: api/v1/Budget/{id}
    [Authorize(Policy = BudgetPermission.View)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetBudgetById(Guid id, CancellationToken cancellationToken = default)
    {
        var query = new GetBudgetByIdQuery(id);
        var budget = await _mediator.Send(query, cancellationToken);

        var response = _mapper.Map<BudgetResponseV1>(budget);
        return Ok(response);
    }

    // GET: api/v1/budget/budget-detail-with-expenses?budgetId={budgetId}
    [Authorize(Policy = BudgetPermission.View)]
    [HttpGet("budget-detail-with-expenses")]
    public async Task<IActionResult> GetBudgetDetailWithExpensesByEmail(
        [FromQuery] Guid budgetId,

        [FromQuery] PagedResultRequestV1 pagedResultRequest,
        CancellationToken cancellationToken = default)
    {
        var query = new GetBudgetDetailWithExpensesByEmailQuery(
            budgetId,
            new PagedQuery(
                pagedResultRequest.page,
                pagedResultRequest.pageSize,
                pagedResultRequest.sortBy,
                pagedResultRequest.sortDesc));

        var budgetDetailWithExpensesByEmail = await _mediator.Send(query, cancellationToken);

        var response = new BudgetDetailWithExpensesResponseV1
        {
            Id = budgetDetailWithExpensesByEmail.Id,
            Name = budgetDetailWithExpensesByEmail.Name,
            Limit = budgetDetailWithExpensesByEmail.Limit,
            TotalSpent = budgetDetailWithExpensesByEmail.TotalSpent,
            IsActive = budgetDetailWithExpensesByEmail.IsActive,
            
            Expenses = new PagedResultResponseV1<ExpenseResponseV1>
            {
                Items = _mapper.Map<List<ExpenseResponseV1>>(budgetDetailWithExpensesByEmail.Expenses.Items),
                
                TotalCount = budgetDetailWithExpensesByEmail.Expenses.TotalCount,
                Page = budgetDetailWithExpensesByEmail.Expenses.Page,
                PageSize = budgetDetailWithExpensesByEmail.Expenses.PageSize,
                TotalPages = budgetDetailWithExpensesByEmail.Expenses.TotalPages,
                HasNext = budgetDetailWithExpensesByEmail.Expenses.HasNext,
                HasPrevious = budgetDetailWithExpensesByEmail.Expenses.HasPrevious
            }
        };
        return Ok(response);
    }


    // GET: api/v1/budget/budget-summary/email?email={email}
    [Authorize(Policy = BudgetPermission.View)]
    [HttpGet("budget-summary/email")]
    public async Task<IActionResult> GetBudgetSummaryByEmail(
        [FromQuery] PagedResultRequestV1 pagedResultRequest,
        CancellationToken cancellationToken = default)
    {
        var query = new GetBudgetsSummaryByEmailQuery(new PagedQuery(
            pagedResultRequest.page,
            pagedResultRequest.pageSize,
            pagedResultRequest.sortBy,
            pagedResultRequest.sortDesc));
        var budgetsSummary = await _mediator.Send(query, cancellationToken);
        
        var response = new BudgetSummaryResponseV1
        {
            TotalBudget = budgetsSummary.TotalBudget,
            TotalExpenses = budgetsSummary.TotalExpenses,
            
            Categories = new PagedResultResponseV1<BudgetCategorySummaryResponseV1>
            {
                Items = _mapper.Map<List<BudgetCategorySummaryResponseV1>>(budgetsSummary.Categories.Items),
                
                TotalCount = budgetsSummary.Categories.TotalCount,
                Page = budgetsSummary.Categories.Page,
                PageSize = budgetsSummary.Categories.PageSize,
                TotalPages = budgetsSummary.Categories.TotalPages,
                HasNext = budgetsSummary.Categories.HasNext,
                HasPrevious = budgetsSummary.Categories.HasPrevious
            }
        };

        return Ok(response);
    }

    // POST: api/v1/budget
    [Authorize(Policy = BudgetPermission.Create)]
    [HttpPost]
    public async Task<IActionResult> CreateBudget([FromBody] CreateBudgetRequestV1 createRequest, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        var mappedCreateRequest = _mapper.Map<CreateBudgetDto>(createRequest);

        var command = new CreateBudgetCommand(mappedCreateRequest);
        var newBudget = await _mediator.Send(command, cancellationToken);

        var mappedNewBudget = _mapper.Map<BudgetResponseV1>(newBudget);
        return CreatedAtAction(nameof(GetBudgetById), new { id = newBudget.Id }, mappedNewBudget);
    }


    // PUT: api/v1/budget/{id}
    [Authorize(Policy = BudgetPermission.Update)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateBudget(Guid id, [FromBody] UpdateBudgetRequestV1 updateRequest, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var command = new UpdateBudgetCommand(
            updateRequest.Name, 
            updateRequest.Amount, 
            updateRequest.StartDate, 
            updateRequest.EndDate, 
            updateRequest.CategoryId
        )
        {
            Id = id
        };
        
        await _mediator.Send(command, cancellationToken);
        return Ok(new {Success = true, Message = "Budget updated successfully" }); 
    }

    // DELETE: api/v1/budget/{id}
    [Authorize(Policy = BudgetPermission.Delete)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteBudget(Guid id, CancellationToken cancellationToken = default)
    {
        var command = new DeleteBudgetCommand(id);
        await _mediator.Send(command, cancellationToken);
        return Ok(new {Success = true, Message = "Budget deleted successfully" }); 
    }

    //----  VIEW AND RESTORE DELETED BUDGETS    -----

    // GET: api/v1/budget/deleted/my
    [Authorize(Policy = BudgetPermission.View)]
    [HttpGet("deleted/my")]
    public async Task<IActionResult> GetAllDeletedBudgetsByEmail(
        [FromQuery] PagedResultRequestV1 pagedResultRequest,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllDeletedBudgetsByEmailQuery(new PagedQuery(
            pagedResultRequest.page,
            pagedResultRequest.pageSize,
            pagedResultRequest.sortBy,
            pagedResultRequest.sortDesc));
        var deletedBudgets = await _mediator.Send(query, cancellationToken);

        var response = new PagedResultResponseV1<BudgetResponseV1>
        {
            Items = _mapper.Map<List<BudgetResponseV1>>(deletedBudgets.Items),

            TotalCount = deletedBudgets.TotalCount,
            Page = deletedBudgets.Page,
            PageSize = deletedBudgets.PageSize,
            TotalPages = deletedBudgets.TotalPages,
            HasNext = deletedBudgets.HasNext,
            HasPrevious = deletedBudgets.HasPrevious
        };
        return Ok(response);
    }

    // GET: api/v1/budget/deleted/my/{id}
    [Authorize(Policy = BudgetPermission.View)]
    [HttpGet("deleted/my/{id:guid}")]
    public async Task<IActionResult> GetDeletedBudgetById(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var query = new GetDeletedBudgetByIdQuery(id);
        var deletedBudget = await _mediator.Send(query, cancellationToken);

        var mappedDeletedBudget = _mapper.Map<BudgetResponseV1>(deletedBudget);
        return Ok(mappedDeletedBudget);
    }

    // GET: api/v1/budget/deleted/restore/{id}
    [Authorize(Policy = BudgetPermission.View)]
    [HttpPost("deleted/restore/{id:guid}")]
    public async Task<IActionResult> RestoreDeletedBudgetById(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var query = new RestoreDeletedBudgetByIdCommand(id);
        try
        {
            await _mediator.Send(query, cancellationToken);
            return Ok(new { Success = true, Message = "Budget restored successfully" });
        }
        catch (Exception)
        {
            return BadRequest(new { Success = false, Message = "Failed to restore budget" });
        }
    }

}