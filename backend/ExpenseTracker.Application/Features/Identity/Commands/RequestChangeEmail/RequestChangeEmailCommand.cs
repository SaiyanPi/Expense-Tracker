using ExpenseTracker.Application.DTOs.Auth;
using MediatR;

namespace ExpenseTracker.Application.Features.Identity.Commands.RequestChangeEmail;

public record RequestChangeEmailCommand(ChangeEmailRequestDto ChangeEmailRequestDto) : IRequest<Unit>;