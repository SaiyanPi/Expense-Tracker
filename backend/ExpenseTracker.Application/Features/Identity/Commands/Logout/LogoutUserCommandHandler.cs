using ExpenseTracker.Application.Common.Interfaces.Services;
using MediatR;

namespace ExpenseTracker.Application.Features.Identity.Commands.Logout;

public class LogoutUserCommandHandler : IRequestHandler<LogoutUserCommand, Unit>
{
    private readonly IIdentityService _identityService;
    private readonly IUserAccessor _userAccessor;

    public LogoutUserCommandHandler(
        IIdentityService identityService,
        IUserAccessor userAccessor)
    {
        _identityService = identityService;
        _userAccessor = userAccessor;
    }

    public async Task<Unit> Handle(LogoutUserCommand request, CancellationToken cancellationToken)
    {
        var userId = _userAccessor.UserId;
        await _identityService.LogoutAsync(userId, cancellationToken);
        return Unit.Value;
    }
}