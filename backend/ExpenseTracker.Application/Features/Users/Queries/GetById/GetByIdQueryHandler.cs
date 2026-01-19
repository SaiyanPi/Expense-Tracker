using AutoMapper;
using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Application.DTOs.Auth;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;

namespace ExpenseTracker.Application.Features.Users.Queries.GetById;

public class GetByIdQueryHandler : IRequestHandler<GetByIdQuery, UserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserAccessor _userAccessor;
    private readonly IMapper _mapper;

    public GetByIdQueryHandler(
        IUserRepository userRepository,
        IUserAccessor userAccessor,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _userAccessor = userAccessor;
        _mapper = mapper;
    }

    public async Task<UserDto> Handle(GetByIdQuery request, CancellationToken cancellationToken)
    {
        // BUISNESS RULE:
        // if the userId is provided in the request, redirect to that user
        // if the suerId is not provided in the request, rdirect to the current user(userId from token)

       // Determine target user: use requested UserId if provided, otherwise use current user's ID
        var targetUserId = string.IsNullOrEmpty(request.UserId) ? _userAccessor.UserId : request.UserId;
        
        var user = await _userRepository.GetByIdAsync(targetUserId, cancellationToken);
        if (user == null)
            throw new NotFoundException(nameof(UserDto), targetUserId);

        return _mapper.Map<UserDto>(user);
    }
}