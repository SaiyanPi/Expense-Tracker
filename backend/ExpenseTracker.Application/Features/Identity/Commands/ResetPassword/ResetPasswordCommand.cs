using ExpenseTracker.Application.DTOs.Auth;
using MediatR;

namespace ExpenseTracker.Application.Features.Identity.Commands.ResetPassword;

public record ResetPasswordCommand( string UserId, string Token, ResetPasswordDto ResetPasswordDto) 
    : IRequest<Unit>;