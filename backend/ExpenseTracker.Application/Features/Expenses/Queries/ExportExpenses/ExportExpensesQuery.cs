using ExpenseTracker.Application.Common.Pagination;
using ExpenseTracker.Application.DTOS.Expense;
using ExpenseTracker.Application.DTOS.FileExport;
using MediatR;

namespace ExpenseTracker.Application.Features.Expenses.Queries.ExportExpenses;

public record ExportExpensesQuery(
    string Format, ExpenseFilter Filter) : IRequest<ExportFileResultDto>;