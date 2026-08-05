using ExpenseTracker.Application.Features.Categories.Commands.UpdateCategory;
using FluentValidation;

namespace ExpenseTracker.Application.Validators.Category;

public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator()
    {

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(50).WithMessage("Name cannot exceed 50 characters");

    }
}
