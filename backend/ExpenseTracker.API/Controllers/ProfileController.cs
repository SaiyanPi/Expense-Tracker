using ExpenseTracker.Application.DTOs.Auth;
using ExpenseTracker.Application.Features.Identity.Commands.ConfirmChangeEmail;
using ExpenseTracker.Application.Features.Identity.Commands.DeleteMyAccount;
using ExpenseTracker.Application.Features.Identity.Commands.RequestChangeEmail;
using ExpenseTracker.Application.Features.Identity.Commands.Update;
using ExpenseTracker.Application.Features.Users.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProfileController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProfileController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET: api/profile/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Profile(string id, CancellationToken cancellationToken = default)
    {
        var query = new GetByIdQuery(id);
        var user = await _mediator.Send(query, cancellationToken);
        return Ok(user);
    }


    // PUT: api/profile/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateProfile(string id, [FromBody] UpdateUserDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var command = new UpdateUserCommand(
            dto.FullName,
            dto.PhoneNumber
        )
        {
            UserId = id
        };
        await _mediator.Send(command, cancellationToken);
        return Ok(new {Success = true, Message = "Updated successfully" });
    }


    // DELETE: api/profile/{id}
    [HttpPost("{id:guid}")]
    public async Task<IActionResult> DeleteProfile(string id, CancellationToken cancellationToken)
    {
        var command = new DeleteProfileCommand(id);
        await _mediator.Send(command, cancellationToken);
        return Ok(new {Success = true, Message = "Your profile has been deleted successfully." });    
    }


    //POST: api/profile/change-email
    [HttpPost("change-email")]
    public async Task<IActionResult> RequestChangeEmail([FromBody] ChangeEmailRequestDto dto, CancellationToken cancellationToken)
    {
        if(!ModelState.IsValid)
            return BadRequest(ModelState);
        
        var command = new RequestChangeEmailCommand(dto);
        await _mediator.Send(command, cancellationToken);
        return Ok(new {Success = true, Message = "Email change confirmation link sent to your new email"});
    }


    // GET: api/profile/confirm-change-email?userId={UserId}&newEmail={newEmail}&token={token}
    [HttpGet("confirm-change-email")]
    public async Task<IActionResult> ConfirmChangeEmail([FromQuery] ConfirmChangeEmailDto dto, CancellationToken cancellationToken)
    {
        if(!ModelState.IsValid)
            return BadRequest(ModelState);
        
        var command = new ConfirmChangeEmailCommand(dto);
        await _mediator.Send(command, cancellationToken);
        return Ok(new {Success = true, Message = "Email has been changed successfully"});
    }
}