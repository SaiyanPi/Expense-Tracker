using MediatR;

namespace ExpenseTracker.Application.Features.Identity.Commands.DeleteMyAccount;

public record DeleteProfileCommand() : IRequest<Unit>;
