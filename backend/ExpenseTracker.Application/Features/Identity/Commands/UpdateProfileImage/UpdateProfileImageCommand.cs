using MediatR;

namespace ExpenseTracker.Application.Features.Identity.Commands.UpdateProfileImage;

public record UpdateProfileImageCommand(Stream Image, string FileName) : IRequest<Unit>;