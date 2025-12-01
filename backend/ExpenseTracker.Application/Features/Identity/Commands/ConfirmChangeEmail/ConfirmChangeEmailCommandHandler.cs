using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;

namespace ExpenseTracker.Application.Features.Identity.Commands.ConfirmChangeEmail;

public class ConfirmChangeEmailCommandHandler : IRequestHandler<ConfirmChangeEmailCommand, Unit>
{
    private readonly IIdentityService _identityService;
    private readonly IUserRepository _userRepository;

    public ConfirmChangeEmailCommandHandler( IIdentityService identityService,
    IUserRepository userRepository)
    {
        _identityService = identityService;
        _userRepository = userRepository;
    }

    public async Task<Unit> Handle(ConfirmChangeEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.ConfirmChangeEmailDto.UserId);
        if(user == null)
            throw new NotFoundException(nameof(User), request.ConfirmChangeEmailDto.UserId);
        
        await _identityService.ConfirmChangeEmailAsync(request.ConfirmChangeEmailDto);
        return Unit.Value;
    }
}
