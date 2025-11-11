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

        RuleFor(x => x.UserId)
            .Must(id => string.IsNullOrEmpty(id) || Guid.TryParse(id, out _))
            .WithMessage("UserId must be either null, empty, or a valid GUID");
    }
}
