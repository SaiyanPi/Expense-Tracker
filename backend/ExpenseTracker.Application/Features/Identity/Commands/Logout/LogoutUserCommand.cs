using ExpenseTracker.Application.DTOs.User;
using MediatR;

namespace ExpenseTracker.Application.Features.Identity.Commands.Logout;

public record LogoutUserCommand(LogoutUserDto LogoutUserDto) : IRequest<Unit>;