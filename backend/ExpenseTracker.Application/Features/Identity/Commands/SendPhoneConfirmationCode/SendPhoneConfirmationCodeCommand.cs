using ExpenseTracker.Application.DTOs.Auth;
using MediatR;

namespace ExpenseTracker.Application.Features.Identity.Commands.SendPhoneConfirmationCode;

public record SendPhoneConfirmationCodeCommand(PhoneConfirmationDto PhoneConfirmationDto) : IRequest<Unit>;