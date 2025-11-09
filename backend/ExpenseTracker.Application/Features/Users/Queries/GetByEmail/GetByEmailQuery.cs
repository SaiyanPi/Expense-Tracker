using ExpenseTracker.Application.DTOs.User;
using MediatR;

namespace ExpenseTracker.Application.Features.Users.Queries.GetByEmail;

public record GetByEmailQuery(string Email) : IRequest<UserDto>;
