using ExpenseTracker.Application.DTOs.Expense;
using MediatR;

namespace ExpenseTracker.Application.Features.Expenses.Queries.GetAllExpensesByEmail;

public record GetAllExpensesByEmailQuery(string Email) : IRequest<IReadOnlyList<ExpenseDto>>;