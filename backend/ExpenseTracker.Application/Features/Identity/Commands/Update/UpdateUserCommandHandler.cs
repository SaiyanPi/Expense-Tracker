using AutoMapper;
using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Application.DTOs.Auth;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;

namespace ExpenseTracker.Application.Features.Identity.Commands.Update;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Unit>
{
    private readonly IIdentityService _identityService;
    private readonly IMapper _mapper;

    public UpdateUserCommandHandler(IIdentityService identityService, IMapper mapper)
    {
        _identityService = identityService;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var dto = new UpdateUserDto
        {
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber
        };

        await _identityService.UpdateAsync(request.UserId, dto, cancellationToken);
        return Unit.Value;
    }
}


