using ExpenseTracker.Application.Common.Authorization.Permissions;
using ExpenseTracker.Application.Common.Pagination;
using ExpenseTracker.Application.Features.Users.Commands.DeleteUser;
using ExpenseTracker.Application.Features.Users.Queries.GetAllUsers;
using ExpenseTracker.Application.Features.Users.Queries.GetByEmail;
using ExpenseTracker.Application.Features.Users.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.API.Controllers;

[Authorize(Policy = UserManagementPermission.All)]
[ApiController]
[Route("api/[controller]")]
public class UserManagementController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserManagementController(IMediator mediator)
    {
        _mediator = mediator;
    }
    // USER MANAGEMENT

    // GET: api/userManagement
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 5,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDesc = false,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllUsersQuery(new PagedQuery(page, pageSize, sortBy, sortDesc));
        var users = await _mediator.Send(query, cancellationToken);
        return Ok(users);
    }

    // GET: api/userManagement/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken = default)
    {
        var query = new GetByIdQuery { UserId = id };
        var user = await _mediator.Send(query, cancellationToken);
        return Ok(user);
    }

    // GET: api/userManagement/email/{email}
    [HttpGet("email/{email}")]
    public async Task<IActionResult> GetByEmail(string email, CancellationToken cancellationToken = default)
    {
        var query = new GetByEmailQuery(email);
        var user = await _mediator.Send(query, cancellationToken);
        return Ok(user);
    }


    // DELETE: api/userManagement/{id} OR api/userManagement
    [HttpDelete("{id:guid?}")]
    public async Task<IActionResult> Delete(string? id, CancellationToken cancellationToken = default)
    {
        var command = new DeleteUserCommand(id);
        await _mediator.Send(command, cancellationToken);
        return Ok(new {Success = true, Message = "User deleted successfully." });    
    }
}