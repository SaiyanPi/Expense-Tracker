using ExpenseTracker.Application.DTOs.Auth;
using MediatR;

namespace ExpenseTracker.Application.Features.Identity.Commands.Update;

public record UpdateUserCommand(string FullName, string PhoneNumber) : IRequest<Unit>
{
    public required string UserId {get; init; }
}














