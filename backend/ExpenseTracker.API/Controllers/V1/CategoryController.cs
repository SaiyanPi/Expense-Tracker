using AutoMapper;
using ExpenseTracker.API.Contracts.V1.Category;
using ExpenseTracker.API.Contracts.V1.Common.Pagination;
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

namespace ExpenseTracker.API.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public CategoryController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }


    // GET: api/v1/Category
    [Authorize(Policy = CategoryPermission.ViewAll)]
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] PagedResultRequestV1 pagedResultRequest,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllCategoriesQuery(new PagedQuery(
            pagedResultRequest.page,
            pagedResultRequest.pageSize,
            pagedResultRequest.sortBy,
            pagedResultRequest.sortDesc));
        var categories = await _mediator.Send(query, cancellationToken);

        var response = new PagedResultResponseV1<CategoryResponseV1>
        {
            Items = _mapper.Map<List<CategoryResponseV1>>(categories.Items),

            TotalCount = categories.TotalCount,
            Page = categories.Page,
            PageSize = categories.PageSize,
            TotalPages = categories.TotalPages,
            HasNext = categories.HasNext,
            HasPrevious = categories.HasPrevious
        };
        return Ok(response);
    }

    // GET: api/v1/Category/my
    [Authorize(Policy = CategoryPermission.View)]
    [HttpGet("my")]
    public async Task<IActionResult> GetAllByEmail(
        [FromQuery] PagedResultRequestV1 pagedResultRequest,    
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllCategoriesByEmailQuery( new PagedQuery(
            pagedResultRequest.page,
            pagedResultRequest.pageSize,
            pagedResultRequest.sortBy,
            pagedResultRequest.sortDesc));
        var categories = await _mediator.Send(query, cancellationToken);
        
        var response = new PagedResultResponseV1<CategoryResponseV1>
        {
            Items = _mapper.Map<List<CategoryResponseV1>>(categories.Items),

            TotalCount = categories.TotalCount,
            Page = categories.Page,
            PageSize = categories.PageSize,
            TotalPages = categories.TotalPages,
            HasNext = categories.HasNext,
            HasPrevious = categories.HasPrevious
        };
        return Ok(response);
    }

    // GET: api/v1/Category/{id}
    [Authorize(Policy = CategoryPermission.View)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var query = new GetCategoryByIdQuery(id);
        var category = await _mediator.Send(query, cancellationToken);

        var response = _mapper.Map<CategoryResponseV1>(category);
        return Ok(response);
    }

    // POST: api/v1/Category
    [Authorize(Policy = CategoryPermission.Create)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCategoryRequestV1 createRequest, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var mappedCreateRequest = _mapper.Map<CreateCategoryDto>(createRequest);

        var command = new CreateCategoryCommand(mappedCreateRequest);

        var newCategory = await _mediator.Send(command, cancellationToken);

        var mappedNewCategory = _mapper.Map<CategoryResponseV1>(newCategory);
        return CreatedAtAction(nameof(GetById), new { id = mappedNewCategory.Id }, mappedNewCategory);
    }
    
    // PUT: api/v1/Category/{id}
    [Authorize(Policy = CategoryPermission.Update)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryRequestV1 updateRequest, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var command = new UpdateCategoryCommand(
            updateRequest.Name
        )
        {
            Id = id
        };
        await _mediator.Send(command, cancellationToken);
        return Ok(new {Success = true, Message = "Expense Category updated successfully" }); 
    }

    // DELETE: api/v1/Category/{id}
    [Authorize(Policy = CategoryPermission.Delete)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        var command = new DeleteCategoryCommand(id);
        await _mediator.Send(command, cancellationToken);
        return Ok(new {Success = true, Message = "Expense Category deleted successfully" }); 
    }


//----  VIEW AND RESTORE DELETED CATEGORIES    -----

    // GET: api/v1/category/deleted/my
    [Authorize(Policy = CategoryPermission.View)]
    [HttpGet("deleted/my")]
    public async Task<IActionResult> GetAllDeletedCategoriesByEmail(
        [FromQuery] PagedResultRequestV1 pagedResultRequest,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllDeletedCategoriesByEmailQuery(new PagedQuery(
            pagedResultRequest.page,
            pagedResultRequest.pageSize,
            pagedResultRequest.sortBy,
            pagedResultRequest.sortDesc));
        var deletedCategories = await _mediator.Send(query, cancellationToken);
        
        var response = new PagedResultResponseV1<CategoryResponseV1>
        {
            Items = _mapper.Map<List<CategoryResponseV1>>(deletedCategories.Items),

            TotalCount = deletedCategories.TotalCount,
            Page = deletedCategories.Page,
            PageSize = deletedCategories.PageSize,
            TotalPages = deletedCategories.TotalPages,
            HasNext = deletedCategories.HasNext,
            HasPrevious = deletedCategories.HasPrevious
        };  
        return Ok(response);
    }

    // GET: api/v1/category/deleted/my/{id}
    [Authorize(Policy = CategoryPermission.View)]
    [HttpGet("deleted/my/{id:guid}")]
    public async Task<IActionResult> GetDeletedExpenseById(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var query = new GetDeletedCategoryByIdQuery(id);
        var deletedCategory = await _mediator.Send(query, cancellationToken);

        var response = _mapper.Map<CategoryResponseV1>(deletedCategory);
        return Ok(response);
    }

    // GET: api/v1/category/deleted/restore/{id}
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
            return Ok(new { Success = true, Message = "Category restored successfully" });
        }
        catch (Exception)
        {
            return BadRequest(new { Success = false, Message = "Failed to restore category" });
        }
    }

}