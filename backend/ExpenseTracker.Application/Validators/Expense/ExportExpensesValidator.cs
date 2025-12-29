using ExpenseTracker.Application.Features.Expenses.Queries.ExportExpenses;
using FluentValidation;

namespace ExpenseTracker.Application.Validators.Expense;

public class ExportExpensesValidator : AbstractValidator<ExportExpensesQuery>
{
    public ExportExpensesValidator()
    {
        RuleFor(x => x.startDate)
            .NotEmpty().WithMessage("Start date is required in format YYYY-MM-DD.");

        RuleFor(x => x.endDate)
            .NotEmpty().WithMessage("End date is required in format YYYY-MM-DD.")
            .GreaterThanOrEqualTo(x => x.startDate).WithMessage("End date must be later than or equal to start date.");

        RuleFor(x => x.Format)
            .NotEmpty().WithMessage("Format is required")
            .Must(format => format.Equals("csv", StringComparison.OrdinalIgnoreCase) ||
                format.Equals("xlsx", StringComparison.OrdinalIgnoreCase) ||
                format.Equals("pdf", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Format must be either 'csv' or 'xlsx' or 'pdf'.");
    }
}