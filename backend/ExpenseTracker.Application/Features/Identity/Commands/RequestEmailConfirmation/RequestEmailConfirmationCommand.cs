using ExpenseTracker.Application.DTOs.Auth;
using MediatR;

namespace ExpenseTracker.Application.Features.Identity.Commands.RequestEmailConfirmation;

public record RequestEmailConfirmationCommand() : IRequest<Unit>;