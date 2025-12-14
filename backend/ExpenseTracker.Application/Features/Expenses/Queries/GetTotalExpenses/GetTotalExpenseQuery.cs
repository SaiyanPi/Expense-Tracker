using ExpenseTracker.Application.DTOs.Expense;
using MediatR;

namespace ExpenseTracker.Application.Features.Expenses.GetTotalExpenses;

public record GetTotalExpensesQuery() : IRequest<TotalExpenseDto>;