using ExpenseTracker.Application.Features.AuditLogs.Query.GetAllAuditLogs;
using ExpenseTracker.Application.Features.SecurityEventLogs.Query.GetAllSecurityEventLogs;
using ExpenseTracker.Domain.SharedKernel;
using FluentValidation;

namespace ExpenseTracker.Application.Validators.SecurityEventLog;

public class FilterSecurityEventLogsValidator : AbstractValidator<GetAllSecurityEventLogsQuery>
{
    public FilterSecurityEventLogsValidator()
    {
        // userId and userEmail VALIDATION IS DONE IN THE HANDLER SIDE-----
        
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
    }
}