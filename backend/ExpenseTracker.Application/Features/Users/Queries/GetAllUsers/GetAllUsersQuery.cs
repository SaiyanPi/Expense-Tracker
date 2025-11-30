using ExpenseTracker.Application.DTOs.Auth;
using MediatR;

namespace ExpenseTracker.Application.Features.Users.Queries.GetAllUsers;

public record GetAllUsersQuery() : IRequest<IReadOnlyList<UserDto>>;