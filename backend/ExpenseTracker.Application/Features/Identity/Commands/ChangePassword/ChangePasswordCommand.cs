using ExpenseTracker.Application.DTOs.Auth;
using MediatR;

namespace ExpenseTracker.Application.Features.Identity.Commands.ChangePassword;

public record ChangePasswordCommand(ChangePasswordDto ChangePasswordDto) : IRequest<Unit>;