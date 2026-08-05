using ExpenseTracker.Application.Features.Categories.Commands.CreateCategory;
using FluentValidation;

namespace ExpenseTracker.Application.Validators.Category;
public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.CreateCategoryDto.Name)
            .NotEmpty().WithMessage("Name is required")
            .MinimumLength(3).WithMessage("Name must be at least 3 characters long")
            .OverridePropertyName("Name");;

        // Apply rule only when UserId is provided (not null or empty)
        When(x => !string.IsNullOrWhiteSpace(x.CreateCategoryDto.UserId), () =>
        {
            RuleFor(x => x.CreateCategoryDto.UserId!)
                .Must(BeAValidGuid)
                .WithMessage("UserId must be a valid GUID when provided.")
                .OverridePropertyName("UserId");;
        });
    }

    private bool BeAValidGuid(string userId)
        => Guid.TryParse(userId, out _);
}
