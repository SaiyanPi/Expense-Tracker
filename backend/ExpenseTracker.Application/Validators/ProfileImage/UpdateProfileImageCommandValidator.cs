using ExpenseTracker.Application.Features.Identity.Commands.UpdateProfileImage;
using FluentValidation;

namespace ExpenseTracker.Application.Validators.ProfileImage;

public sealed class UpdateProfileImageCommandValidator
    : AbstractValidator<UpdateProfileImageCommand>
{
    private const long MaxFileSize = 5 * 1024 * 1024;

    public UpdateProfileImageCommandValidator()
    {
        RuleFor(x => x.ImageUpdate)
            .NotNull()
            .WithMessage("Profile image is required.");

        When(x => x.ImageUpdate is not null, () =>
        {
            RuleFor(x => x.ImageUpdate.Length)
                .LessThanOrEqualTo(MaxFileSize)
                .WithMessage("Profile image cannot exceed 5 MB.");

            RuleFor(x => x.ImageUpdate.FileName)
                .Must(HaveAllowedExtension)
                .WithMessage("Only JPG, JPEG, and PNG images are allowed.");
        });
    }

    private static bool HaveAllowedExtension(string? fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return false;

        var extension = Path.GetExtension(fileName);

        return extension.Equals(".jpg", StringComparison.OrdinalIgnoreCase)
            || extension.Equals(".jpeg", StringComparison.OrdinalIgnoreCase)
            || extension.Equals(".png", StringComparison.OrdinalIgnoreCase);
    }
}