using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Application.Features.Identity.Commands.DeleteMyAccount;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;

namespace ExpenseTracker.Application.Features.Users.Commands.DeleteUser;

public record DeleteProfileCommandHandler : IRequestHandler<DeleteProfileCommand, Unit>
{
    private readonly IIdentityService _identityService;
    private readonly IUserAccessor _userAccessor;
    private readonly IUserRoleService _userRoleService;

    public DeleteProfileCommandHandler(
        IIdentityService identityService,
        IUserAccessor userAccessor,
        IUserRoleService userRoleService)
    {
        _identityService = identityService;
        _userAccessor = userAccessor;
        _userRoleService = userRoleService;
    }       

    public async Task<Unit> Handle(DeleteProfileCommand request, CancellationToken cancellationToken)
    {
        // BUISNESS RULE:
        // if user is admin redirect to delete user with the id from query
        // if user is admin but id is not in the query redirect to delete his own id from token
        // if user is regular userredirect to delete his own id from token

        var userId = _userAccessor.UserId;

        var isAdmin = await _userRoleService.IsAdminAsync(userId);
        string targetUserId;

        if (isAdmin)
        {
            targetUserId = string.IsNullOrEmpty(request.UserId) ? userId : request.UserId!;
        }
        else
        {
            targetUserId = userId;
        }
        await _identityService.DeleteAsync(targetUserId, cancellationToken);
        return Unit.Value;
    }
}   