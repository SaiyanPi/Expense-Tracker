using ExpenseTracker.Application.Common.Observability.Metrics;
using ExpenseTracker.Application.DTOs.Budget;
using MediatR;

namespace ExpenseTracker.Application.Features.Budgets.Commands.CreateBudget;

public record CreateBudgetCommand(CreateBudgetDto CreateBudgetDto)
    : IRequest<BudgetDto>, ITrackBusinessLatencyAndSuccess
{
    public string OperationName => BusinessOperationNames.CreateBudget;
}