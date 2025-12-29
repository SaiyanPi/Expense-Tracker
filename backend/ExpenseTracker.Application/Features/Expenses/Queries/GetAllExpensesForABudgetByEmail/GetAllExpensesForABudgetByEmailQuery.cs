using ExpenseTracker.Application.Common.Pagination;
using ExpenseTracker.Application.DTOs.Expense;
using ExpenseTracker.Application.DTOS.Expense;
using MediatR;

namespace ExpenseTracker.Application.Features.Expenses.Queries.GetAllExpensesForABudgetByEmail;

public record GetAllExpensesForABudgetByEmailQuery(Guid BudgetId, PagedQuery Paging) : IRequest<PagedResult<ExpenseSummaryForBudgetDto>>;