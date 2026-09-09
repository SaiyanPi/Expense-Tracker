using ExpenseTracker.Application.Common.Authorization.Permissions;
using ExpenseTracker.Application.Common.ProfileImage;
using ExpenseTracker.Application.DTOs.Auth;
using ExpenseTracker.Application.Features.Identity.Commands.ConfirmChangeEmail;
using ExpenseTracker.Application.Features.Identity.Commands.DeleteMyAccount;
using ExpenseTracker.Application.Features.Identity.Commands.RequestChangeEmail;
using ExpenseTracker.Application.Features.Identity.Commands.Update;
using ExpenseTracker.Application.Features.Identity.Commands.UpdateProfileImage;
using ExpenseTracker.Application.Features.Users.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.API.Controllers.V1;

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

    // GET: api/profile/my
    [HttpGet("my")]
    public async Task<IActionResult> Profile(
        CancellationToken cancellationToken = default)
    {
        var query = new GetByIdQuery();
        var user = await _mediator.Send(query, cancellationToken);
        return Ok(user);
    }


    // PUT: api/profile/my/update
    [HttpPut("my/update")]
    public async Task<IActionResult> UpdateProfile(
        [FromBody] UpdateUserDto dto,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var command = new UpdateUserCommand(
            dto.FullName,
            dto.PhoneNumber);

        var updatedUser = await _mediator.Send(command, cancellationToken);
        return Ok(updatedUser);
    }


    // //POST: api/profile/my/image
    // [HttpPost("my/image")]
    // public async Task<IActionResult> UploadProfileImage(
    //     IFormFile image,
    //     CancellationToken cancellationToken)
    // {

    //     var command = new UploadProfileImageCommand(
    //         image.OpenReadStream(),
    //         image.FileName);

    //     await _mediator.Send(command, cancellationToken);
    //     return Ok(new { Success = true, Message = "Profile image uploaded successfully." });
    // }



    //PUT: api/profile/my/image/update
    [HttpPut("my/image/update")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(6 * 1024 * 1024)]
    public async Task<IActionResult> UpdateProfileImage(
        [FromForm] IFormFile image,
        CancellationToken cancellationToken)
    {
        await using var stream = image.OpenReadStream();

        var command = new UpdateProfileImageCommand(
            new ProfileImageUpdate(
                stream,
                image.FileName,
                image.Length,
                image.ContentType)
        );

        var updatedUser = await _mediator.Send(command, cancellationToken);
        return Ok(updatedUser);
    }

    
    // DELETE: api/profile/my/delete
    [HttpDelete("my/delete")]
    public async Task<IActionResult> DeleteProfile(
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteProfileCommand();
        await _mediator.Send(command, cancellationToken);
        return Ok(new {Success = true, Message = "Your Profile has been deleted successfully." });    
    }

}