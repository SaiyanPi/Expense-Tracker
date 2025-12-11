using ExpenseTracker.Application.DTOs.Budget;
using ExpenseTracker.Domain.Models;
using MediatR;

namespace ExpenseTracker.Application.Features.Budgets.Queries.GetBudgetDetailWithExpensesByEmail;

public record GetBudgetDetailWithExpensesByEmailQuery(Guid BudgetId, string email) : IRequest<BudgetDetailWithExpensesDto>;