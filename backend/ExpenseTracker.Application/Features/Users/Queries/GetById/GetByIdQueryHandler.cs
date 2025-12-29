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
    private readonly IUserRoleService _userRoleService;
    private readonly IMapper _mapper;

    public GetByIdQueryHandler(
        IUserRepository userRepository,
        IUserAccessor userAccessor,
        IUserRoleService userRoleService,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _userAccessor = userAccessor;
        _userRoleService = userRoleService;
        _mapper = mapper;
    }

    public async Task<UserDto> Handle(GetByIdQuery request, CancellationToken cancellationToken)
    {
        // BUISNESS RULE:
        // if the user is admin and id is not provided in the query, redirect to admin's profile
        // if the user is admin and id is provided in the query, redirect to id's profile
        // if the user is regular user redirect to his profile

        var userId = _userAccessor.UserId;

        var isAdmin = await _userRoleService.IsAdminAsync(userId);
        string targetUserId;

        if (isAdmin)
        {
            targetUserId = string.IsNullOrEmpty(request.UserId) ? userId : request.UserId!;
        }
        else
        {
            targetUserId = userId;
        }

        var user = await _userRepository.GetByIdAsync(targetUserId, cancellationToken);
        if (user == null)
            throw new NotFoundException(nameof(UserDto), targetUserId);

        return _mapper.Map<UserDto>(user);
    }
}