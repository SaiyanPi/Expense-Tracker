using ExpenseTracker.Application.Features.Expenses.GetTotalExpenses;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;

namespace ExpenseTracker.Application.DTOs.Expense;

public class GetTotalExpensesQueryHandler : IRequestHandler<GetTotalExpensesQuery, TotalExpenseDto>
{
    private readonly IExpenseRepository _expenseRepository;

    public GetTotalExpensesQueryHandler(IExpenseRepository expenseRepository)
    {
        _expenseRepository = expenseRepository;
    }

    public async Task<TotalExpenseDto> Handle(GetTotalExpensesQuery request, CancellationToken cancellationToken)
    {
        var totalAmount = await _expenseRepository.GetTotalExpenseAsync(cancellationToken);
        return new TotalExpenseDto { TotalAmount = totalAmount }; 
    }
}