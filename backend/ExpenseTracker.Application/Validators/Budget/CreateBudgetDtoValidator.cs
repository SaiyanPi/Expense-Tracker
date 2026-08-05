using ExpenseTracker.Application.Features.Budgets.Commands.CreateBudget;
using FluentValidation;

public class CreateBudgetCommandValidator : AbstractValidator<CreateBudgetCommand>
{
    public CreateBudgetCommandValidator()
    {
        RuleFor(x => x.CreateBudgetDto.Name)
            .NotEmpty().WithMessage("Name is required")
            .MinimumLength(3).WithMessage("Name must be at least 3 characters long");

        RuleFor(x => x.CreateBudgetDto.Amount)
            .GreaterThan(100).WithMessage("Budget amount must be greater than hundred.");

        RuleFor(x => x.CreateBudgetDto.StartDate)
        .NotEmpty().WithMessage("Start date is required.")
        .Must(startDate => startDate >= DateTime.UtcNow.Date).WithMessage("Start date cannot be in the past.")
        .LessThan(x => x.CreateBudgetDto.EndDate).WithMessage("Start date must be earlier than end date.");

        RuleFor(x => x.CreateBudgetDto.EndDate)
            .NotEmpty().WithMessage("End date is required.")
            .GreaterThan(x => x.CreateBudgetDto.StartDate).WithMessage("End date must be later than start date.");

        // Apply rule only when UserId is provided (not null or empty)
        When(x => !string.IsNullOrWhiteSpace(x.CreateBudgetDto.UserId), () =>
        {
            RuleFor(x => x.CreateBudgetDto.UserId!)
                .Must(BeAValidGuid)
                .WithMessage("UserId must be a valid GUID when provided.");
        });
        
        RuleFor(x => x.CreateBudgetDto.CategoryId)
            .Must(categoryId => categoryId == null || categoryId != Guid.Empty)
            .WithMessage("CategoryId must be a valid non-empty GUID when provided.");
    }

    private bool BeAValidGuid(string userId)
        => Guid.TryParse(userId, out _);

}