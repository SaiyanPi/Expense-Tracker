using MediatR;

namespace ExpenseTracker.Application.Features.Identity.Commands.DeleteMyAccount;

public record DeleteProfileCommand(string UserId) : IRequest<Unit>;
