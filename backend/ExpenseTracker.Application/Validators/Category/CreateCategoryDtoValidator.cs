using ExpenseTracker.Application.DTOs.Category;
using FluentValidation;

namespace ExpenseTracker.Application.Validators.Category;
public class CreateCategoryDtoValidator : AbstractValidator<CreateCategoryDto>
{
    public CreateCategoryDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MinimumLength(3).WithMessage("Name must be at least 3 characters long");

        // Apply rule only when UserId is provided (not null or empty)
        When(x => !string.IsNullOrWhiteSpace(x.UserId), () =>
        {
            RuleFor(x => x.UserId!)
                .Must(BeAValidGuid)
                .WithMessage("UserId must be a valid GUID when provided.");
        });
    }

    private bool BeAValidGuid(string userId)
        => Guid.TryParse(userId, out _);
}
