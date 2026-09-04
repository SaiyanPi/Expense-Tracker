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

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("PhoneNumber is required.")
            .Matches(@"^(?:\+977)?9[678]\d{8}$")
            .WithMessage("Phone number must contain only digits and may start with '+'.");
    }
}