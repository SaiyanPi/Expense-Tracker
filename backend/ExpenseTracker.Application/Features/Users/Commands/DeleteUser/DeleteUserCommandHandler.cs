using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;

namespace ExpenseTracker.Application.Features.Users.Commands.DeleteUser;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Unit>
{
    private readonly IUserRepository _userRepository;
    private readonly IIdentityService _identityService;
    private readonly IUserAccessor _userAccessor;
    private readonly IUserRoleService _userRoleService;
    public DeleteUserCommandHandler(
        IUserRepository userRepository,
        IIdentityService identityService,
        IUserAccessor userAccessor,
        IUserRoleService userRoleService
        )
    {
        _userRepository = userRepository;
        _identityService = identityService;
        _userAccessor = userAccessor;
        _userRoleService = userRoleService;
    }       

    public async Task<Unit> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        // BUISNESS RULE:
        // admin can only delete himself regular users, cannot delete other admins
        // if the user id is provided delete the regular user but if the user is admin throw exception
        // if id is not provided self delete

        var userId = _userAccessor.UserId;
       
        var targetUserId = request.UserId ?? userId; // if request.UserId is null, use userId

        var user = await _userRepository.GetByIdAsync(targetUserId);
        if(user is null)
            throw new NotFoundException(nameof(User), targetUserId);
        
        // check roles
        var currentUserIsAdmin = await _userRoleService.IsAdminAsync(userId);
        var targetUserIsAdmin = await _userRoleService.IsAdminAsync(targetUserId);
        
        if (targetUserId != userId)
        {
            // Trying to delete someone else
            if (!currentUserIsAdmin)
                throw new UnauthorizedAccessException("You do not have permission to delete other users.");

            // Admin cannot delete another admin
            if (targetUserIsAdmin)
                throw new ForbiddenException("Admin cannot delete another admin.");
        }
    
        await _identityService.DeleteAsync(targetUserId, cancellationToken);
        return Unit.Value;
    }
}   