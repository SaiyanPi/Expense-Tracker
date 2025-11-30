using ExpenseTracker.Application.Common.Interfaces.Services;
using MediatR;

namespace ExpenseTracker.Application.Features.Identity.Commands.EmailConfirmation;

public class EmailConfirmationCommandHandler : IRequestHandler<EmailConfirmationCommand, Unit>
{
    private readonly IIdentityService _identityService;

    public EmailConfirmationCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Unit> Handle(EmailConfirmationCommand request, CancellationToken cancellationToken)
    {
        await _identityService.ConfirmEmailAsync(request.VerifyEmailDto, cancellationToken);
        return Unit.Value;
    }
}