using ExpenseTracker.Application.Common.Observability.Metrics;
using ExpenseTracker.Application.DTOs.Expense;
using MediatR;

namespace ExpenseTracker.Application.Features.Expenses.Commands.CreateExpense;

public record CreateExpenseCommand(CreateExpenseDto CreateExpenseDto) 
    : IRequest<ExpenseDto>, ITrackBusinessLatencyAndSuccess
{
    public string OperationName => BusinessOperationNames.CreateExpense;
}