using ExpenseTracker.Application.DTOs.Auth;
using MediatR;

namespace ExpenseTracker.Application.Features.Identity.Commands.ConfirmChangeEmail;

public record ConfirmChangeEmailCommand(ConfirmChangeEmailDto ConfirmChangeEmailDto) : IRequest<Unit>;