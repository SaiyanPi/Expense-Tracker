using ExpenseTracker.Application.Features.AuditLogs.Query.GetAllAuditLogs;
using ExpenseTracker.Domain.SharedKernel;
using FluentValidation;

namespace ExpenseTracker.Application.Validators.AuditLog;

public class FilterAuditLogsValidator : AbstractValidator<GetAuditLogsQuery>
{
    public FilterAuditLogsValidator()
    {
        // userId VALIDATION IS DONE IN THE HANDLER SIDE-----
        
        RuleFor(x => x)
            .Must(x => !x.Filter.StartDate.HasValue || !x.Filter.EndDate.HasValue || 
                x.Filter.StartDate <= x.Filter.EndDate)
            .WithMessage("StartDate must be less than or equal to EndDate. Date Format(YYYY-MM-DD)");

        RuleFor(x => x.Filter.EntityName)
            .Must(value =>
            {
                if (string.IsNullOrWhiteSpace(value))
                    return true;

                // Try parse (case-insensitive)
                if (!Enum.TryParse<EntityType>(value, ignoreCase: true, out var parsed))
                    return false;

                // Reject undefined numeric values
                return Enum.IsDefined(typeof(EntityType), parsed);
            })
            .WithMessage(x =>
            {
                var mappings = string.Join(", ", Enum.GetValues<EntityType>()
                    .Select(e => $"{(int)e} for {e}"));
                return $"Invalid entity. Valid values: {mappings}.";
            });

        RuleFor(x => x.Filter.Action)
            .Must(value =>
            {
                if (string.IsNullOrWhiteSpace(value))
                    return true;

                if (!Enum.TryParse<AuditAction>(value, true, out var parsed))
                    return false;

                return Enum.IsDefined(typeof(AuditAction), parsed);
            })
            .WithMessage(x =>
            {
                var mappings = string.Join(", ", Enum.GetValues<AuditAction>()
                    .Select(e => $"{(int)e} for {e}"));
                return $"Invalid action. Valid values: {mappings}.";
            });
    }
}