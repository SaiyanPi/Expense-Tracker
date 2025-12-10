using ExpenseTracker.Application.DTOs.Expense;
using MediatR;

namespace ExpenseTracker.Application.Features.Expenses.Queries.GetAllExpensesForABudgetByEmail;

public record GetAllExpensesForABudgetByEmailQuery(Guid BudgetId, string Email) : IRequest<IReadOnlyList<ExpenseDto>>;