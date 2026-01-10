using ExpenseTracker.Application.Features.AuditLogs.Query.GetAllAuditLogs;
using ExpenseTracker.Application.Features.AuditLogs.Query.GetAuditTimelineByEntityNameAndEntityId;
using ExpenseTracker.Domain.SharedKernel;
using FluentValidation;

namespace ExpenseTracker.Application.Validators.AuditLog;

public class GetAuditTimelineByEntityNameAndIdValidator : AbstractValidator<GetAuditTimelineByEntityNameAndIdQuery>
{
    public GetAuditTimelineByEntityNameAndIdValidator()
    {
        // EntityId VALIDATION IS DONE IN THE HANDLER SIDE -----
        
         RuleFor(x => x.EntityName)
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


    }
}