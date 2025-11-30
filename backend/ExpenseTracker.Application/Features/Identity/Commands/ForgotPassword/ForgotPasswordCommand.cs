using ExpenseTracker.Application.DTOs.Auth;
using MediatR;

namespace ExpenseTracker.Application.Features.Identity.Commands.ForgotPassword;

public record ForgotPasswordCommand(ForgotPasswordDto ForgotPasswordDto) : IRequest<Unit>;