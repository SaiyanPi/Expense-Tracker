using ExpenseTracker.Application.DTOs.Auth;
using MediatR;

namespace ExpenseTracker.Application.Features.Users.Queries.GetById;

public record GetByIdQuery : IRequest<UserDto>
{
    // This is used by user mgmt controller and profile controller. For user mgmt, userId will be provided and 
    // for profile controller, it will be null
    public string? UserId { get; set; }

}