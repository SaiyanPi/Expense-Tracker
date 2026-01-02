using ExpenseTracker.Application.Common.Authorization.Permissions;
using ExpenseTracker.Application.Common.Pagination;
using ExpenseTracker.Application.DTOs.Category;
using ExpenseTracker.Application.Features.Categories.Commands.CreateCategory;
using ExpenseTracker.Application.Features.Categories.Commands.DeleteCategory;
using ExpenseTracker.Application.Features.Categories.Commands.RestoreDeletedCategoryById;
using ExpenseTracker.Application.Features.Categories.Commands.UpdateCategory;
using ExpenseTracker.Application.Features.Categories.Queries.GetAllCategories;
using ExpenseTracker.Application.Features.Categories.Queries.GetAllCategoriesByEmail;
using ExpenseTracker.Application.Features.Categories.Queries.GetAllDeletedCategoriesByEmail;
using ExpenseTracker.Application.Features.Categories.Queries.GetCategoryById;
using ExpenseTracker.Application.Features.Categories.Queries.GetDeletedCategoryById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CategoryTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoryController(IMediator mediator)
    {
        _mediator = mediator;
    }


    // GET: api/Category
    [Authorize(Policy = CategoryPermission.ViewAll)]
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 5,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDesc = false,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllCategoriesQuery(new PagedQuery(page, pageSize, sortBy, sortDesc));
        var categories = await _mediator.Send(query, cancellationToken);
        return Ok(categories);
    }

    // GET: api/Category/my
    [Authorize(Policy = CategoryPermission.View)]
    [HttpGet("my")]
    public async Task<IActionResult> GetAllByEmail(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 5,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDesc = false,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllCategoriesByEmailQuery( new PagedQuery(page, pageSize, sortBy, sortDesc));
        var categories = await _mediator.Send(query, cancellationToken);
        return Ok(categories);
    }

    // GET: api/Category/{id}
    [Authorize(Policy = CategoryPermission.View)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var query = new GetCategoryByIdQuery(id);
        var category = await _mediator.Send(query, cancellationToken);
        return Ok(category);
    }

    // POST: api/Category
    [Authorize(Policy = CategoryPermission.Create)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var command = new CreateCategoryCommand(dto);
        var newCategory = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = newCategory.Id }, newCategory);
    }
    
    // PUT: api/Category/{id}
    [Authorize(Policy = CategoryPermission.Update)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryDto dto, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var command = new UpdateCategoryCommand(
            dto.Name
        )
        {
            Id = id
        };
        await _mediator.Send(command, cancellationToken);
        return Ok(new {Success = true, Message = "Expense Category updated successfully" }); 
    }

    // DELETE: api/Category/{id}
    [Authorize(Policy = CategoryPermission.Delete)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        var command = new DeleteCategoryCommand(id);
        await _mediator.Send(command, cancellationToken);
        return Ok(new {Success = true, Message = "Expense Category deleted successfully" }); 
    }


//----  VIEW AND RESTORE DELETED CATEGORIES    -----

    // GET: api/category/deleted/my
    [Authorize(Policy = CategoryPermission.View)]
    [HttpGet("deleted/my")]
    public async Task<IActionResult> GetAllDeletedCategoriesByEmail(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 5, 
        [FromQuery] string? sortBy = null, 
        [FromQuery] bool sortDesc = false,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllDeletedCategoriesByEmailQuery(new PagedQuery(page, pageSize, sortBy, sortDesc));
        var deletedCategories = await _mediator.Send(query, cancellationToken);
        return Ok(deletedCategories);
    }

    // GET: api/category/deleted/my/{id}
    [Authorize(Policy = CategoryPermission.View)]
    [HttpGet("deleted/my/{id:guid}")]
    public async Task<IActionResult> GetDeletedExpenseById(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var query = new GetDeletedCategoryByIdQuery(id);
        var deletedCategory = await _mediator.Send(query, cancellationToken);
        return Ok(deletedCategory);
    }

    // GET: api/category/deleted/restore/{id}
    [Authorize(Policy = CategoryPermission.View)]
    [HttpPost("deleted/restore/{id:guid}")]
    public async Task<IActionResult> RestoreDeletedCategoryById(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var query = new RestoreDeletedCategoryByIdCommand(id);
        try
        {
            await _mediator.Send(query, cancellationToken);
            return Ok(new { message = "Category restored successfully" });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Failed to restore category" });
        }
    }

}