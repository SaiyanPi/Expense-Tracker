using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Application.DTOs.Auth;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;

namespace ExpenseTracker.Application.Features.Identity.Commands.SendPhoneConfirmationCode;

public class SendPhoneConfirmationCodeHandler : IRequestHandler<SendPhoneConfirmationCodeCommand, Unit>
{
    private readonly IIdentityService _identityService;    

    public SendPhoneConfirmationCodeHandler(IIdentityService identityService, 
    ISmsSenderService smsSenderService)
    {
        _identityService = identityService;
    }

    public async Task<Unit> Handle(SendPhoneConfirmationCodeCommand request, CancellationToken cancellationToken)
    {
        await _identityService.GeneratePhoneConfirmationTokenAsync(request.PhoneConfirmationDto, cancellationToken);

        return Unit.Value;
    }
}