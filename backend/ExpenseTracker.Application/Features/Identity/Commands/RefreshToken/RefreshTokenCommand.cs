using ExpenseTracker.Application.DTOs.Auth;
using MediatR;

namespace ExpenseTracker.Application.Features.Identity.Commands.RefreshToken;

public record RefreshTokenCommand(RefreshTokenDto RefreshTokenDto) : IRequest<AuthResultDto>;