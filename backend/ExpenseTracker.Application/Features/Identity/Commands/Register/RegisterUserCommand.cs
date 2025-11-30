using ExpenseTracker.Application.DTOs.Auth;
using MediatR;

namespace ExpenseTracker.Application.Features.Identity.Commands.Register;

public record RegisterUserCommand(RegisterUserDto RegisterUserDto, string Role) : IRequest<AuthResultDto>;