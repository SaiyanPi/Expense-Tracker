using AutoMapper;
using ExpenseTracker.Application.DTOs.Expense;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;

namespace ExpenseTracker.Application.Features.Expenses.Queries.GetAllExpenses;

public class GetAllExpensesQueryHandler : IRequestHandler<GetAllExpensesQuery, IReadOnlyList<ExpenseDto>>
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IMapper _mapper;

    public GetAllExpensesQueryHandler(IExpenseRepository expenseRepository, IMapper mapper)
    {
        _expenseRepository = expenseRepository;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<ExpenseDto>> Handle(GetAllExpensesQuery request, CancellationToken cancellationToken)
    {
        var expenses = await _expenseRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<ExpenseDto>>(expenses);
    }
}