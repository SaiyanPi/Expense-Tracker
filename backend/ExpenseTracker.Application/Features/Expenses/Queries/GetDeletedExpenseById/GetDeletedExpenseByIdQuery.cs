using ExpenseTracker.Application.DTOs.Expense;
using MediatR;

namespace ExpenseTracker.Application.Features.Expenses.Queries.GetDeletedExpenseById;

public record GetDeletedExpenseByIdQuery(Guid Id) : IRequest<ExpenseDto>;