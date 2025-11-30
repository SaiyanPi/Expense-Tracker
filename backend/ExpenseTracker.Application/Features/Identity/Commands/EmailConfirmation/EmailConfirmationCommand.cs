using ExpenseTracker.Application.DTOs.Auth;
using MediatR;

namespace ExpenseTracker.Application.Features.Identity.Commands.EmailConfirmation;

public record EmailConfirmationCommand(VerifyEmailDto VerifyEmailDto) : IRequest<Unit>;