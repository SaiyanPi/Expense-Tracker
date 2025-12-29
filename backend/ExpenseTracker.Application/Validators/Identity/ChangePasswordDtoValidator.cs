using ExpenseTracker.Application.DTOs.Auth;
using FluentValidation;

namespace ExpenseTracker.Application.Validators.Identity;

public class ChangePasswordDtoValidator : AbstractValidator<ChangePasswordDto>
{
    public ChangePasswordDtoValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty()
            .WithMessage("Current password is required.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("New password is required.")
            .MinimumLength(6).WithMessage("New Password must be at least 6 characters long.")
            .Matches("[A-Z]").WithMessage("New Password must contain at least one uppercase letter.")
            .Matches("[a-z]").WithMessage("New Password must contain at least one lowercase letter.")
            .Matches("[0-9]").WithMessage("New Password must contain at least one number.")
            .Matches("[^a-zA-Z0-9]").WithMessage("New Password must contain at least one special character.");
    }
}