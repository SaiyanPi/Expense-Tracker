using ExpenseTracker.Application.DTOs.Expense;
using MediatR;

namespace ExpenseTracker.Application.Features.Expenses.Queries.GetAllExpenses;

public record GetAllExpensesQuery() : IRequest<IReadOnlyList<ExpenseDto>>;