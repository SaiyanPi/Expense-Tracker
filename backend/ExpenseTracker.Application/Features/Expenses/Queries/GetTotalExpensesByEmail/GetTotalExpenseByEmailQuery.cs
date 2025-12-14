using ExpenseTracker.Application.DTOs.Expense;
using MediatR;

namespace ExpenseTracker.Application.Features.Expenses.Queries.GetTotalExpensesByEmail;

public record GetTotalExpenseByEmailQuery(string Email) : IRequest<TotalExpenseDto>;