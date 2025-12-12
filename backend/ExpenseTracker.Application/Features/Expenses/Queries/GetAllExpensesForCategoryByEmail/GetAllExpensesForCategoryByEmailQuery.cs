using ExpenseTracker.Application.DTOs.Expense;
using ExpenseTracker.Domain.Models;
using MediatR;

namespace ExpenseTracker.Application.Features.Expenses.Queries.GetAllExpensesForCategoryByEmail;

public record GetAllExpensesForCategoryByEmailQuery(Guid CategoryId, string Email) : IRequest<IReadOnlyList<ExpenseDto>>;