using ExpenseTracker.Application.Common.Pagination;
using ExpenseTracker.Application.DTOs.Auth;
using MediatR;

namespace ExpenseTracker.Application.Features.Users.Queries.GetAllUsers;

public record GetAllUsersQuery(PagedQuery Paging) : IRequest<PagedResult<UserDto>>;