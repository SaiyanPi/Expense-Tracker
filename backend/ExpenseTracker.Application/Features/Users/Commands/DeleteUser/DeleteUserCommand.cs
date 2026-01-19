using MediatR;

namespace ExpenseTracker.Application.Features.Users.Commands.DeleteUser;

public record DeleteUserCommand(string UserId) : IRequest<Unit>;