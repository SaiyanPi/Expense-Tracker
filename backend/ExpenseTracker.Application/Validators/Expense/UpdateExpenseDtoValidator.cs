using ExpenseTracker.Application.DTOs.Expense;
using FluentValidation;

namespace ExpenseTracker.Application.Validators.Expense;

public class UpdateExpenseDtoValidator : AbstractValidator<UpdateExpenseDto>
{
    public UpdateExpenseDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MinimumLength(3).WithMessage("Title must be at least 3 characters long");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

        RuleFor(x => x.Amount)
            .NotEmpty().WithMessage("Amount is required")
            .GreaterThan(0).WithMessage("Amount must be greater than zero");

        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("Date is required")
            .LessThanOrEqualTo(DateTime.Now).WithMessage("Date must be in the past or present");

        RuleFor(x => x.CategoryId)
            .Must(categoryId => categoryId == null || categoryId != Guid.Empty)
            .WithMessage("CategoryId must be a valid non-empty GUID when provided.");
        
        RuleFor(x => x.BudgetId)
            .Must(budgetId => budgetId == null || budgetId != Guid.Empty)
            .WithMessage("BudgetId must be a valid non-empty GUID when provided.");

    }
}
