using ExpenseTracker.Application.DTOs.Expense;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;

namespace ExpenseTracker.Application.Features.Expenses.Queries.FilterExpenses;

public class FilterExpenseQueryHandler : IRequestHandler<FilterExpensesQuery, FilteredExpensesResultDto>
{
    private readonly IExpenseRepository _expenseRepository;

    public FilterExpenseQueryHandler(IExpenseRepository expenseRepository)
    {
        _expenseRepository = expenseRepository;
    }

    public async Task<FilteredExpensesResultDto> Handle(FilterExpensesQuery request, CancellationToken cancellationToken)
    {
        var filteredExpenses = await _expenseRepository.FilterExpensesAsync(
            request.StartDate,
            request.EndDate,
            request.MinAmount,
            request.MaxAmount,
            request.CategoryId,
            request.UserId,
            cancellationToken);

    
        return new FilteredExpensesResultDto
        {
            TotalAmount = filteredExpenses.TotalAmount ,
            Expenses = filteredExpenses.Expenses.Select(e => new FilteredExpenseDto
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description,
                Amount = e.Amount,
                Date = e.Date,
                CategoryId = e.CategoryId ?? Guid.Empty,
                CategoryName = e.Category?.Name ?? string.Empty,
                UserId = e.UserId ?? string.Empty,
            }).ToList()
        };
    }
}