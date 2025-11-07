using ExpenseTracker.Application.DTOs.User;
using MediatR;

namespace ExpenseTracker.Application.Features.Identity.Commands.Login;

public record LoginUserCommand(LoginUserDto LoginUserDto) : IRequest<AuthResultDto>;