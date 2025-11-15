using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Application.DTOs.User;
using MediatR;


namespace ExpenseTracker.Application.Features.Identity.Commands.Register;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, AuthResultDto>
{
    private readonly IIdentityService _identityService;
    public RegisterUserCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<AuthResultDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        => await _identityService.RegisterUserAsync(request.RegisterUserDto, request.Role, cancellationToken);
   
}
