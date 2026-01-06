using ExpenseTracker.Application.Features.AuditLogs.Query.GetAllAuditLogs;
using ExpenseTracker.Domain.SharedKernel;
using FluentValidation;

namespace ExpenseTracker.Application.Validators.AuditLog;

public class FilterAuditLogsValidator : AbstractValidator<GetAuditLogsQuery>
{
    public FilterAuditLogsValidator()
    {
        // userId VALIDATION IS DONE IN THE HANDLER-----
        
        RuleFor(x => x)
            .Must(x => !x.Filter.StartDate.HasValue || !x.Filter.EndDate.HasValue || 
                x.Filter.StartDate <= x.Filter.EndDate)
            .WithMessage("StartDate must be less than or equal to EndDate. Date Format(YYYY-MM-DD)");

        RuleFor(x => x.Filter.Action)
            .Must(action => !action.HasValue || Enum.IsDefined(typeof(AuditAction), action.Value))
            .WithMessage(x =>
            {
                var mappings = string.Join(", ", Enum.GetValues(typeof(AuditAction))
                    .Cast<AuditAction>()
                    .Select(e => $"{(int)e} for {e}"));
                return $"Invalid action. Valid values: {mappings}.";
            });
        
        // EntityName validation
        var allowedEntities = new[]
        {
            nameof(Category),
            nameof(Budget),
            nameof(Expense)
        };

        RuleFor(x => x.Filter.EntityName)
            .Must(e => string.IsNullOrWhiteSpace(e) || allowedEntities.Contains(e, StringComparer.OrdinalIgnoreCase))
            .WithMessage(x =>
                $"Invalid entity. Entity can be either: {string.Join(", ", allowedEntities)}. Leave empty to include all entities.");

    }
}