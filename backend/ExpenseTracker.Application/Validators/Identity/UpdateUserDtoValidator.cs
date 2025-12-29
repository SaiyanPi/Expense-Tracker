using ExpenseTracker.Application.DTOs.Auth;
using FluentValidation;

namespace ExpenseTracker.Application.Validators.Identity;

public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
{
    public UpdateUserDtoValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MinimumLength(5).WithMessage("Name must be at least 5 characters long.")
            .MaximumLength(20).WithMessage("Name must be at most 20 characters long.");

    }
}