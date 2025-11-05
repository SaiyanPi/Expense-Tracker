using AutoMapper;
using ExpenseTracker.Application.DTOs.Expense;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;

namespace ExpenseTracker.Application.Features.Expenses.Commands.CreateExpense;

public class CreateExpenseCommandHandler : IRequestHandler<CreateExpenseCommand, ExpenseDto>
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IMapper _mapper;

    public CreateExpenseCommandHandler(IExpenseRepository expenseRepository, IMapper mapper)
    {
        _expenseRepository = expenseRepository;
        _mapper = mapper;
    }

    public async Task<ExpenseDto> Handle(CreateExpenseCommand request, CancellationToken cancellationToken)
    {
        var expense = _mapper.Map<Expense>(request.ExpenseDto);
        await _expenseRepository.AddAsync(expense, cancellationToken);
        return _mapper.Map<ExpenseDto>(expense);
    }
}