using ExpenseTracker.Application.Common.Observability.Metrics;
using ExpenseTracker.Application.Common.ProfileImage;
using ExpenseTracker.Application.DTOs.Auth;
using MediatR;

namespace ExpenseTracker.Application.Features.Identity.Commands.UpdateProfileImage;

// public record UpdateProfileImageCommand(Stream Image, string FileName) : IRequest<Unit>;
public record UpdateProfileImageCommand(ProfileImageUpdate ImageUpdate) : IRequest<UserDto>;