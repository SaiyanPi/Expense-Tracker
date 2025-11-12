using ExpenseTracker.Application.DTOs.Expense;
using MediatR;

namespace ExpenseTracker.Application.Features.Expenses.Commands.CreateExpense;

public record CreateExpenseCommand(CreateExpenseDto CreateExpenseDto) : IRequest<ExpenseDto>;