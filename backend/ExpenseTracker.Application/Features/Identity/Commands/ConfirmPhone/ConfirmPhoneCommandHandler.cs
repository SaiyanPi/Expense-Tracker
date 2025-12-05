using ExpenseTracker.Application.Common.Interfaces.Services;
using MediatR;

namespace ExpenseTracker.Application.Features.Identity.Commands.ConfirmPhone;

public class ConfirmPhoneCommandHandler : IRequestHandler<ConfirmPhoneCommand, Unit>
{
    private readonly IIdentityService _identityService;

    public ConfirmPhoneCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Unit> Handle(ConfirmPhoneCommand request, CancellationToken cancellationToken)
    {
        await _identityService.ConfirmPhoneNumberAsync(request.VerifyPhoneDto, cancellationToken);
        
        return Unit.Value;
    }
}