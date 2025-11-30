using AutoMapper;
using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.DTOs.Auth;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ExpenseTracker.Application.Features.Users.Queries.GetByEmail;

public class GetByEmailQueryHandler : IRequestHandler<GetByEmailQuery, UserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetByEmailQueryHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<UserDto> Handle(GetByEmailQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user == null)
            throw new NotFoundException(nameof(UserDto), request.Email);

        return _mapper.Map<UserDto>(user);
    }
}