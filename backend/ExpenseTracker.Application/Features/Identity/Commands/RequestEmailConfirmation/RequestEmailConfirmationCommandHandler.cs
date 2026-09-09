using ExpenseTracker.Application.Common.Interfaces.Services;
using MediatR;

namespace ExpenseTracker.Application.Features.Identity.Commands.RequestEmailConfirmation;

public class RequestEmailConfirmationCommandHandler : IRequestHandler<RequestEmailConfirmationCommand, Unit>
{
    private readonly IIdentityService _identityService;
    private readonly IUserAccessor _userAccessor;

    public RequestEmailConfirmationCommandHandler(IIdentityService identityService, IUserAccessor userAccessor)
    {
        _identityService = identityService;
        _userAccessor = userAccessor;
    }

    public async Task<Unit> Handle(RequestEmailConfirmationCommand request, CancellationToken cancellationToken)
    {
        var userId = _userAccessor.UserId;

        await _identityService.RequestEmailConfirmationTokenAsync(userId);
        return Unit.Value;
    }
}