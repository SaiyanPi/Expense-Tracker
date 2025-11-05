using ExpenseTracker.Application.DTOs.Expense;
using MediatR;

namespace ExpenseTracker.Application.Features.Expenses.Queries.GetExpenseById;

public record GetExpenseByIdQuery(Guid Id) : IRequest<ExpenseDto>;