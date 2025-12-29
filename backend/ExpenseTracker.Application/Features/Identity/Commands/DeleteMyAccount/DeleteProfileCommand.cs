using MediatR;

namespace ExpenseTracker.Application.Features.Identity.Commands.DeleteMyAccount;

public record DeleteProfileCommand() : IRequest<Unit>
{
    // Optional: if null, handler will decide based on admin or not
    public string? UserId { get; set; }

}
