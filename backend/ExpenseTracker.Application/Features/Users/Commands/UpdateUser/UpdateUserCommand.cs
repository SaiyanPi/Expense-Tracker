using ExpenseTracker.Application.DTOs.User;
using MediatR;

namespace ExpenseTracker.Application.Features.Users.Commands.UpdateUser;

public record UpdateUserCommand(UpdateUserDto UpdateUserDto) : IRequest<Unit>;















// Unlike Domain entities(category and expense), we use dtos for updating Identity/User entities.



// WHY UPDATE OPERATION IS NOT INCLUDED INSIDE IDENTITY FOLDER?

// Depending on what we're updating, UpdateProfile can be considered either Identity operation or
// domain operation. Since we're only updating 'FullName', it's not included inside the Identity
// folder