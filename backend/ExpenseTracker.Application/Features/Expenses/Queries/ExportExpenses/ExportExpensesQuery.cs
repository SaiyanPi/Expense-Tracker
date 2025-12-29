using ExpenseTracker.Application.DTOS.Expense;
using MediatR;

namespace ExpenseTracker.Application.Features.Expenses.Queries.ExportExpenses;

public record ExportExpensesQuery(
    DateTime startDate, 
    DateTime endDate, 
    string Format) : IRequest<ExportFileResultDto>;