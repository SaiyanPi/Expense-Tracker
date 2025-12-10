using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Application.Features.Identity.Commands.DeleteMyAccount;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;

namespace ExpenseTracker.Application.Features.Users.Commands.DeleteUser;

public record DeleteProfileCommandHandler : IRequestHandler<DeleteProfileCommand, Unit>
{
    private readonly IIdentityService _identityService;

    public DeleteProfileCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }       

    public async Task<Unit> Handle(DeleteProfileCommand request, CancellationToken cancellationToken)
    {
        await _identityService.DeleteAsync(request.UserId, cancellationToken);
        return Unit.Value;
    }
}   