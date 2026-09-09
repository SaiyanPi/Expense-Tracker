using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Application.DTOs.Auth;
using MediatR;

namespace ExpenseTracker.Application.Features.Identity.Commands.UpdateProfileImage;

public class UpdateProfileImageCommandHandler: IRequestHandler<UpdateProfileImageCommand, UserDto>
{
    private readonly IIdentityService _identityService;
    private readonly IUserAccessor _userAccessor;

    public UpdateProfileImageCommandHandler(
        IIdentityService identityService,
        IUserAccessor userAccessor)
    {
        _identityService = identityService;
        _userAccessor = userAccessor;
    }

    public async Task<UserDto> Handle(
        UpdateProfileImageCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _userAccessor.UserId;
        
        var updatedUser = await _identityService.UpdateProfileImageAsync(
            userId,
            request.ImageUpdate.Content,
            request.ImageUpdate.FileName,
            cancellationToken);

        return updatedUser;
    }

}