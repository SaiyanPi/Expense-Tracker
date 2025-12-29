using ExpenseTracker.Application.DTOs.Auth;
using MediatR;

namespace ExpenseTracker.Application.Features.Users.Queries.GetById;

public record GetByIdQuery : IRequest<UserDto>
{
    // Optional: if null, handler will decide based on admin or not
    public string? UserId { get; set; }

}