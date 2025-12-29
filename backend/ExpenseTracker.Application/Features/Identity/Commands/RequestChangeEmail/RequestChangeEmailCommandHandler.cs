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
    private readonly IUserAccessor _userAccessor;
    private readonly IUserRoleService _userRoleService;


    public RequestChangeEmailCommandHandler(
        IIdentityService identityService,
        IUserAccessor userAccessor,
        IUserRoleService userRoleService)
    {
        _identityService = identityService;
        _userAccessor = userAccessor;
        _userRoleService =userRoleService;
    }

    public async Task<Unit> Handle(RequestChangeEmailCommand request, CancellationToken cancellationToken)
    {
        var userId = _userAccessor.UserId;

        var userEmail = _userAccessor.UserEmail;

        await _identityService.RequestChangeEmailAsync(userId, request.ChangeEmailRequestDto, cancellationToken);
        return Unit.Value;
       
    }
}