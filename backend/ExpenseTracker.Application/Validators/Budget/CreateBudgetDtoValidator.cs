using ExpenseTracker.Application.DTOs.Budget;
using FluentValidation;

public class CreateBudgetDtoValidator : AbstractValidator<CreateBudgetDto>
{
    public CreateBudgetDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MinimumLength(3).WithMessage("Name must be at least 3 characters long");

        RuleFor(x => x.Amount)
            .GreaterThan(100).WithMessage("Budget amount must be greater than hundred.");

        RuleFor(x => x.StartDate)
            .LessThan(x => x.EndDate).WithMessage("Start date must be earlier than end date.");

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate).WithMessage("End date must be later than start date.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");
        
        RuleFor(x => x.CategoryId)
            .Must(categoryId => categoryId == null || categoryId != Guid.Empty)
            .WithMessage("CategoryId must be a valid non-empty GUID when provided.");
    }

}