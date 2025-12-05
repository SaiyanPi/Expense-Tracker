using ExpenseTracker.Application.DTOs.Auth;
using MediatR;

namespace ExpenseTracker.Application.Features.Identity.Commands.ConfirmPhone;

public record ConfirmPhoneCommand(VerifyPhoneDto VerifyPhoneDto) : IRequest<Unit>;