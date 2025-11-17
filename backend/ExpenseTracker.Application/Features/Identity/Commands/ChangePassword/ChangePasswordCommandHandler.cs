using ExpenseTracker.Application.Common.Interfaces.Services;
using MediatR;

namespace ExpenseTracker.Application.Features.Identity.Commands.ChangePassword;

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Unit>
{
    private readonly IIdentityService _identityService;

    public ChangePasswordCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Unit> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        await _identityService.ChangePasswordAsync(request.ChangePasswordDto, cancellationToken);
        return Unit.Value;
    }
}