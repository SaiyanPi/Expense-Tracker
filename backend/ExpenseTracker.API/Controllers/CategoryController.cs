using ExpenseTracker.Application.DTOs.Category;
using ExpenseTracker.Application.Features.Categories.Commands.CreateCategory;
using ExpenseTracker.Application.Features.Categories.Commands.DeleteCategory;
using ExpenseTracker.Application.Features.Categories.Commands.UpdateCategory;
using ExpenseTracker.Application.Features.Categories.Queries.GetAllCategories;
using ExpenseTracker.Application.Features.Categories.Queries.GetAllCategoriesByEmail;
using ExpenseTracker.Application.Features.Categories.Queries.GetCategoryById;
using MediatR;
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
    //[Authorize(Policy = "CanViewAllUsersCategory")]
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        var query = new GetAllCategoriesQuery();
        var categories = await _mediator.Send(query, cancellationToken);
        return Ok(categories);
    }

    // GET: api/Category/email?email={email}
    //[Authorize]
    [HttpGet("email")]
    public async Task<IActionResult> GetAllByEmail(string email, CancellationToken cancellationToken = default)
    {
        var query = new GetAllCategoriesByEmailQuery(email);
        var categories = await _mediator.Send(query, cancellationToken);
        return Ok(categories);
    }

    // GET: api/Category/{id}
   //[Authorize("canViewAllUsersCategory")]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var query = new GetCategoryByIdQuery(id);
        var category = await _mediator.Send(query, cancellationToken);
        return Ok(category);
    }

    // POST: api/Category
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
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var command = new UpdateCategoryCommand(
            dto.Name,
            dto.UserId
        )
        {
            Id = id
        };
        await _mediator.Send(command, cancellationToken);
        return Ok(new {Success = true, Message = "Expense Category updated successfully" }); 
    }

    // DELETE: api/Category/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteCategoryCommand(id);
        await _mediator.Send(command, cancellationToken);
        return Ok(new {Success = true, Message = "Expense Category deleted successfully" }); 
    }

}