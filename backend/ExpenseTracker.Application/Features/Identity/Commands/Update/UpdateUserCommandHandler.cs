using AutoMapper;
using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Application.DTOs.Auth;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ExpenseTracker.Application.Features.Identity.Commands.Update;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Unit>
{
    private readonly IIdentityService _identityService;
    private readonly IUserAccessor _userAccessor;
    private readonly IUserRoleService _userRoleService;
    private readonly IMapper _mapper;

    public UpdateUserCommandHandler(
        IIdentityService identityService,
        IUserAccessor userAccessor,
        IUserRoleService userRoleService,
        IMapper mapper)
    {
        _identityService = identityService;
        _userAccessor = userAccessor;
        _userRoleService = userRoleService;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        // BUISNESS RULE:
        // regardless of role, redirect to update (userManagement doesn't have update endpoint)

        var userId = _userAccessor.UserId;

        var dto = new UpdateUserDto
        {
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber
        };

        await _identityService.UpdateAsync(userId, dto, cancellationToken);
        return Unit.Value;
    }
}


