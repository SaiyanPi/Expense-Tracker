using ExpenseTracker.Application.DTOs.Auth;
using MediatR;

namespace ExpenseTracker.Application.Features.Identity.Commands.Logout;

public record LogoutUserCommand() : IRequest<Unit>;