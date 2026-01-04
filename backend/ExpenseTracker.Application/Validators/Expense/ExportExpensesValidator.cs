using ExpenseTracker.Application.Features.Expenses.Queries.ExportExpenses;
using FluentValidation;

namespace ExpenseTracker.Application.Validators.AuditLog;

public class ExportExpensesValidator : AbstractValidator<ExportExpensesQuery>
{
    public ExportExpensesValidator()
    {
        RuleFor(x => x.Format)
            .NotEmpty().WithMessage("Format is required")
            .Must(format => format.Equals("csv", StringComparison.OrdinalIgnoreCase) ||
                format.Equals("xlsx", StringComparison.OrdinalIgnoreCase) ||
                format.Equals("pdf", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Format must be either 'csv' or 'xlsx' or 'pdf'.");
            
        // userId and categoryId VALIDATION IS DONE IN THE HANDLER-----
        
        RuleFor(x => x)
            .Must(x => !x.Filter.StartDate.HasValue || !x.Filter.EndDate.HasValue || 
                x.Filter.StartDate <= x.Filter.EndDate)
            .WithMessage("StartDate must be less than or equal to EndDate. Date Format(YYYY-MM-DD)");

        
        RuleFor(x => x)
                .Must(x => !x.Filter.MinAmount.HasValue || !x.Filter.MaxAmount.HasValue || 
                    x.Filter.MinAmount <= x.Filter.MaxAmount)
                .WithMessage("MinAmount must be less than or equal to MaxAmount");


    }
}