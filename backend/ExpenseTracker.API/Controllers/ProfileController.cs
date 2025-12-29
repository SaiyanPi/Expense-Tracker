using ExpenseTracker.Application.Common.Authorization.Permissions;
using ExpenseTracker.Application.DTOs.Auth;
using ExpenseTracker.Application.Features.Identity.Commands.ConfirmChangeEmail;
using ExpenseTracker.Application.Features.Identity.Commands.DeleteMyAccount;
using ExpenseTracker.Application.Features.Identity.Commands.RequestChangeEmail;
using ExpenseTracker.Application.Features.Identity.Commands.Update;
using ExpenseTracker.Application.Features.Users.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProfileController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET: api/profile/{id} OR GET: api/profile
    [HttpGet("{id?}")]
    public async Task<IActionResult> Profile(
        string? id,
        CancellationToken cancellationToken = default)
    {
        var query = new GetByIdQuery{ UserId = id };
        var user = await _mediator.Send(query, cancellationToken);
        return Ok(user);
    }


    // PUT: api/profile/update
    [HttpPut("update")]
    public async Task<IActionResult> UpdateProfile(
        [FromBody] UpdateUserDto dto,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var command = new UpdateUserCommand(
            dto.FullName,
            dto.PhoneNumber);

        await _mediator.Send(command, cancellationToken);
        return Ok(new {Success = true, Message = "Updated successfully" });
    }


    // DELETE: api/profile/{id} OR GET: api/profile
    [HttpPost("{id?}")]
    public async Task<IActionResult> DeleteProfile(
        string? id,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteProfileCommand{ UserId = id };
        await _mediator.Send(command, cancellationToken);
        return Ok(new {Success = true, Message = "Profile has been deleted successfully." });    
    }

}