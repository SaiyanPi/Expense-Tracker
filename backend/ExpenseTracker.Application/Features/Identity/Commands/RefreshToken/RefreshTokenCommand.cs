using ExpenseTracker.Application.DTOs.User;
using MediatR;

namespace ExpenseTracker.Application.Features.Identity.Commands.RefreshToken;

public record RefreshTokenCommand(RefreshTokenDto RefreshTokenDto) : IRequest<AuthResultDto>;