using ExpenseTracker.Application.DTOs.User;
using MediatR;

namespace ExpenseTracker.Application.Features.Users.Commands.Register;

public record RegisterUserCommand(RegisterUserDto RegisterUserDto) : IRequest<AuthResultDto>;