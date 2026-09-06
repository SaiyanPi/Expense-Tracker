using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.Common.Interfaces.Services;
using MediatR;

namespace ExpenseTracker.Application.Features.Identity.Commands.UpdateProfileImage;

public class UpdateProfileImageCommandHandler: IRequestHandler<UpdateProfileImageCommand, Unit>
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

    public async Task<Unit> Handle(
        UpdateProfileImageCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _userAccessor.UserId;
        
        await _identityService.UpdateProfileImageAsync(
            userId,
            request.ImageUpdate.Content,
            request.ImageUpdate.FileName,
            cancellationToken);

        return Unit.Value;
    }

}