using ExpenseTracker.Application.DTOs.Expense;
using ExpenseTracker.Application.Features.Expenses.Commands.CreateExpense;
using FluentValidation;

namespace ExpenseTracker.Application.Validators.Expense;

public class CreateExpenseCommandValidator : AbstractValidator<CreateExpenseCommand>
{
    public CreateExpenseCommandValidator()
    {
        RuleFor(x => x.CreateExpenseDto.Title)
            .NotEmpty().WithMessage("Title is required")
            .MinimumLength(3).WithMessage("Title must be at least 3 characters long")
            .OverridePropertyName("Title");

        RuleFor(x => x.CreateExpenseDto.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters")
            .OverridePropertyName("Description");

        RuleFor(x => x.CreateExpenseDto.Amount)
            .NotEmpty().WithMessage("Amount is required")
            .GreaterThan(0).WithMessage("Amount must be greater than zero")
            .OverridePropertyName("Amount");

        RuleFor(x => x.CreateExpenseDto.Date)
            .LessThanOrEqualTo(DateTime.Now)
            .When(x => x.CreateExpenseDto.Date.HasValue)
            .WithMessage("Date must be in the past or present")
            .OverridePropertyName("Date");

        RuleFor(x => x.CreateExpenseDto.CategoryId)
            .NotEmpty().WithMessage("CategoryId is required")
            .OverridePropertyName("CategoryId");

        // Apply rule only when BudgetId is provided(not null or empty)
        RuleFor(x => x.CreateExpenseDto.BudgetId)
            .Must(budgetId => budgetId == null || budgetId != Guid.Empty)
            .WithMessage("BudgetId must be a valid GUID when provided.")
            .OverridePropertyName("BudgetId");

        // // Apply rule only when UserId is provided (not null or empty)
        // When(x => !string.IsNullOrWhiteSpace(x.CreateExpenseDto.UserId), () =>
        // {
        //     RuleFor(x => x.CreateExpenseDto.UserId!)
        //         .Must(BeAValidGuid)
        //         .WithMessage("UserId must be a valid GUID when provided.");
        // });
    }
    private bool BeAValidGuid(string userId)
        => Guid.TryParse(userId, out _);
}
