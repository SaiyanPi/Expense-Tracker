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
        // admin can only delete regular users, cannot delete other admins

        var userIsAdmin = await _userRoleService.IsAdminAsync(request.UserId);
        if (userIsAdmin)
                throw new ForbiddenException("Admin cannot delete another admin.");

        await _identityService.DeleteAsync(request.UserId, cancellationToken);
        return Unit.Value;
    }
}   