using System.Text;
using AutoMapper;
using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;

namespace ExpenseTracker.Application.Features.Identity.Commands.RequestChangeEmail;

public class RequestChangeEmailCommandHandler : IRequestHandler<RequestChangeEmailCommand, Unit>
{
    private readonly IIdentityService _identityService;
    private readonly IUserRepository _userRepository;

    public RequestChangeEmailCommandHandler(IIdentityService identityService,
        IUserRepository userRepository)
    {
        _identityService = identityService;
        _userRepository = userRepository;
    }

    public async Task<Unit> Handle(RequestChangeEmailCommand request, CancellationToken cancellationToken)
    {
        await _identityService.RequestChangeEmailAsync(request.ChangeEmailRequestDto);
        return Unit.Value;
       
    }
}