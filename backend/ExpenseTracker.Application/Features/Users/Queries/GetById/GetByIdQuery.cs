using ExpenseTracker.Application.DTOs.Auth;
using MediatR;

namespace ExpenseTracker.Application.Features.Users.Queries.GetById;

public record GetByIdQuery(string Id) : IRequest<UserDto>;