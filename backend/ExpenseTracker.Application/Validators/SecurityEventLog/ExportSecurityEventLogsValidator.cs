using ExpenseTracker.Application.Features.AuditLogs.Query.ExportAuditLogs;
using ExpenseTracker.Application.Features.SecurityEventLogs.Query.ExportSecurityEventLogs;
using ExpenseTracker.Domain.SharedKernel;
using FluentValidation;

namespace ExpenseTracker.Application.Validators.Expense;

public class ExportSecurityEventLogsValidator : AbstractValidator<ExportSecurityEventLogsQuery>
{
    public ExportSecurityEventLogsValidator()
    {
        // userId VALIDATION IS DONE IN THE HANDLER SIDE-----
        
        RuleFor(x => x)
            .Must(x => !x.Filter.StartDate.HasValue || !x.Filter.EndDate.HasValue || 
                x.Filter.StartDate <= x.Filter.EndDate)
            .WithMessage("StartDate must be less than or equal to EndDate. Date Format(YYYY-MM-DD)");

         RuleFor(x => x.Filter.EventType)
            .Must(value =>
            {
                if (string.IsNullOrWhiteSpace(value))
                    return true;

                // Try parse (case-insensitive)
                if (!Enum.TryParse<SecurityEventTypes>(value, ignoreCase: true, out var parsed))
                    return false;

                // Reject undefined numeric values
                return Enum.IsDefined(typeof(SecurityEventTypes), parsed);
            })
            .WithMessage(x =>
            {
                var mappings = string.Join(", ", Enum.GetValues<SecurityEventTypes>()
                    .Select(e => $"{(int)e} for {e}"));
                return $"Invalid event type. Valid values: {mappings}.";
            });

        RuleFor(x => x.Filter.Outcome)
            .Must(value =>
            {
                if (string.IsNullOrWhiteSpace(value))
                    return true;

                if (!Enum.TryParse<SecurityEventOutcome>(value, true, out var parsed))
                    return false;

                return Enum.IsDefined(typeof(SecurityEventOutcome), parsed);
            })
            .WithMessage(x =>
            {
                var mappings = string.Join(", ", Enum.GetValues<SecurityEventOutcome>()
                    .Select(e => $"{(int)e} for {e}"));
                return $"Invalid outcome. Valid values: {mappings}.";
            });

        RuleFor(x => x.Format)
            .NotEmpty().WithMessage("Format is required")
            .Must(format => format.Equals("csv", StringComparison.OrdinalIgnoreCase) ||
                format.Equals("xlsx", StringComparison.OrdinalIgnoreCase) ||
                format.Equals("pdf", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Format must be either 'csv' or 'xlsx' or 'pdf'.");
    }
}