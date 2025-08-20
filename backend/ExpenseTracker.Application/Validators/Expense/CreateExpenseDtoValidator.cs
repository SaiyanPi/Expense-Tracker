using System.Data;
using ExpenseTrackler.Application.DTOs.Expense;
using FluentValidation;

namespace ExpenseTracker.Application.Validators.Expense;

public class CreateExpenseDtoValidator : AbstractValidator<CreateExpenseDto>
{
    public CreateExpenseDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MinimumLength(50).WithMessage("Title must be at least 50 characters long");

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
            .NotEmpty().WithMessage("CategoryId is required");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required");
    }
}
