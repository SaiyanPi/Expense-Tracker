using AutoMapper;
using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;

namespace ExpenseTracker.Application.Features.Expenses.Commands.UpdateExpense;

public class UpdateExpenseCommandHandler : IRequestHandler<UpdateExpenseCommand, Unit>
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IMapper _mapper;

    public UpdateExpenseCommandHandler(IExpenseRepository expenseRepository, IMapper mapper)
    {
        _expenseRepository = expenseRepository;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(UpdateExpenseCommand request, CancellationToken cancellationToken)
    {
        var expense = await _expenseRepository.GetByIdAsync(request.Id, cancellationToken);
        if (expense == null)
            throw new NotFoundException(nameof(Expense), request.Id);

        _mapper.Map(request, expense);

        await _expenseRepository.UpdateAsync(expense, cancellationToken);
        return Unit.Value;
    }
}


// we are using repository directly 