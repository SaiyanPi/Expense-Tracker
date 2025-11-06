using AutoMapper;
using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Application.DTOs.User;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ExpenseTracker.Application.Features.Users.Commands.Register;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, AuthResultDto>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IUserRepository _domainUserRepository;
    private readonly IMapper _mapper;

    public RegisterUserCommandHandler(
        UserManager<ApplicationUser> userManager,
        IJwtTokenService jwtTokenService,
        IUserRepository domainUserRepository,
        IMapper mapper)
    {
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
        _domainUserRepository = domainUserRepository;
        _mapper = mapper;
    }

    public async Task<AuthResultDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var dto = request.RegisterUserDto;

        // 1️⃣ Create DomainUser
        var domainUser = new Domain.Entities.User
        {
            FullName = dto.FullName,
            Email = dto.Email
        };

        await _domainUserRepository.AddAsync(domainUser, cancellationToken);

        // 2️⃣ Create ApplicationUser linked to DomainUser
        var appUser = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            DomainUserId = domainUser.Id
        };

        var result = await _userManager.CreateAsync(appUser, dto.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new ValidationException(errors);
        }

        // 3️⃣ Generate JWT Token
        var authResult = await _jwtTokenService.GenerateTokenAsync(appUser);

        // 4️⃣ Return AuthResultDto
        return authResult;
    }
}
