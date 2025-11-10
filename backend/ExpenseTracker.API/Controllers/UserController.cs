using ExpenseTracker.Application.DTOs.User;
using ExpenseTracker.Application.Features.Users.Commands.DeleteUser;
using ExpenseTracker.Application.Features.Users.Commands.UpdateUser;
using ExpenseTracker.Application.Features.Users.Queries.GetAllUsers;
using ExpenseTracker.Application.Features.Users.Queries.GetByEmail;
using ExpenseTracker.Application.Features.Users.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET: api/user
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        var query = new GetAllUsersQuery();
        var users = await _mediator.Send(query, cancellationToken);
        return Ok(users);
    }

    // GET: api/user/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken = default)
    {
        var query = new GetByIdQuery(id);
        var user = await _mediator.Send(query, cancellationToken);
        return Ok(user);
    }

    // GET: api/user/email/{email}
    [HttpGet("email/{email}")]
    public async Task<IActionResult> GetByEmail(string email, CancellationToken cancellationToken = default)
    {
        var query = new GetByEmailQuery(email);
        var user = await _mediator.Send(query, cancellationToken);
        return Ok(user);
    }

    // POST: api/user
    // [HttpPost]
    // public async Task<IActionResult> Create([FromBody] RegisterUserDto dto, CancellationToken cancellationToken = default)
    // {
    //     if (!ModelState.IsValid)
    //         return BadRequest(ModelState);

    //     var newUserId = await _userService.RegisterAsync(dto, cancellationToken);
    //     return CreatedAtAction(nameof(GetById), new { id = newUserId }, null);
    // }

    // PUT: api/user/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateUserDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var command = new UpdateUserCommand(id, dto);
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    // DELETE: api/user/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        var command = new DeleteUserCommand(id);
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }
}