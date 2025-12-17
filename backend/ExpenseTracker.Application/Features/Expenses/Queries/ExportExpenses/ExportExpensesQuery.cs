using ExpenseTracker.Application.DTOS.Expense;
using MediatR;

namespace ExpenseTracker.Application.Features.Expenses.Queries.ExportExpenses;

public record ExportExpensesQuery(string UserId, string Format) : IRequest<ExportFileResultDto>;