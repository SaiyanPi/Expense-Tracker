using ExpenseTracker.Application.Features.Budgets.Commands.CreateBudget;
using FluentValidation;

public class CreateBudgetCommandValidator : AbstractValidator<CreateBudgetCommand>
{
    public CreateBudgetCommandValidator()
    {
        RuleFor(x => x.CreateBudgetDto.Name)
            .NotEmpty().WithMessage("Name is required")
            .MinimumLength(3).WithMessage("Name must be at least 3 characters long")
            .OverridePropertyName("Name");

        RuleFor(x => x.CreateBudgetDto.Amount)
            .GreaterThan(100).WithMessage("Budget amount must be greater than hundred.")
            .OverridePropertyName("Amount");

        RuleFor(x => x.CreateBudgetDto.StartDate)
            .NotEmpty().WithMessage("Start date is required.")
            .Must(startDate => startDate >= DateTime.UtcNow.Date).WithMessage("Start date cannot be in the past.")
            .LessThan(x => x.CreateBudgetDto.EndDate).WithMessage("Start date must be earlier than end date.")
            .OverridePropertyName("StartDate");

        RuleFor(x => x.CreateBudgetDto.EndDate)
            .NotEmpty().WithMessage("End date is required.")
            .GreaterThan(x => x.CreateBudgetDto.StartDate).WithMessage("End date must be later than start date.")
            .OverridePropertyName("EndDate");

        // Apply rule only when UserId is provided (not null or empty)
        When(x => !string.IsNullOrWhiteSpace(x.CreateBudgetDto.UserId), () =>
        {
            RuleFor(x => x.CreateBudgetDto.UserId!)
                .Must(BeAValidGuid)
                .WithMessage("UserId must be a valid GUID when provided.")
                .OverridePropertyName("UserId");
        });
        
        // If CategoryId is provided validate for Guid
        RuleFor(x => x.CreateBudgetDto.CategoryId)
            .Must(categoryId => categoryId != Guid.Empty)
            .When(x => x.CreateBudgetDto.CategoryId.HasValue)
            .WithMessage("Category Id must be a valid GUID when provided.")
            .OverridePropertyName("CategoryId");
    }

    private bool BeAValidGuid(string userId)
        => Guid.TryParse(userId, out _);

}